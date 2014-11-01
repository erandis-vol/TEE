using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace TEE3
{
    public partial class Form1 : Form
    {
        private string filePath = string.Empty;
        private Dictionary<string, uint> tablePointers, namePointers;
        private string fileCode = string.Empty;

        private string[] typeNames = null;
        private int numberOfTypes = 0, originalNumberOfTypes = 0;
        private List<Entry>[] table;

        private bool waitingForRemovalSelection = false;
        private int selectedType = 0;
        private bool editingType = false, mc = false;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            tablePointers = new Dictionary<string, uint>();
            namePointers = new Dictionary<string, uint>();
            SetGameOffsets();
            mnuTable.Enabled = false;
            tName.Visible = false;
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {

        }

        private void mnuOpen_Click(object sender, EventArgs e)
        {
            // Get the file.
            openFileDialog1.Title = "Open ROM";
            openFileDialog1.Filter = "GBA ROMs|*.gba";
            openFileDialog1.FileName = "";
            if (openFileDialog1.ShowDialog() != DialogResult.OK) return;

            // Check it.
            if (!File.Exists(openFileDialog1.FileName))
            {
                MessageBox.Show("File does not exist!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Load it.
            
            using (BinaryReader br = new BinaryReader(File.OpenRead(openFileDialog1.FileName)))
            {
                // Get ROM code
                br.BaseStream.Seek(0xAC, SeekOrigin.Begin);
                string code = Encoding.UTF8.GetString(br.ReadBytes(4));

                // Load parts
                if (namePointers.ContainsKey(code))
                {
                    // Set these things to remember.
                    filePath = openFileDialog1.FileName;
                    fileCode = code;

                    // Load the stuffz
                    LoadTable(br);
                    LoadTypeNames(br);

                    // Draw
                    pAtk.Height = 24 * numberOfTypes + 1;
                    pDef.Width = 24 * numberOfTypes + 1;
                    pGrid.Height = 24 * numberOfTypes + 1;
                    pGrid.Width = 24 * numberOfTypes + 1;
                    this.Height = 165 + 24 * numberOfTypes;
                    this.Width = 106 + 24 * numberOfTypes;
                    this.CenterToScreen();

                    pLbl.Invalidate();
                    pAtk.Invalidate();
                    pDef.Invalidate();
                    pGrid.Invalidate();
                    mnuTable.Enabled = true;
                }
                else
                {
                    MessageBox.Show("Unsupported ROM " + code + "!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void mnuSave_Click(object sender, EventArgs e)
        {
            if (table != null && numberOfTypes > 0)
            {
                SaveTable();
                SaveTypeNames();
            }
        }

        private void mnuExit_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void mnuClear_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < table.Length; i++)
            {
                table[i].Clear();
            }
            pGrid.Invalidate();
        }

        private void mnuAdd_Click(object sender, EventArgs e)
        {
            if (numberOfTypes == 0xFE)
            {
                MessageBox.Show("Cannot add anymore types!", "Darn!", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            // Resize arrays
            numberOfTypes++;
            Array.Resize(ref typeNames, numberOfTypes);
            typeNames[numberOfTypes - 1] = "NEWTYP";

            Array.Resize(ref table, numberOfTypes);
            table[numberOfTypes - 1] = new List<Entry>();

            // Resize form & draw.
            pAtk.Height = 24 * numberOfTypes + 1;
            pDef.Width = 24 * numberOfTypes + 1;
            pGrid.Height = 24 * numberOfTypes + 1;
            pGrid.Width = 24 * numberOfTypes + 1;
            this.Height = 165 + 24 * numberOfTypes;
            this.Width = 106 + 24 * numberOfTypes;
            this.CenterToScreen();

            pLbl.Invalidate();
            pAtk.Invalidate();
            pDef.Invalidate();
            pGrid.Invalidate();
        }

        private void mnuRemove_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Would you like to remove a type?\nIf so, select a type from the left.\n\n(Sorry this is so messy. ^_^)", "Remove type?", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No) return;
            waitingForRemovalSelection = true;
            if (editingType)
            {
                editingType = false;
                selectedType = -1;
                tName.Visible = false;

                pAtk.Invalidate();
                pDef.Invalidate();
            }
        }

        private void mnuAbout_Click(object sender, EventArgs e)
        {
            AboutDialog a = new AboutDialog();
            a.ShowDialog();
        }

        private void SetGameOffsets()
        {
            // I have this so I don't need an included file.

            // Clear
            tablePointers.Clear();
            namePointers.Clear();

            // Set table pointers
            tablePointers.Add("AXPE", 0x1CDC8);
            tablePointers.Add("AXVE", 0x1CDC8);
            tablePointers.Add("BPGE", 0x1E944);
            tablePointers.Add("BPRE", 0x1E944);
            tablePointers.Add("BPEE", 0x47134);

            // Set name pointers
            namePointers.Add("AXPE", 0x2E3A8);
            namePointers.Add("AXVE", 0x2E3A8);
            namePointers.Add("BPGE", 0x309C8);
            namePointers.Add("BPRE", 0x309C8);
            namePointers.Add("BPEE", 0x166F4);

            // I can add more later.
        }

        private void LoadTable(BinaryReader br)
        {
            // Get table offset
            br.BaseStream.Seek(tablePointers[fileCode], SeekOrigin.Begin);
            uint tableStart = br.ReadUInt32();
            if (tableStart <= 0x8000000)
            {
                MessageBox.Show("Bad type effectiveness table pointer!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            else tableStart -= 0x08000000;

            // Goto
            br.BaseStream.Seek(tableStart, SeekOrigin.Begin);
            //table.Clear();
            //table.Add(new List<Entry>()); // To make sure type 0 works ;)
            table = new List<Entry>[1];
            this.Text = "Type Effectiveness Editor 3.0 - Table @ 0x" + tableStart.ToString("X");

            int count = 0;
            while (br.BaseStream.Position < br.BaseStream.Length - 3)
            {
                byte a = br.ReadByte();
                byte d = br.ReadByte();
                byte e = br.ReadByte();

                // Resize if necessary.
                if (a + 1 > table.Length)
                {
                    Array.Resize(ref table, a + 1);
                }

                if (d + 1 > table.Length)
                {
                    Array.Resize(ref table, d + 1);
                }

                // Fill any null entries with fresh lists.
                for (int i = 0; i < table.Length; i++)
                {
                    if (table[i] == null) table[i] = new List<Entry>();
                }

                // Create an entry
                Entry en = new Entry()
                {
                    Type = d,
                    Effectiveness = (Effect)e,
                    Foresight = false
                };

                // Add entry to table
                table[a].Add(en);
                count++;

                // Check for section ender.
                if (br.ReadByte() == 0xFE) break;
                br.BaseStream.Seek(-1, SeekOrigin.Current);
            }

            br.ReadUInt16(); // Skip the rest of that crap.
            while (br.BaseStream.Position < br.BaseStream.Length - 3)
            {
                byte a = br.ReadByte();
                byte d = br.ReadByte();
                byte e = br.ReadByte();

                // The end of this section.
                if (a == 0xFF) break;

                // Create an entry
                Entry en = new Entry()
                {
                    Type = d,
                    Effectiveness = (Effect)e,
                    Foresight = true
                };

                // Add entry to table
                table[a].Add(en);
                count++;
            }

            numberOfTypes = table.Length;
            originalNumberOfTypes = numberOfTypes;
            if (numberOfTypes != 18)
            {
                //MessageBox.Show("Detected that the game uses a different number of types than the default 18!", "Wow!", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            //MessageBox.Show("There are " + count + " entries, covering " + numberOfTypes + " types!", "Wow!", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private int LoadTableSize(BinaryReader br, out uint tableOffset)
        {
            // Get table offset
            br.BaseStream.Seek(tablePointers[fileCode], SeekOrigin.Begin);
            uint tableStart = br.ReadUInt32() - 0x8000000;
            tableOffset = tableStart;

            // Goto
            int size = 0;
            br.BaseStream.Seek(tableStart, SeekOrigin.Begin);

            // Read normal table
            while (true)
            {
                br.ReadBytes(3);
                size += 3;

                if (br.ReadByte() == 0xFE) break;
                else br.BaseStream.Seek(-1, SeekOrigin.Current);
            }
            br.ReadUInt16();
            size += 3;

            // Read foresight table
            while (true)
            {
                br.ReadBytes(3);
                size += 3;

                if (br.ReadByte() == 0xFF) break;
                else br.BaseStream.Seek(-1, SeekOrigin.Current);
            }
            size += 3;

            // Done.
            return size;
        }

        private void SaveTable()
        {
            // Step 1. Read old table length.
            int originalSize = 0; uint tableOffset = 0;
            using (BinaryReader br = new BinaryReader(File.OpenRead(filePath)))
            {
                originalSize = LoadTableSize(br, out tableOffset);
            }

            // Step 2. Create table save
            byte[] saveTable = CreateSaveTable();
            uint saveToOffset = tableOffset;

            //MessageBox.Show("Original Size: " + originalSize + " bytes\nNew Size: " + saveTable.Length + " bytes");

            // Step 3. Repoint (if necessary)
            if (saveTable.Length > originalSize)
            {
                // Step 3a. Find free space
                FreeSpaceDialog fsDialog = new FreeSpaceDialog(filePath, (uint)saveTable.Length);
                if (fsDialog.ShowDialog() != DialogResult.OK)
                {
                    MessageBox.Show("Saving aborted!", "Abort!", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }

                // Step 3b. Find/replace pointers
                saveToOffset = fsDialog.FreeSpaceOffset;
                Tasks.FindAndReplacePointer(filePath, tableOffset, saveToOffset);

                // Step 3c. Overwrite old table
                using (BinaryWriter bw = new BinaryWriter(File.OpenWrite(filePath)))
                {
                    bw.BaseStream.Seek(tableOffset, SeekOrigin.Begin);
                    for (int i = 0; i < originalSize; i++)
                    {
                        bw.Write(byte.MaxValue);
                    }
                }
            }

            // Step 4. Write save table
            using (BinaryWriter bw = new BinaryWriter(File.OpenWrite(filePath)))
            {
                bw.BaseStream.Seek(saveToOffset, SeekOrigin.Begin);
                bw.Write(saveTable);
            }
            this.Text = "Type Effectiveness Editor 3.0 - Table @ 0x" + saveToOffset.ToString("X");
        }

        private byte[] CreateSaveTable()
        {
            List<byte> normal = new List<byte>();
            List<byte> foresight = new List<byte>();

            // split data for writing
            //int num = 0;
            for (byte a = 0; a < numberOfTypes; a++)
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
                    //num++;
                }
            }

            // Build array
            List<byte> output = new List<byte>();
            output.AddRange(normal);
            output.AddRange(new byte[] { 0xFE, 0xFE, 0x00 });
            output.AddRange(foresight);
            output.AddRange(new byte[] { 0xFF, 0xFF, 0x00 });
            return output.ToArray();
        }

        private void LoadTypeNames(BinaryReader br)
        {
            br.BaseStream.Seek(namePointers[fileCode], SeekOrigin.Begin);
            
            uint nameStart = br.ReadUInt32();
            nameStart -= 0x8000000;
            br.BaseStream.Seek(nameStart, SeekOrigin.Begin);

            typeNames = new string[numberOfTypes];
            for (int i = 0; i < numberOfTypes; i++)
            {
                typeNames[i] = TextTable.GetString(br.ReadBytes(7));
            }
        }

        private void SaveTypeNames()
        {
            // Step 1. Get original offset
            uint originalOffset = 0;
            using (BinaryReader br = new BinaryReader(File.OpenRead(filePath)))
            {
                br.BaseStream.Seek(namePointers[fileCode], SeekOrigin.Begin);
                originalOffset = br.ReadUInt32() - 0x8000000;
            }
            uint nameSaveOffset = originalOffset;

            // Step 2. Repoint if necessary
            if (numberOfTypes > originalNumberOfTypes)
            {
                // Step 2a. Find free space
                FreeSpaceDialog fsDialog = new FreeSpaceDialog(filePath, (uint)numberOfTypes * 7);
                if (fsDialog.ShowDialog() != DialogResult.OK)
                {
                    MessageBox.Show("Saving of type names aborted!", "Abort!", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }

                // Step 2b. Replace pointers
                nameSaveOffset = fsDialog.FreeSpaceOffset;
                Tasks.FindAndReplacePointer(filePath, originalOffset, nameSaveOffset);
            }
            
            // Step 3. Write names & such
            using (BinaryWriter bw = new BinaryWriter(File.OpenWrite(filePath)))
            {
                // Step 3a. Overwrite original with FF
                bw.BaseStream.Seek(originalOffset, SeekOrigin.Begin);
                for (int i = 0; i < originalNumberOfTypes * 7; i++)
                {
                    bw.Write(byte.MaxValue);
                }

                // Step 3b. Write the names
                bw.BaseStream.Seek(nameSaveOffset, SeekOrigin.Begin);
                for (int i = 0; i < numberOfTypes; i++)
                {
                    // Convert to GBA string.
                    // Limits to 6 max characters, and adds 0xFF is < 6
                    byte[] nameBuffer = TextTable.GetBytes(typeNames[i], 6);

                    // Add 0xFF to end of name (if it was 6 characters; safe otherwise)
                    Array.Resize(ref nameBuffer, 7);
                    nameBuffer[6] = byte.MaxValue;

                    // Write to file
                    bw.Write(nameBuffer);
                }
            }
        }

        private void pAtk_Paint(object sender, PaintEventArgs e)
        {
            if (numberOfTypes > 0)
            {
                e.Graphics.DrawLine(SystemPens.WindowFrame, 0, 0, 0, numberOfTypes * 24);
                for (int i = 0; i <= numberOfTypes; i++)
                {
                    e.Graphics.DrawLine(SystemPens.WindowFrame, 0, i * 24, 432, i * 24);
                }
            }

            if (typeNames != null)
            {
                for (int t = 0; t < numberOfTypes; t++)
                {
                    if (editingType && t == selectedType)
                        e.Graphics.FillRectangle(SystemBrushes.Window, 1, t * 24 + 1, 63, 23);
                    else
                        e.Graphics.DrawString(typeNames[t], Font, SystemBrushes.WindowText, 0, 10 + t * 24);
                }
            }
        }

        private void pAtk_MouseUp(object sender, MouseEventArgs e)
        {
            if (table == null || numberOfTypes == 0) return;

            // Type removal stuff
            if (waitingForRemovalSelection)
            {
                int type = e.Y / 24;
                if (type < 0 || type >= numberOfTypes) return;

                waitingForRemovalSelection = false;
                if (MessageBox.Show("Remove the " + typeNames[type] + " type?", "Remove?", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No) return;

                // Remove name
                List<string> temp2 = typeNames.ToList();
                temp2.RemoveAt(type);
                typeNames = temp2.ToArray();

                // Remove it.
                List<List<Entry>> temp = table.ToList();
                temp.RemoveAt(type);
                table = temp.ToArray();
                numberOfTypes--;
                for (int i = 0; i < numberOfTypes; i++)
                {
                    for (int y = 0; y < table[i].Count; y++)
                    {
                        if (table[i][y].Type == type)
                        {
                            table[i].RemoveAt(y);
                            break;
                        }
                    }

                    for (int y = 0; y < table[i].Count; y++)
                    {
                        if (table[i][y].Type > type)
                        {
                            table[i][y].Type -= 1;
                        }
                    }
                }

                // Resize form & draw.
                pAtk.Height = 24 * numberOfTypes + 1;
                pDef.Width = 24 * numberOfTypes + 1;
                pGrid.Height = 24 * numberOfTypes + 1;
                pGrid.Width = 24 * numberOfTypes + 1;
                this.Height = 165 + 24 * numberOfTypes;
                this.Width = 106 + 24 * numberOfTypes;
                this.CenterToScreen();

                pLbl.Invalidate();
                pAtk.Invalidate();
                pDef.Invalidate();
                pGrid.Invalidate();
            }
            else
            {
                int y = e.Y / 24;
                if (y < 0 || y >= numberOfTypes) return;

                if (!editingType)
                {
                    selectedType = y;
                    editingType = true;

                    // move tName
                    mc = true;
                    tName.Location = new Point(14, 115 + 8 + y * 24);
                    tName.Visible = true;
                    tName.Text = typeNames[selectedType];
                    tName.SelectAll();
                    mc = false;

                    pDef.Invalidate();
                    pAtk.Invalidate();
                }
                else if (editingType && y != selectedType)
                {
                    editingType = false;
                    selectedType = -1;
                    tName.Visible = false;

                    pAtk.Invalidate();
                    pDef.Invalidate();
                }
            }
        }

        private void pDef_Paint(object sender, PaintEventArgs e)
        {
            if (numberOfTypes > 0)
            {
                e.Graphics.DrawLine(SystemPens.WindowFrame, 0, 0, numberOfTypes * 24, 0);
                for (int i = 0; i <= numberOfTypes; i++)
                {
                    e.Graphics.DrawLine(SystemPens.WindowFrame, i * 24, 0, i * 24, numberOfTypes * 24);
                }
            }

            if (table != null)
            {
                StringFormat sform = new StringFormat(StringFormatFlags.DirectionVertical);
                for (int t = 0; t < numberOfTypes; t++)
                {
                    if (editingType && t == selectedType)
                    {
                        e.Graphics.FillRectangle(SystemBrushes.Window, t * 24 + 1, 1, 23, 63);
                    }
                    e.Graphics.DrawString(typeNames[t], Font, SystemBrushes.WindowText, t * 24, 0, sform);
                }
            }
        }

        private void pLbl_Paint(object sender, PaintEventArgs e)
        {
            if (numberOfTypes > 0)
            {
                e.Graphics.DrawLine(SystemPens.WindowFrame, 0, 0, 0, 64);
                e.Graphics.DrawLine(SystemPens.WindowFrame, 0, 0, 64, 0);
            }

            if (table != null)
            {
                StringFormat sform = new StringFormat(StringFormatFlags.DirectionVertical);
                e.Graphics.DrawString("Attacker", Font, Brushes.Black, 0, 50);
                e.Graphics.DrawString("Defender", Font, Brushes.Black, 48, 0, sform);
            }
        }

        private void pGrid_Paint(object sender, PaintEventArgs e)
        {
            // Draw the squares
            if (table != null && table.Length > 0)
            {
                for (int a = 0; a < numberOfTypes; a++)
                {
                    Entry[] ee = table[a].ToArray();
                    for (int d = 0; d < ee.Length; d++)
                    {
                        switch (ee[d].Effectiveness)
                        {
                            case Effect.None:
                                e.Graphics.FillRectangle(Brushes.LightCoral, new Rectangle(ee[d].Type * 24, a * 24, 24, 24));
                                break;
                            case Effect.Half:
                                e.Graphics.FillRectangle(Brushes.Khaki, new Rectangle(ee[d].Type * 24, a * 24, 24, 24));
                                break;
                            case Effect.Double:
                                e.Graphics.FillRectangle(Brushes.DarkSeaGreen, new Rectangle(ee[d].Type * 24, a * 24, 24, 24));
                                break;
                        }

                        if (ee[d].Foresight)
                        {
                            //e.Graphics.DrawString("fs", Font, Brushes.White, 6 + ee[d].Type * 24, 10 + a * 24);
                            e.Graphics.DrawRectangle(Pens.MediumOrchid, ee[d].Type * 24 + 1, a * 24 + 1, 22, 22);
                        }
                    }
                }
            }
            else
            {
                using (Font fnt = new Font(Font.FontFamily, 18f, FontStyle.Regular))
                {
                    SizeF size = e.Graphics.MeasureString("Please open a ROM to being!", fnt);
                    //StringFormat format = new StringFormat();
                    //format.Alignment = StringAlignment.Center;
                    //format.LineAlignment = StringAlignment.Center;
                    e.Graphics.DrawString("Please open a ROM to begin!", new Font(Font.FontFamily, 18f, FontStyle.Regular), SystemBrushes.WindowText, 432 / 2 - size.Width / 2 - 24, 432 / 2 - size.Height / 2 - 32);
                }
            }

            // Draw the grid
            if (numberOfTypes > 0)
            {
                for (int i = 0; i <= numberOfTypes; i++)
                {
                    e.Graphics.DrawLine(SystemPens.WindowFrame, 0, i * 24, numberOfTypes * 24, i * 24);
                    e.Graphics.DrawLine(SystemPens.WindowFrame, i * 24, 0, i * 24, numberOfTypes * 24);
                }
            }
        }

        private void pGrid_MouseUp(object sender, MouseEventArgs e)
        {
            int x = e.X / 24; // Defender
            int y = e.Y / 24; // Attacker

            // This is where all the work for table clicking happens. ;)
            if (numberOfTypes > 0 && table != null)
            {
                if (e.Button == MouseButtons.Left)
                {
                    bool addNew = true;
                    for (int a = 0; a < table[y].Count; a++)
                    {
                        if (table[y][a].Type == x)
                        {
                            table[y][a].Foresight = chkForesight.Checked;
                            switch (table[y][a].Effectiveness)
                            {
                                case Effect.None:
                                    table[y][a].Effectiveness = Effect.Half;
                                    break;
                                case Effect.Half:
                                    table[y][a].Effectiveness = Effect.Double;
                                    break;
                                case Effect.Double:
                                    table[y][a].Effectiveness = Effect.None;
                                    break;
                            }

                            addNew = false;
                            break;
                        }
                    }

                    if (addNew)
                    {
                        Entry ee = new Entry()
                        {
                            Effectiveness = Effect.None,
                            Foresight = chkForesight.Checked,
                            Type = (byte)x
                        };
                        table[y].Add(ee);
                    }

                    pGrid.Invalidate();
                }
                else if (e.Button == MouseButtons.Right)
                {
                    for (int a = 0; a < table[y].Count; a++)
                    {
                        if (table[y][a].Type == x)
                        {
                            table[y].RemoveAt(a);
                            pGrid.Invalidate();
                            break;
                        }
                    }
                }
            }
        }

        private void tName_TextChanged(object sender, EventArgs e)
        {
            if (mc) return;

            if (editingType && tName.TextLength > 0)
            {
                typeNames[selectedType] = tName.Text.TrimEnd();
                pAtk.Invalidate();
                pDef.Invalidate();
            }
        }

        private void tName_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (editingType)
            {
                if (e.KeyCode == Keys.Escape)
                {
                    editingType = false;
                    selectedType = -1;
                    tName.Visible = false;

                    pAtk.Invalidate();
                    pDef.Invalidate();
                }
            }
        }

    }
}
