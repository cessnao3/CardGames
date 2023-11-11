namespace CardClient.Forms
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
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainMenu));
            menuStrip1 = new System.Windows.Forms.MenuStrip();
            fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            lobbiesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            refreshToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            newLobbyHeartsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            newLobbyEuchreToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            tmrServerTick = new System.Windows.Forms.Timer(components);
            ListLobbies = new System.Windows.Forms.ListView();
            ListGames = new System.Windows.Forms.ListView();
            tmrLobbyCheck = new System.Windows.Forms.Timer(components);
            grpLobbies = new System.Windows.Forms.GroupBox();
            grpGames = new System.Windows.Forms.GroupBox();
            menuStrip1.SuspendLayout();
            grpLobbies.SuspendLayout();
            grpGames.SuspendLayout();
            SuspendLayout();
            // 
            // menuStrip1
            // 
            menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { fileToolStripMenuItem, lobbiesToolStripMenuItem, aboutToolStripMenuItem });
            menuStrip1.Location = new System.Drawing.Point(0, 0);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Padding = new System.Windows.Forms.Padding(7, 2, 0, 2);
            menuStrip1.Size = new System.Drawing.Size(913, 24);
            menuStrip1.TabIndex = 0;
            menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { exitToolStripMenuItem });
            fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            fileToolStripMenuItem.Text = "File";
            // 
            // exitToolStripMenuItem
            // 
            exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            exitToolStripMenuItem.Size = new System.Drawing.Size(93, 22);
            exitToolStripMenuItem.Text = "Exit";
            exitToolStripMenuItem.Click += ExitClicked;
            // 
            // lobbiesToolStripMenuItem
            // 
            lobbiesToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { refreshToolStripMenuItem, newLobbyHeartsToolStripMenuItem, newLobbyEuchreToolStripMenuItem });
            lobbiesToolStripMenuItem.Name = "lobbiesToolStripMenuItem";
            lobbiesToolStripMenuItem.Size = new System.Drawing.Size(60, 20);
            lobbiesToolStripMenuItem.Text = "Lobbies";
            // 
            // refreshToolStripMenuItem
            // 
            refreshToolStripMenuItem.Name = "refreshToolStripMenuItem";
            refreshToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            refreshToolStripMenuItem.Text = "Refresh";
            refreshToolStripMenuItem.Click += RefreshClicked;
            // 
            // newLobbyHeartsToolStripMenuItem
            // 
            newLobbyHeartsToolStripMenuItem.Name = "newLobbyHeartsToolStripMenuItem";
            newLobbyHeartsToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            newLobbyHeartsToolStripMenuItem.Text = "New Hearts Lobby";
            newLobbyHeartsToolStripMenuItem.Click += NewLobbyClicked;
            // 
            // newEuchreLobbyToolStripMenuItem
            // 
            newLobbyEuchreToolStripMenuItem.Name = "newEuchreLobbyToolStripMenuItem";
            newLobbyEuchreToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            newLobbyEuchreToolStripMenuItem.Text = "New Euchre Lobby";
            newLobbyEuchreToolStripMenuItem.Click += NewLobbyClicked;
            // 
            // aboutToolStripMenuItem
            // 
            aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            aboutToolStripMenuItem.Size = new System.Drawing.Size(52, 20);
            aboutToolStripMenuItem.Text = "About";
            aboutToolStripMenuItem.Click += AboutToolStripMenuItem_Click;
            // 
            // tmrServerTick
            // 
            tmrServerTick.Tick += TmrServerTick_Tick;
            // 
            // ListLobbies
            // 
            ListLobbies.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            ListLobbies.Location = new System.Drawing.Point(7, 22);
            ListLobbies.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            ListLobbies.MultiSelect = false;
            ListLobbies.Name = "ListLobbies";
            ListLobbies.Size = new System.Drawing.Size(349, 424);
            ListLobbies.TabIndex = 1;
            ListLobbies.UseCompatibleStateImageBehavior = false;
            ListLobbies.DoubleClick += ListLobbies_DoubleClick;
            // 
            // ListGames
            // 
            ListGames.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            ListGames.Location = new System.Drawing.Point(7, 22);
            ListGames.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            ListGames.MultiSelect = false;
            ListGames.Name = "ListGames";
            ListGames.Size = new System.Drawing.Size(499, 424);
            ListGames.TabIndex = 2;
            ListGames.UseCompatibleStateImageBehavior = false;
            ListGames.DoubleClick += ListGames_DoubleClick;
            // 
            // tmrLobbyCheck
            // 
            tmrLobbyCheck.Interval = 10000;
            tmrLobbyCheck.Tick += LobbbyCheckTick;
            // 
            // grpLobbies
            // 
            grpLobbies.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            grpLobbies.Controls.Add(ListLobbies);
            grpLobbies.Location = new System.Drawing.Point(14, 31);
            grpLobbies.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            grpLobbies.Name = "grpLobbies";
            grpLobbies.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            grpLobbies.Size = new System.Drawing.Size(364, 453);
            grpLobbies.TabIndex = 3;
            grpLobbies.TabStop = false;
            grpLobbies.Text = "Lobbies";
            // 
            // grpGames
            // 
            grpGames.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            grpGames.Controls.Add(ListGames);
            grpGames.Location = new System.Drawing.Point(386, 31);
            grpGames.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            grpGames.Name = "grpGames";
            grpGames.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            grpGames.Size = new System.Drawing.Size(513, 453);
            grpGames.TabIndex = 4;
            grpGames.TabStop = false;
            grpGames.Text = "Active Games";
            // 
            // MainMenu
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(913, 498);
            Controls.Add(grpGames);
            Controls.Add(grpLobbies);
            Controls.Add(menuStrip1);
            Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
            MainMenuStrip = menuStrip1;
            Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            MinimumSize = new System.Drawing.Size(744, 386);
            Name = "MainMenu";
            Text = "Main Menu";
            Load += MainMenu_Load;
            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
            grpLobbies.ResumeLayout(false);
            grpGames.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.Timer tmrServerTick;
        private System.Windows.Forms.ToolStripMenuItem lobbiesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem newLobbyHeartsToolStripMenuItem;
        private System.Windows.Forms.ListView ListLobbies;
        private System.Windows.Forms.ListView ListGames;
        private System.Windows.Forms.Timer tmrLobbyCheck;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem newLobbyEuchreToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem refreshToolStripMenuItem;
        private System.Windows.Forms.GroupBox grpLobbies;
        private System.Windows.Forms.GroupBox grpGames;
    }
}

