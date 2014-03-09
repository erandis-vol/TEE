namespace TEE
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.open = new System.Windows.Forms.OpenFileDialog();
            this.chkForesight = new System.Windows.Forms.CheckBox();
            this.menu = new System.Windows.Forms.MenuStrip();
            this.mnuFile = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.mnuHelp = new System.Windows.Forms.ToolStripMenuItem();
            this.tName = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.mnuActions = new System.Windows.Forms.ToolStripMenuItem();
            this.pictureBox3 = new System.Windows.Forms.PictureBox();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.pLbl = new System.Windows.Forms.PictureBox();
            this.pDef = new System.Windows.Forms.PictureBox();
            this.pAtk = new System.Windows.Forms.PictureBox();
            this.pGrid = new System.Windows.Forms.PictureBox();
            this.mnuOpen = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuSave = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuExit = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuClear = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuAbout = new System.Windows.Forms.ToolStripMenuItem();
            this.menu.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pLbl)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pDef)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pAtk)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pGrid)).BeginInit();
            this.SuspendLayout();
            // 
            // open
            // 
            this.open.FileName = "openFileDialog1";
            // 
            // chkForesight
            // 
            this.chkForesight.AutoSize = true;
            this.chkForesight.Location = new System.Drawing.Point(435, 27);
            this.chkForesight.Name = "chkForesight";
            this.chkForesight.Size = new System.Drawing.Size(75, 17);
            this.chkForesight.TabIndex = 7;
            this.chkForesight.Text = "Foresight?";
            this.chkForesight.UseVisualStyleBackColor = true;
            this.chkForesight.CheckedChanged += new System.EventHandler(this.chkForesight_CheckedChanged);
            // 
            // menu
            // 
            this.menu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuFile,
            this.mnuActions,
            this.mnuHelp});
            this.menu.Location = new System.Drawing.Point(0, 0);
            this.menu.Name = "menu";
            this.menu.Size = new System.Drawing.Size(522, 24);
            this.menu.TabIndex = 11;
            this.menu.Text = "menuStrip1";
            // 
            // mnuFile
            // 
            this.mnuFile.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuOpen,
            this.mnuSave,
            this.toolStripSeparator1,
            this.mnuExit});
            this.mnuFile.Name = "mnuFile";
            this.mnuFile.Size = new System.Drawing.Size(40, 20);
            this.mnuFile.Text = "FILE";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(143, 6);
            // 
            // mnuHelp
            // 
            this.mnuHelp.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuAbout});
            this.mnuHelp.Name = "mnuHelp";
            this.mnuHelp.Size = new System.Drawing.Size(47, 20);
            this.mnuHelp.Text = "HELP";
            // 
            // tName
            // 
            this.tName.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.tName.Location = new System.Drawing.Point(12, 120);
            this.tName.MaxLength = 6;
            this.tName.Name = "tName";
            this.tName.Size = new System.Drawing.Size(58, 13);
            this.tName.TabIndex = 13;
            this.tName.Text = "???";
            this.tName.TextChanged += new System.EventHandler(this.tName_TextChanged);
            this.tName.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.tName_PreviewKeyDown);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(99, 28);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(75, 13);
            this.label1.TabIndex = 15;
            this.label1.Text = "No Effect (0%)";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(203, 28);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(77, 13);
            this.label2.TabIndex = 17;
            this.label2.Text = "Not Very (50%)";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(309, 28);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(115, 13);
            this.label3.TabIndex = 19;
            this.label3.Text = "Super Effective (200%)";
            // 
            // mnuActions
            // 
            this.mnuActions.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuClear});
            this.mnuActions.Name = "mnuActions";
            this.mnuActions.Size = new System.Drawing.Size(69, 20);
            this.mnuActions.Text = "ACTIONS";
            // 
            // pictureBox3
            // 
            this.pictureBox3.BackColor = System.Drawing.Color.Green;
            this.pictureBox3.Location = new System.Drawing.Point(286, 27);
            this.pictureBox3.Name = "pictureBox3";
            this.pictureBox3.Size = new System.Drawing.Size(17, 17);
            this.pictureBox3.TabIndex = 18;
            this.pictureBox3.TabStop = false;
            // 
            // pictureBox2
            // 
            this.pictureBox2.BackColor = System.Drawing.Color.Yellow;
            this.pictureBox2.Location = new System.Drawing.Point(180, 27);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(17, 17);
            this.pictureBox2.TabIndex = 16;
            this.pictureBox2.TabStop = false;
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.Color.Red;
            this.pictureBox1.Location = new System.Drawing.Point(76, 27);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(17, 17);
            this.pictureBox1.TabIndex = 14;
            this.pictureBox1.TabStop = false;
            // 
            // pLbl
            // 
            this.pLbl.Location = new System.Drawing.Point(12, 50);
            this.pLbl.Name = "pLbl";
            this.pLbl.Size = new System.Drawing.Size(64, 64);
            this.pLbl.TabIndex = 12;
            this.pLbl.TabStop = false;
            this.pLbl.Paint += new System.Windows.Forms.PaintEventHandler(this.pLbl_Paint);
            // 
            // pDef
            // 
            this.pDef.Location = new System.Drawing.Point(76, 50);
            this.pDef.Name = "pDef";
            this.pDef.Size = new System.Drawing.Size(433, 64);
            this.pDef.TabIndex = 10;
            this.pDef.TabStop = false;
            this.pDef.Paint += new System.Windows.Forms.PaintEventHandler(this.pDef_Paint);
            // 
            // pAtk
            // 
            this.pAtk.Location = new System.Drawing.Point(12, 114);
            this.pAtk.Name = "pAtk";
            this.pAtk.Size = new System.Drawing.Size(64, 433);
            this.pAtk.TabIndex = 9;
            this.pAtk.TabStop = false;
            this.pAtk.Paint += new System.Windows.Forms.PaintEventHandler(this.pAtk_Paint);
            this.pAtk.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.pAtk_MouseDoubleClick);
            // 
            // pGrid
            // 
            this.pGrid.Location = new System.Drawing.Point(76, 114);
            this.pGrid.Name = "pGrid";
            this.pGrid.Size = new System.Drawing.Size(433, 433);
            this.pGrid.TabIndex = 8;
            this.pGrid.TabStop = false;
            this.pGrid.Paint += new System.Windows.Forms.PaintEventHandler(this.pGrid_Paint);
            this.pGrid.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pGrid_MouseUp);
            // 
            // mnuOpen
            // 
            this.mnuOpen.Image = global::TEE.Properties.Resources.folder_horizontal_open;
            this.mnuOpen.Name = "mnuOpen";
            this.mnuOpen.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.mnuOpen.Size = new System.Drawing.Size(146, 22);
            this.mnuOpen.Text = "Open";
            this.mnuOpen.Click += new System.EventHandler(this.mnuOpen_Click);
            // 
            // mnuSave
            // 
            this.mnuSave.Image = global::TEE.Properties.Resources.disk;
            this.mnuSave.Name = "mnuSave";
            this.mnuSave.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.mnuSave.Size = new System.Drawing.Size(146, 22);
            this.mnuSave.Text = "Save";
            this.mnuSave.Click += new System.EventHandler(this.mnuSave_Click);
            // 
            // mnuExit
            // 
            this.mnuExit.Image = global::TEE.Properties.Resources.cross;
            this.mnuExit.Name = "mnuExit";
            this.mnuExit.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.X)));
            this.mnuExit.Size = new System.Drawing.Size(146, 22);
            this.mnuExit.Text = "Exit";
            this.mnuExit.Click += new System.EventHandler(this.mnuExit_Click);
            // 
            // mnuClear
            // 
            this.mnuClear.Image = global::TEE.Properties.Resources.table;
            this.mnuClear.Name = "mnuClear";
            this.mnuClear.Size = new System.Drawing.Size(152, 22);
            this.mnuClear.Text = "Clear";
            this.mnuClear.Click += new System.EventHandler(this.mnuClear_Click);
            // 
            // mnuAbout
            // 
            this.mnuAbout.Image = global::TEE.Properties.Resources.information;
            this.mnuAbout.Name = "mnuAbout";
            this.mnuAbout.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.A)));
            this.mnuAbout.Size = new System.Drawing.Size(152, 22);
            this.mnuAbout.Text = "About";
            this.mnuAbout.Click += new System.EventHandler(this.mnuAbout_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(522, 559);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.pictureBox3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.pictureBox2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.tName);
            this.Controls.Add(this.pLbl);
            this.Controls.Add(this.pDef);
            this.Controls.Add(this.pAtk);
            this.Controls.Add(this.pGrid);
            this.Controls.Add(this.chkForesight);
            this.Controls.Add(this.menu);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menu;
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Type Effectiveness Editor 2.0";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MainForm_FormClosed);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.menu.ResumeLayout(false);
            this.menu.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pLbl)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pDef)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pAtk)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pGrid)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.OpenFileDialog open;
        private System.Windows.Forms.CheckBox chkForesight;
        private System.Windows.Forms.PictureBox pGrid;
        private System.Windows.Forms.PictureBox pAtk;
        private System.Windows.Forms.PictureBox pDef;
        private System.Windows.Forms.MenuStrip menu;
        private System.Windows.Forms.ToolStripMenuItem mnuFile;
        private System.Windows.Forms.ToolStripMenuItem mnuOpen;
        private System.Windows.Forms.ToolStripMenuItem mnuSave;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem mnuExit;
        private System.Windows.Forms.ToolStripMenuItem mnuHelp;
        private System.Windows.Forms.ToolStripMenuItem mnuAbout;
        private System.Windows.Forms.PictureBox pLbl;
        private System.Windows.Forms.TextBox tName;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.PictureBox pictureBox3;
        private System.Windows.Forms.ToolStripMenuItem mnuActions;
        private System.Windows.Forms.ToolStripMenuItem mnuClear;
    }
}

