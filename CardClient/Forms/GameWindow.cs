using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CardGameLibrary.Messages;

namespace CardClient.Forms
{
    public partial class GameWindow : Form
    {
        int GameID { get; }

        public GameWindow(int gameId)
        {
            InitializeComponent();

            GameID = gameId;
            gameScreen.SetGameID(GameID);

            GameUpdateTick(null, EventArgs.Empty);

            Text = $"Game - {Network.GameComms.GetPlayer()?.CapitalizedName ?? "<<UNKNOWN>>"}";
        }

        private void GameScreenClick(object sender, EventArgs e)
        {
            gameScreen.OnGameCardClick(sender, e);
        }

        private void GameUpdateTick(object? sender, EventArgs e)
        {
            Network.GameComms.SendMessage(new MsgClientRequest(MsgClientRequest.RequestType.GameStatus, GameID));
        }

        public void UpdateGame(MsgGameStatus status)
        {
            // Check that the ID values
            if (status.GameID != GameID) return;

            // Update the status window
            gameScreen.UpdateFromStatus(status);
            if (status.CurrentGameStatus != null && status.CurrentGameStatus.Length > 0)
            {
                toolStripStatus.Text = "Status: " + status.CurrentGameStatus;
            }
            else
            {
                toolStripStatus.Text = "No Game Status";
            }
        }

        private void ExitMenuItemPressed(object sender, EventArgs e)
        {
            Close();
        }

        private void AboutMenuItemPressed(object sender, EventArgs e)
        {
            (new AboutForm()).ShowDialog(this);
        }

        private void RefreshMenuItemPressed(object sender, EventArgs e)
        {
            GameUpdateTick(null, EventArgs.Empty);
        }
    }
}
