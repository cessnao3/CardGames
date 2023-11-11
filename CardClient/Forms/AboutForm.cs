using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CardClient.Forms
{
    public partial class AboutForm : Form
    {
        public AboutForm()
        {
            InitializeComponent();

            LblVersion.Text = $"Version: {typeof(Program).Assembly?.GetName().Version?.ToString() ?? "Unknown"}";
        }

        private void LblCardLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            // Visit the URL
            LblCardLink.LinkVisited = true;
            System.Diagnostics.Process.Start(LblCardLink.Text);
        }

        private void BtnClose_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
