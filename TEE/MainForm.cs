/*
 * Copyright 2014 eien no itari
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at:
 * 
 *  http://www.apache.org/licenses/LICENSE-2.0
 *  
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace TEE
{
    public partial class MainForm : Form
    {
        #region Internal
        // todo: load names from ROM
        private string[] types = new string[] { "Normal", "Fighting", "Flying", "Poison", "Ground", "Rock", "Bug", "Ghost", "Steel", "???", "Fire", "Water", "Grass", "Electric", "Psychic", "Ice", "Dragon", "Dark"};
        private Dictionary<string, uint[]> pointers;

        private Dictionary<string, uint> typeNamePointers;

        private enum Effect : byte
        {
            None = 0,
            Half = 5,
            Double = 20
        }

        private class Entry
        {
            public byte Type;
            public Effect Effectiveness;
            public bool Foresight;
        }

        #endregion

        private GBAFile gba = null;
        private string gbaCode = string.Empty;

        private uint[] tablePointers = null;
        private List<Entry>[] table = null;
        private int entries = 0;

        private int selectedType = -1;
        private bool editingType = false;

        private bool mc = false;

        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            // Load roms.dat
            pointers = new Dictionary<string, uint[]>();
            typeNamePointers = new Dictionary<string, uint>();

            StreamReader sr = File.OpenText(Path.Combine(Environment.CurrentDirectory, "roms.dat"));
            while (true)
            {
                // I'm not even gonna bother being safe about checking this stuff.
                string line = sr.ReadLine().Trim();
                if (line == "eof") break;

                string[] parts = line.Split('=');
                //offsets.Add(parts[0], Convert.ToUInt32(parts[1], 16));
                string[] list = parts[1].Split(',');
                // If it doesn't work, it crashes. Oh well.
                uint[] p = new uint[list.Length];
                for (int i = 0; i < list.Length; i++) p[i] = Convert.ToUInt32(list[i], 16);

                pointers.Add(parts[0], p);
            }

            while (true)
            {
                // I'm not even gonna bother being safe about checking this stuff.
                string line = sr.ReadLine().Trim();
                if (line == "eof") break;

                string[] parts = line.Split('=');
                // If it doesn't work, it crashes. Oh well.
                typeNamePointers.Add(parts[0], Convert.ToUInt32(parts[1], 16));
            }
            sr.Close();
            sr.Dispose();

            LockControls();
            StyleForm();
        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (gba != null) gba.Close();
        }

        private void StyleForm()
        {
            // menu
            menu.Renderer = new ToolStripProfessionalRenderer(new MenuColorTable());
            mnuFile.ForeColor = Color.White;
            mnuOpen.ForeColor = Color.White;
            mnuSave.ForeColor = Color.White;
            mnuExit.ForeColor = Color.White;

            mnuActions.ForeColor = Color.White;
            mnuClear.ForeColor = Color.White;

            mnuHelp.ForeColor = Color.White;
            mnuAbout.ForeColor = Color.White;            

            // misc
            chkForesight.ForeColor = Color.White;

            tName.BackColor = Color.FromArgb(30, 30, 30);
            tName.ForeColor = Color.White;

            // form
            ForeColor = Color.White;
            BackColor = Color.FromArgb(45, 45, 48);
        }

        #region Menu

        private void mnuOpen_Click(object sender, EventArgs e)
        {
            open.FileName = "";
            open.Title = "Open ROM";
            open.Filter = "GBA ROMs|*.gba";

            if (open.ShowDialog() != DialogResult.OK) return;

            // Open for IO
            if (gba != null) gba.Close();
            gba = new GBAFile(open.FileName);

            // Read GBA header specifics
            gba.Position = 0xAC;
            gbaCode = gba.ReadString(4); // Only this right now

            // Get pointers
            if (!pointers.ContainsKey(gbaCode))
            {
                MessageBox.Show("Game " + gbaCode + " is not supported!", "Invalid ROM", MessageBoxButtons.OK, MessageBoxIcon.Error);
                gba.Close();

                LockControls();
                table = null;
                return;
            }
            tablePointers = pointers[gbaCode];

            // Reset
            gba.Position = 0x0;

            // LOAD!!
            LoadTypeNames();
            LoadTable();
            UnlockControls();
        }

        private void mnuSave_Click(object sender, EventArgs e)
        {
            if (gba == null) return;

            // save
            if (!SaveTable()) return;
            SaveTypeNames(); // bonus

            // reload
            LoadTable();
            MessageBox.Show("Saved!", "Success!", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        }

        private void mnuExit_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void mnuClear_Click(object sender, EventArgs e)
        {
            if (table != null)
            {
                for (int i = 0; i < 18; i++)
                {
                    table[i].Clear();
                }
                pGrid.Invalidate();
            }
        }

        private void mnuAbout_Click(object sender, EventArgs e)
        {
            //MessageBox.Show("Type Effectiveness Editor\nVersion 2.0 (3/8/2014)\nBy eien no itari\n\nA simple tool to edit the type strength/weakness table in the GBA Pokémon games.\n\nYay!\nThis version has an actual visual editor!", "About", MessageBoxButtons.OK, MessageBoxIcon.Information);
            AboutDialog ad = new AboutDialog();
            ad.ShowDialog();
        }

        #endregion

        private void LockControls()
        {           
            mnuSave.Enabled = false;
            mnuClear.Enabled = false;

            //chkForesight.Enabled = false;
            tName.Visible = false;
        }

        private void UnlockControls()
        {
            mnuSave.Enabled = true;
            mnuClear.Enabled = true;

            chkForesight.Enabled = true;
        }

        #region Load/Save

        private void LoadTypeNames()
        {
            uint offset = typeNamePointers[gbaCode];
            gba.Seek(offset);

            offset = gba.ReadPointer();
            gba.Seek(offset);

            for (int i = 0; i < 18; i++)
            {
                string name = TextTable.GetString(gba.ReadBytes(7));
                types[i] = name;
            }

            // Draw
            pAtk.Invalidate();
            pDef.Invalidate();
            pLbl.Invalidate();
        }

        private void SaveTypeNames()
        {
            uint offset = typeNamePointers[gbaCode];
            gba.Seek(offset);

            offset = gba.ReadPointer();
            gba.Seek(offset);

            for (int i = 0; i < 18; i++)
            {
                string name = types[i];
                byte[] buffer = TextTable.GetBytes(name, 6);
                
                Array.Resize(ref buffer, 7);
                buffer[6] = 0xFF;

                gba.WriteBytes(buffer);
            }
        }

        private void LoadTable()
        {
            gba.Seek(tablePointers[0]); // Just go to the first
            uint offset = gba.ReadPointer();

            // Load Table
            gba.Seek(offset);
            table = new List<Entry>[18];
            for (int i = 0; i < 18; i++)
                table[i] = new List<Entry>();
            entries = 0;

            // Normal Table
            while (true)
            {
                byte attacker = gba.ReadByte();
                byte defender = gba.ReadByte();
                byte effect = gba.ReadByte();

                Entry e = new Entry();
                e.Type = defender;
                e.Effectiveness = (Effect)effect;
                e.Foresight = false;

                table[attacker].Add(e);
                entries++;

                if (gba.PeekByte() == 0xFE) break;
            }

            // Moves effected by Foresight
            gba.ReadWord(); // FE FE
            gba.ReadByte(); // 00
            while (gba.PeekByte() != 0xFF)
            {
                byte attacker = gba.ReadByte();
                byte defender = gba.ReadByte();
                byte effect = gba.ReadByte();

                Entry e = new Entry();
                e.Type = defender;
                e.Effectiveness = (Effect)effect;
                e.Foresight = true;

                table[attacker].Add(e);
                entries++;
            }

            // End of Table:
            // FF FF
            // 00

            pGrid.Invalidate();
        }

        private bool SaveTable()
        {
            List<byte> normal = new List<byte>();
            List<byte> foresight = new List<byte>();

            // split data for writing
            int num = 0;
            for (byte a = 0; a < 18; a++)
            {
                Entry[] row = table[a].ToArray();
                for (int i = 0; i < row.Length; i++)
                {
                    if (row[i].Foresight)
                    {
                        foresight.Add(a);
                        foresight.Add(row[i].Type);
                        foresight.Add((byte)row[i].Effectiveness);
                    }
                    else
                    {
                        normal.Add(a);
                        normal.Add(row[i].Type);
                        normal.Add((byte)row[i].Effectiveness);
                    }
                    num++;
                }
            }

            // overwrite old data
            int oldSpace = 6 + entries * 3;
            int neededSpace = 6 + num * 3;            

            // repoint?
            if (neededSpace > oldSpace)
            {
                if (MessageBox.Show("The new table is too large for the old data!\nThe tool will try to re-point it.\n\nContinue?", "Repoint?", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) != DialogResult.OK) return false;

                int fsOffset = gba.FindFreeSpace(neededSpace, 0xFF);
                if (fsOffset == -1)
                {
                    MessageBox.Show("Not enough free space in ROM!\nUnable to repoint.\n\nSave aborted.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }

                // overwrite old data
                gba.Seek(tablePointers[0]);
                gba.Seek(gba.ReadPointer());
                for (int i = 0; i < 6 + entries * 3; i++)
                {
                    gba.WriteByte(0xFF);
                }

                // re-point
                uint ufsOffset = (uint)fsOffset;
                for (int ptr = 0; ptr < tablePointers.Length; ptr++)
                {
                    uint ptrOffset = tablePointers[ptr];
                    
                    gba.Seek(ptrOffset);
                    gba.WritePointer(ufsOffset);
                }
            }
            else
            {
                // overwrite old data
                gba.Seek(tablePointers[0]);
                gba.Seek(gba.ReadPointer());
                for (int i = 0; i < 6 + entries * 3; i++)
                {
                    gba.WriteByte(0xFF);
                }
            }

            // write data
            gba.Seek(tablePointers[0]);
            gba.Seek(gba.ReadPointer());

            // normal entries
            if (normal.Count > 0)
            {
                gba.WriteBytes(normal.ToArray());
            }
            gba.WriteBytes(new byte[] { 0xFE, 0xFE, 0x00 });
            // foresight
            if (foresight.Count > 0)
            {
                gba.WriteBytes(foresight.ToArray());
            }
            gba.WriteBytes(new byte[] { 0xFF, 0xFF, 0x00 });

            return true;
        }

        #endregion

        #region Controls

        private void chkForesight_CheckedChanged(object sender, EventArgs e)
        {
            if (mc) return;
            // pass...
        }

        private void pAtk_Paint(object sender, PaintEventArgs e)
        {
            Pen p = new Pen(new SolidBrush(Color.FromArgb(63, 63, 70)));
            e.Graphics.DrawLine(p, 0, 0, 0, 432);
            for (int i = 0; i <= 18; i++)
            {
                e.Graphics.DrawLine(p, 0, i * 24, 432, i * 24);
            }
            
            if (table != null)
            {
                for (int t = 0; t < 18; t++)
                {
                    if (t == selectedType)
                    {
                        e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(30, 30, 30)), 1, t * 24 + 1, 63, 23);
                    }
                    else
                    {
                        e.Graphics.DrawString(types[t], Font, Brushes.White, 0, 10 + t * 24);
                    }
                }
            }
        }

        private void pDef_Paint(object sender, PaintEventArgs e)
        {
            Pen p = new Pen(new SolidBrush(Color.FromArgb(63, 63, 70)));
            e.Graphics.DrawLine(p, 0, 0, 432, 0);
            for (int i = 0; i <= 18; i++)
            {
                e.Graphics.DrawLine(p, i * 24, 0, i * 24, 432);
            }

            if (table != null)
            {
                StringFormat sform = new StringFormat(StringFormatFlags.DirectionVertical);
                for (int t = 0; t < 18; t++)
                {
                    if (t == selectedType)
                    {
                        e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(30, 30, 30)), 1 + t * 24, 1, 23, 63);
                    }

                    e.Graphics.DrawString(types[t], Font, Brushes.White, t * 24, 0, sform);
                }
            }
        }

        private void pLbl_Paint(object sender, PaintEventArgs e)
        {
            Pen p = new Pen(new SolidBrush(Color.FromArgb(63, 63, 70)));
            e.Graphics.DrawLine(p, 0, 0, 0, 64);
            e.Graphics.DrawLine(p, 0, 0, 64, 0);

            if (table != null)
            {
                StringFormat sform = new StringFormat(StringFormatFlags.DirectionVertical);
                e.Graphics.DrawString("Attacker", Font, Brushes.White, 0, 50);
                e.Graphics.DrawString("Defender", Font, Brushes.White, 48, 0, sform);
            }
        }

        private void pGrid_Paint(object sender, PaintEventArgs e)
        {
            if (table != null)
            {
                //Font f = new Font(Font.FontFamily.Name, 6.0f, FontStyle.Regular);
                for (int atk = 0; atk < 18; atk++)
                {
                    Entry[] attacks = table[atk].ToArray();
                    for (int def = 0; def < attacks.Length; def++)
                    {
                        switch (attacks[def].Effectiveness)
                        {
                            case Effect.None:
                                e.Graphics.FillRectangle(Brushes.Red, new Rectangle(attacks[def].Type * 24, atk * 24, 24, 24));
                                break;
                            case Effect.Half:
                                e.Graphics.FillRectangle(Brushes.Yellow, new Rectangle(attacks[def].Type * 24, atk * 24, 24, 24));
                                break;
                            case Effect.Double:
                                e.Graphics.FillRectangle(Brushes.Green, new Rectangle(attacks[def].Type * 24, atk * 24, 24, 24));
                                break;
                        }

                        //e.Graphics.DrawString(((int)attacks[def].Effectiveness * 10) + "%", f, Brushes.Black, attacks[def].Type * 24, atk * 24);

                        if (attacks[def].Foresight)
                        {
                            e.Graphics.DrawString("FS", Font, Brushes.White, 6 + attacks[def].Type * 24, 10 + atk * 24);
                        }
                    }
                }
            }

            Pen p = new Pen(new SolidBrush(Color.FromArgb(63, 63, 70)));
            for (int i = 0; i <= 18; i++)
            {
                e.Graphics.DrawLine(p, 0, i * 24, 432, i * 24);
                e.Graphics.DrawLine(p, i * 24, 0, i * 24, 432);
            }
        }

        private void pGrid_MouseUp(object sender, MouseEventArgs e)
        {
            int x = e.X / 24;
            int y = e.Y / 24;
            
            if (x < 0 || y < 0 || x >= 18 | y >= 18) return;

            if (table == null) return;

            if (e.Button == MouseButtons.Left)
            {
                bool contains = false;
                for (int i = 0; i < table[y].Count; i++)
                {
                    if (table[y][i].Type == x)
                    {
                        contains = true;
                        switch (table[y][i].Effectiveness)
                        {
                            case Effect.None:
                                table[y][i].Effectiveness = Effect.Half;
                                break;
                            case Effect.Half:
                                table[y][i].Effectiveness = Effect.Double;
                                break;
                            case Effect.Double:
                                table[y][i].Effectiveness = Effect.None;
                                break;
                        }

                        table[y][i].Foresight = chkForesight.Checked;

                        pGrid.Invalidate();
                        break;
                    }
                }

                if (!contains)
                {
                    Entry ee = new Entry()
                    {
                        Effectiveness = Effect.None,
                        Foresight = chkForesight.Checked,
                        Type = (byte)x
                    };
                    table[y].Add(ee);
                    pGrid.Invalidate();
                }
            }
            else if (e.Button == MouseButtons.Right)
            {
                for (int i = 0; i < table[y].Count; i++)
                {
                    if (table[y][i].Type == x)
                    {
                        table[y].RemoveAt(i);
                        pGrid.Invalidate();
                        break;
                    }
                }
            }
        }

        private void pAtk_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (table == null) return;

            int y = e.Y / 24;
            if (y < 0 || y >= 18) return;

            selectedType = y;
            editingType = true;

            // move tName
            mc = true;
            tName.Location = new Point(13, 115 + 8 + y * 24);
            tName.Visible = true;
            tName.Text = types[selectedType];
            tName.SelectAll();
            mc = false;

            pDef.Invalidate();
            pAtk.Invalidate();
        }

        private void tName_TextChanged(object sender, EventArgs e)
        {
            if (mc) return;

            if (editingType && tName.TextLength > 0)
            {
                types[selectedType] = tName.Text.TrimEnd();
                pAtk.Invalidate();
                pDef.Invalidate();
            }
        }

        private void tName_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (editingType)
            {
                if (e.KeyCode == Keys.Enter)
                {
                    editingType = false;
                    selectedType = -1;
                    tName.Visible = false;
                    
                    pAtk.Invalidate();
                    pDef.Invalidate();
                }
            }
        }

        #endregion
    }
}
