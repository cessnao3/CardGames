namespace CardClient.Forms
{
    partial class LoginWindow
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
            this.TxtUser = new System.Windows.Forms.TextBox();
            this.LblUser = new System.Windows.Forms.Label();
            this.LblPassword = new System.Windows.Forms.Label();
            this.TxtPass = new System.Windows.Forms.TextBox();
            this.BtnLogin = new System.Windows.Forms.Button();
            this.BtnNew = new System.Windows.Forms.Button();
            this.TxtHost = new System.Windows.Forms.TextBox();
            this.LblHost = new System.Windows.Forms.Label();
            this.chkUseSecure = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // TxtUser
            // 
            this.TxtUser.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.TxtUser.Location = new System.Drawing.Point(71, 38);
            this.TxtUser.Name = "TxtUser";
            this.TxtUser.Size = new System.Drawing.Size(178, 20);
            this.TxtUser.TabIndex = 0;
            this.TxtUser.TextChanged += new System.EventHandler(this.TxtBox_UpdateText);
            // 
            // LblUser
            // 
            this.LblUser.AutoSize = true;
            this.LblUser.Location = new System.Drawing.Point(12, 41);
            this.LblUser.Name = "LblUser";
            this.LblUser.Size = new System.Drawing.Size(29, 13);
            this.LblUser.TabIndex = 1;
            this.LblUser.Text = "User";
            // 
            // LblPassword
            // 
            this.LblPassword.AutoSize = true;
            this.LblPassword.Location = new System.Drawing.Point(12, 67);
            this.LblPassword.Name = "LblPassword";
            this.LblPassword.Size = new System.Drawing.Size(53, 13);
            this.LblPassword.TabIndex = 2;
            this.LblPassword.Text = "Password";
            // 
            // TxtPass
            // 
            this.TxtPass.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.TxtPass.Location = new System.Drawing.Point(71, 64);
            this.TxtPass.Name = "TxtPass";
            this.TxtPass.PasswordChar = '*';
            this.TxtPass.Size = new System.Drawing.Size(178, 20);
            this.TxtPass.TabIndex = 1;
            this.TxtPass.TextChanged += new System.EventHandler(this.TxtBox_UpdateText);
            // 
            // BtnLogin
            // 
            this.BtnLogin.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.BtnLogin.Location = new System.Drawing.Point(174, 116);
            this.BtnLogin.Name = "BtnLogin";
            this.BtnLogin.Size = new System.Drawing.Size(75, 23);
            this.BtnLogin.TabIndex = 2;
            this.BtnLogin.Text = "Login";
            this.BtnLogin.UseVisualStyleBackColor = true;
            this.BtnLogin.Click += new System.EventHandler(this.BtnLogin_Click);
            // 
            // BtnNew
            // 
            this.BtnNew.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.BtnNew.Location = new System.Drawing.Point(12, 116);
            this.BtnNew.Name = "BtnNew";
            this.BtnNew.Size = new System.Drawing.Size(75, 23);
            this.BtnNew.TabIndex = 3;
            this.BtnNew.Text = "New User";
            this.BtnNew.UseVisualStyleBackColor = true;
            this.BtnNew.Click += new System.EventHandler(this.BtnLogin_Click);
            // 
            // TxtHost
            // 
            this.TxtHost.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.TxtHost.Location = new System.Drawing.Point(71, 12);
            this.TxtHost.Name = "TxtHost";
            this.TxtHost.Size = new System.Drawing.Size(178, 20);
            this.TxtHost.TabIndex = 4;
            this.TxtHost.Text = "127.0.0.1";
            // 
            // LblHost
            // 
            this.LblHost.AutoSize = true;
            this.LblHost.Location = new System.Drawing.Point(12, 15);
            this.LblHost.Name = "LblHost";
            this.LblHost.Size = new System.Drawing.Size(29, 13);
            this.LblHost.TabIndex = 7;
            this.LblHost.Text = "Host";
            // 
            // chkUseSecure
            // 
            this.chkUseSecure.AutoSize = true;
            this.chkUseSecure.Location = new System.Drawing.Point(15, 90);
            this.chkUseSecure.Name = "chkUseSecure";
            this.chkUseSecure.Size = new System.Drawing.Size(139, 17);
            this.chkUseSecure.TabIndex = 8;
            this.chkUseSecure.Text = "Use Secure Connection";
            this.chkUseSecure.UseVisualStyleBackColor = true;
            // 
            // LoginWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(261, 151);
            this.Controls.Add(this.chkUseSecure);
            this.Controls.Add(this.LblHost);
            this.Controls.Add(this.TxtHost);
            this.Controls.Add(this.BtnNew);
            this.Controls.Add(this.BtnLogin);
            this.Controls.Add(this.TxtPass);
            this.Controls.Add(this.LblPassword);
            this.Controls.Add(this.LblUser);
            this.Controls.Add(this.TxtUser);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "LoginWindow";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Login";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox TxtUser;
        private System.Windows.Forms.Label LblUser;
        private System.Windows.Forms.Label LblPassword;
        private System.Windows.Forms.TextBox TxtPass;
        private System.Windows.Forms.Button BtnLogin;
        private System.Windows.Forms.Button BtnNew;
        private System.Windows.Forms.TextBox TxtHost;
        private System.Windows.Forms.Label LblHost;
        private System.Windows.Forms.CheckBox chkUseSecure;
    }
}