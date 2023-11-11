namespace CardClient.Forms
{
    partial class AboutForm
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
            this.LblAttribution = new System.Windows.Forms.Label();
            this.LblCardLink = new System.Windows.Forms.LinkLabel();
            this.LblCreatedBy = new System.Windows.Forms.Label();
            this.LblVersion = new System.Windows.Forms.Label();
            this.LblName = new System.Windows.Forms.Label();
            this.btnClose = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // LblAttribution
            // 
            this.LblAttribution.AutoSize = true;
            this.LblAttribution.Location = new System.Drawing.Point(12, 81);
            this.LblAttribution.Name = "LblAttribution";
            this.LblAttribution.Size = new System.Drawing.Size(241, 13);
            this.LblAttribution.TabIndex = 0;
            this.LblAttribution.Text = "Card vector created by freepik - www.freepik.com";
            // 
            // LblCardLink
            // 
            this.LblCardLink.AutoSize = true;
            this.LblCardLink.Location = new System.Drawing.Point(12, 94);
            this.LblCardLink.Name = "LblCardLink";
            this.LblCardLink.Size = new System.Drawing.Size(247, 13);
            this.LblCardLink.TabIndex = 1;
            this.LblCardLink.TabStop = true;
            this.LblCardLink.Text = "https://www.freepik.com/free-photos-vectors/card";
            this.LblCardLink.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.LblCardLink_LinkClicked);
            // 
            // LblCreatedBy
            // 
            this.LblCreatedBy.AutoSize = true;
            this.LblCreatedBy.Location = new System.Drawing.Point(12, 46);
            this.LblCreatedBy.Name = "LblCreatedBy";
            this.LblCreatedBy.Size = new System.Drawing.Size(128, 13);
            this.LblCreatedBy.TabIndex = 2;
            this.LblCreatedBy.Text = "Created By: Ian O\'Rourke";
            // 
            // LblVersion
            // 
            this.LblVersion.AutoSize = true;
            this.LblVersion.Location = new System.Drawing.Point(12, 59);
            this.LblVersion.Name = "LblVersion";
            this.LblVersion.Size = new System.Drawing.Size(81, 13);
            this.LblVersion.TabIndex = 3;
            this.LblVersion.Text = "Version: 0.0.0.0";
            // 
            // LblName
            // 
            this.LblName.AutoSize = true;
            this.LblName.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LblName.Location = new System.Drawing.Point(11, 9);
            this.LblName.Name = "LblName";
            this.LblName.Size = new System.Drawing.Size(135, 20);
            this.LblName.TabIndex = 4;
            this.LblName.Text = "Card Game Client";
            // 
            // btnClose
            // 
            this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClose.Location = new System.Drawing.Point(177, 128);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(75, 23);
            this.btnClose.TabIndex = 5;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.BtnClose_Click);
            // 
            // AboutForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(264, 163);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.LblName);
            this.Controls.Add(this.LblVersion);
            this.Controls.Add(this.LblCreatedBy);
            this.Controls.Add(this.LblCardLink);
            this.Controls.Add(this.LblAttribution);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "AboutForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "About";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label LblAttribution;
        private System.Windows.Forms.LinkLabel LblCardLink;
        private System.Windows.Forms.Label LblCreatedBy;
        private System.Windows.Forms.Label LblVersion;
        private System.Windows.Forms.Label LblName;
        private System.Windows.Forms.Button btnClose;
    }
}