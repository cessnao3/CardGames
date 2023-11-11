using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using CardGameLibrary.Messages;
using CardGameLibrary.Network;

namespace CardClient.Forms
{
    public partial class LoginWindow : Form
    {
        public string? OutputUsername { get; private set; }
        public string? OutputPassword { get; private set; }

        public LoginWindow()
        {
            InitializeComponent();

            DialogResult = DialogResult.Cancel;
            AcceptButton = BtnLogin;
        }

        private void BtnLogin_Click(object sender, EventArgs e)
        {
            string hostname = TxtHost.Text.ToLower().Trim();
            string username = TxtUser.Text.ToLower().Trim();
            string password = TxtPass.Text.ToLower().Trim();

            // Try to set the hostname
            if (!Network.GameComms.SetHost(hostname))
            {
                MessageBox.Show(this, "Please check hostname");
            }

            bool isNewUser = (Button)sender == BtnNew;

            if (username.Length > 0 && password.Length > 0)
            {
                // Save output values
                OutputUsername = username;
                OutputPassword = password;

                // Convert the bytes to a hex string
                StringBuilder sb = new();
                foreach (var b in System.Security.Cryptography.MD5.HashData(Encoding.ASCII.GetBytes(OutputPassword)))
                {
                    sb.Append(b.ToString("x2"));
                }

                // Create the message
                MsgLogin msg = new(isNewUser ? MsgLogin.ActionType.NewUser : MsgLogin.ActionType.LoginUser, username, sb.ToString());

                try
                {
                    Network.GameComms.ResetSocket();
                }
                catch (System.Net.Sockets.SocketException)
                {
                    MessageBox.Show(this, "Unable to connect to server");
                    return;
                }

                if (chkUseSecure.Checked)
                {
                    if (!Network.GameComms.SetupSSL())
                    {
                        MessageBox.Show(this, "Unable to setup SSL connection");
                        return;
                    }
                }

                Network.GameComms.SendMessage(msg);

                MsgServerResponse? msg_response = null;

                for (int i = 0; i < 10; ++i)
                {
                    MsgBase? msg_b = Network.GameComms.ReceiveMessage();

                    if (msg_b == null)
                    {
                        Thread.Sleep(100);
                    }
                    else
                    {
                        if (msg_b is MsgServerResponse resp)
                        {
                            msg_response = resp;
                        }

                        break;
                    }
                }

                if (msg_response != null && msg_response.ResponseCode == MsgServerResponse.ResponseCodes.OK)
                {
                    Network.GameComms.SetPlayer(msg_response.User);
                    DialogResult = DialogResult.OK;
                    Close();
                }
                else
                {
                    MessageBox.Show(this, "Login Failed");
                }
            }
        }

        private void TxtBox_UpdateText(object sender, EventArgs e)
        {
            string string_val = ((TextBox)sender).Text.Trim().ToLower();
            int i = 0;
            while (i < string_val.Length)
            {
                if ((string_val[i] >= 'a' && string_val[i] <= 'z') ||
                    (string_val[i] >= '0' && string_val[i] <= '9'))
                {
                    i += 1;
                }
                else
                {
                    string_val = string_val.Remove(i);
                }
            }

            ((TextBox)sender).Text = string_val;
        }
    }
}
