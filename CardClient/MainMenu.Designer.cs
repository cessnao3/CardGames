namespace CardClient
{
    partial class MainMenu
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainMenu));
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.lobbiesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.refreshToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newLobbyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newEuchreLobbyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tmrServerTick = new System.Windows.Forms.Timer(this.components);
            this.ListLobbies = new System.Windows.Forms.ListView();
            this.ListGames = new System.Windows.Forms.ListView();
            this.tmrLobbyCheck = new System.Windows.Forms.Timer(this.components);
            this.grpLobbies = new System.Windows.Forms.GroupBox();
            this.grpGames = new System.Windows.Forms.GroupBox();
            this.menuStrip1.SuspendLayout();
            this.grpLobbies.SuspendLayout();
            this.grpGames.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.lobbiesToolStripMenuItem,
            this.aboutToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(783, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(93, 22);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // lobbiesToolStripMenuItem
            // 
            this.lobbiesToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.refreshToolStripMenuItem,
            this.newLobbyToolStripMenuItem,
            this.newEuchreLobbyToolStripMenuItem});
            this.lobbiesToolStripMenuItem.Name = "lobbiesToolStripMenuItem";
            this.lobbiesToolStripMenuItem.Size = new System.Drawing.Size(60, 20);
            this.lobbiesToolStripMenuItem.Text = "Lobbies";
            // 
            // refreshToolStripMenuItem
            // 
            this.refreshToolStripMenuItem.Name = "refreshToolStripMenuItem";
            this.refreshToolStripMenuItem.Size = new System.Drawing.Size(173, 22);
            this.refreshToolStripMenuItem.Text = "Refresh";
            this.refreshToolStripMenuItem.Click += new System.EventHandler(this.refreshToolStripMenuItem_Click);
            // 
            // newLobbyToolStripMenuItem
            // 
            this.newLobbyToolStripMenuItem.Name = "newLobbyToolStripMenuItem";
            this.newLobbyToolStripMenuItem.Size = new System.Drawing.Size(173, 22);
            this.newLobbyToolStripMenuItem.Text = "New Hearts Lobby";
            this.newLobbyToolStripMenuItem.Click += new System.EventHandler(this.newLobbyToolStripMenuItem_Click);
            // 
            // newEuchreLobbyToolStripMenuItem
            // 
            this.newEuchreLobbyToolStripMenuItem.Name = "newEuchreLobbyToolStripMenuItem";
            this.newEuchreLobbyToolStripMenuItem.Size = new System.Drawing.Size(173, 22);
            this.newEuchreLobbyToolStripMenuItem.Text = "New Euchre Lobby";
            this.newEuchreLobbyToolStripMenuItem.Click += new System.EventHandler(this.newEuchreLobbyToolStripMenuItem_Click);
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(52, 20);
            this.aboutToolStripMenuItem.Text = "About";
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
            // 
            // tmrServerTick
            // 
            this.tmrServerTick.Tick += new System.EventHandler(this.tmrServerTick_Tick);
            // 
            // ListLobbies
            // 
            this.ListLobbies.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ListLobbies.HideSelection = false;
            this.ListLobbies.Location = new System.Drawing.Point(6, 19);
            this.ListLobbies.MultiSelect = false;
            this.ListLobbies.Name = "ListLobbies";
            this.ListLobbies.Size = new System.Drawing.Size(300, 368);
            this.ListLobbies.TabIndex = 1;
            this.ListLobbies.UseCompatibleStateImageBehavior = false;
            this.ListLobbies.DoubleClick += new System.EventHandler(this.ListLobbies_DoubleClick);
            // 
            // ListGames
            // 
            this.ListGames.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ListGames.HideSelection = false;
            this.ListGames.Location = new System.Drawing.Point(6, 19);
            this.ListGames.MultiSelect = false;
            this.ListGames.Name = "ListGames";
            this.ListGames.Size = new System.Drawing.Size(428, 368);
            this.ListGames.TabIndex = 2;
            this.ListGames.UseCompatibleStateImageBehavior = false;
            this.ListGames.DoubleClick += new System.EventHandler(this.ListGames_DoubleClick);
            // 
            // tmrLobbyCheck
            // 
            this.tmrLobbyCheck.Interval = 10000;
            this.tmrLobbyCheck.Tick += new System.EventHandler(this.tmrLobbyCheck_Tick);
            // 
            // grpLobbies
            // 
            this.grpLobbies.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.grpLobbies.Controls.Add(this.ListLobbies);
            this.grpLobbies.Location = new System.Drawing.Point(12, 27);
            this.grpLobbies.Name = "grpLobbies";
            this.grpLobbies.Size = new System.Drawing.Size(312, 393);
            this.grpLobbies.TabIndex = 3;
            this.grpLobbies.TabStop = false;
            this.grpLobbies.Text = "Lobbies";
            // 
            // grpGames
            // 
            this.grpGames.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpGames.Controls.Add(this.ListGames);
            this.grpGames.Location = new System.Drawing.Point(331, 27);
            this.grpGames.Name = "grpGames";
            this.grpGames.Size = new System.Drawing.Size(440, 393);
            this.grpGames.TabIndex = 4;
            this.grpGames.TabStop = false;
            this.grpGames.Text = "Active Games";
            // 
            // MainMenu
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(783, 432);
            this.Controls.Add(this.grpGames);
            this.Controls.Add(this.grpLobbies);
            this.Controls.Add(this.menuStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.MinimumSize = new System.Drawing.Size(640, 340);
            this.Name = "MainMenu";
            this.Text = "Main Menu";
            this.Load += new System.EventHandler(this.MainMenu_Load);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.grpLobbies.ResumeLayout(false);
            this.grpGames.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.Timer tmrServerTick;
        private System.Windows.Forms.ToolStripMenuItem lobbiesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem newLobbyToolStripMenuItem;
        private System.Windows.Forms.ListView ListLobbies;
        private System.Windows.Forms.ListView ListGames;
        private System.Windows.Forms.Timer tmrLobbyCheck;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem newEuchreLobbyToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem refreshToolStripMenuItem;
        private System.Windows.Forms.GroupBox grpLobbies;
        private System.Windows.Forms.GroupBox grpGames;
    }
}

