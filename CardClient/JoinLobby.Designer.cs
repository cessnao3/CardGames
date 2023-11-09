namespace CardClient
{
    partial class JoinLobby
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
            this.components = new System.ComponentModel.Container();
            this.BtnNorth = new System.Windows.Forms.Button();
            this.BtnWest = new System.Windows.Forms.Button();
            this.BtnEast = new System.Windows.Forms.Button();
            this.BtnSouth = new System.Windows.Forms.Button();
            this.tmrStatusUpdate = new System.Windows.Forms.Timer(this.components);
            this.BtnLeave = new System.Windows.Forms.Button();
            this.BtnClose = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // BtnNorth
            // 
            this.BtnNorth.Enabled = false;
            this.BtnNorth.Location = new System.Drawing.Point(93, 12);
            this.BtnNorth.Name = "BtnNorth";
            this.BtnNorth.Size = new System.Drawing.Size(75, 23);
            this.BtnNorth.TabIndex = 0;
            this.BtnNorth.Text = "North";
            this.BtnNorth.UseVisualStyleBackColor = true;
            this.BtnNorth.Click += new System.EventHandler(this.BtnNorth_Click);
            // 
            // BtnWest
            // 
            this.BtnWest.Enabled = false;
            this.BtnWest.Location = new System.Drawing.Point(12, 41);
            this.BtnWest.Name = "BtnWest";
            this.BtnWest.Size = new System.Drawing.Size(75, 23);
            this.BtnWest.TabIndex = 1;
            this.BtnWest.Text = "West";
            this.BtnWest.UseVisualStyleBackColor = true;
            this.BtnWest.Click += new System.EventHandler(this.BtnWest_Click);
            // 
            // BtnEast
            // 
            this.BtnEast.Enabled = false;
            this.BtnEast.Location = new System.Drawing.Point(174, 41);
            this.BtnEast.Name = "BtnEast";
            this.BtnEast.Size = new System.Drawing.Size(75, 23);
            this.BtnEast.TabIndex = 2;
            this.BtnEast.Text = "East";
            this.BtnEast.UseVisualStyleBackColor = true;
            this.BtnEast.Click += new System.EventHandler(this.BtnEast_Click);
            // 
            // BtnSouth
            // 
            this.BtnSouth.Enabled = false;
            this.BtnSouth.Location = new System.Drawing.Point(93, 41);
            this.BtnSouth.Name = "BtnSouth";
            this.BtnSouth.Size = new System.Drawing.Size(75, 23);
            this.BtnSouth.TabIndex = 3;
            this.BtnSouth.Text = "South";
            this.BtnSouth.UseVisualStyleBackColor = true;
            this.BtnSouth.Click += new System.EventHandler(this.BtnSouth_Click);
            // 
            // tmrStatusUpdate
            // 
            this.tmrStatusUpdate.Enabled = true;
            this.tmrStatusUpdate.Interval = 10000;
            this.tmrStatusUpdate.Tick += new System.EventHandler(this.tmrStatusUpdate_Tick);
            // 
            // BtnLeave
            // 
            this.BtnLeave.Enabled = false;
            this.BtnLeave.Location = new System.Drawing.Point(12, 70);
            this.BtnLeave.Name = "BtnLeave";
            this.BtnLeave.Size = new System.Drawing.Size(75, 23);
            this.BtnLeave.TabIndex = 4;
            this.BtnLeave.Text = "Leave";
            this.BtnLeave.UseVisualStyleBackColor = true;
            this.BtnLeave.Click += new System.EventHandler(this.BtnLeave_Click);
            // 
            // BtnClose
            // 
            this.BtnClose.Location = new System.Drawing.Point(174, 70);
            this.BtnClose.Name = "BtnClose";
            this.BtnClose.Size = new System.Drawing.Size(75, 23);
            this.BtnClose.TabIndex = 5;
            this.BtnClose.Text = "Close";
            this.BtnClose.UseVisualStyleBackColor = true;
            this.BtnClose.Click += new System.EventHandler(this.BtnClose_Click);
            // 
            // JoinLobby
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(261, 102);
            this.Controls.Add(this.BtnClose);
            this.Controls.Add(this.BtnLeave);
            this.Controls.Add(this.BtnSouth);
            this.Controls.Add(this.BtnEast);
            this.Controls.Add(this.BtnWest);
            this.Controls.Add(this.BtnNorth);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "JoinLobby";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Join Lobby";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button BtnNorth;
        private System.Windows.Forms.Button BtnWest;
        private System.Windows.Forms.Button BtnEast;
        private System.Windows.Forms.Button BtnSouth;
        private System.Windows.Forms.Timer tmrStatusUpdate;
        private System.Windows.Forms.Button BtnLeave;
        private System.Windows.Forms.Button BtnClose;
    }
}