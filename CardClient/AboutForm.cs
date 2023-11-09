using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CardClient
{
    public partial class AboutForm : Form
    {
        public AboutForm()
        {
            InitializeComponent();

            LblVersion.Text = string.Format("Version: {0:}", typeof(Program).Assembly.GetName().Version.ToString());
        }

        private void LblCardLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            // Visit the URL
            LblCardLink.LinkVisited = true;
            System.Diagnostics.Process.Start(LblCardLink.Text);
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
