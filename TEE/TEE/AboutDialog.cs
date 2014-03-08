using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace TEE
{
    public partial class AboutDialog : Form
    {
        public AboutDialog()
        {
            InitializeComponent();

           
        }

        private void AboutDialog_Load(object sender, EventArgs e)
        {
            bShow.Text = "Show License";
            this.Height = 206;
            label1.Visible = false;
        }

        private void bShow_Click(object sender, EventArgs e)
        {
            if (label1.Visible)
            {
                label1.Visible = false;
                this.Height = 206;
                bShow.Text = "Show License";
            }
            else
            {
                label1.Visible = true;
                this.Height = 375;
                bShow.Text = "Hide License";
            }
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.DrawIcon(this.Icon, new Rectangle(0, 0, 32, 32));
        }
    }
}
