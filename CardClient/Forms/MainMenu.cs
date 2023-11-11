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
using CardGameLibrary.GameParameters;

namespace CardClient.Forms
{
    public partial class MainMenu : Form
    {
        class CommonEntry
        {
            public int ID { get; private set; }
            public GameTypes GameType { get; private set; }

            public CommonEntry(int id, GameTypes game_type)
            {
                ID = id;
                GameType = game_type;
            }

            public override string ToString()
            {
                return $"{GameType} ID {ID}";
            }
        }

        GameWindow? GameWindow { get; set; } = null;

        JoinLobby? LobbyWindow { get; set; } = null;

        public MainMenu()
        {
            InitializeComponent();
        }

        private void MainMenu_Load(object sender, EventArgs e)
        {
            LoginWindow lw = new();
            lw.ShowDialog(this);

            if (lw.DialogResult != DialogResult.OK)
            {
                Close();
            }
            else
            {
                // Enable the tick timers
                tmrLobbyCheck.Enabled = true;
                tmrServerTick.Enabled = true;

                // Check for lobby parameters
                LobbbyCheckTick(null, EventArgs.Empty);
            }
        }

        private void TmrServerTick_Tick(object sender, EventArgs e)
        {
            // Read input messages
            int read_msg = 0;
            while (read_msg < 10)
            {
                MsgBase? msg = Network.GameComms.ReceiveMessage();
                if (msg == null) break;

                Console.WriteLine("Received " + msg.GetType().ToString());

                if (msg is MsgGameStatus gameStatus)
                {
                    GameWindow?.UpdateGame(gameStatus);
                }
                else if (msg is MsgGameList gameList)
                {
                    ListGames.Items.Clear();
                    foreach (MsgGameList.ListItem i in gameList.Games)
                    {
                        CommonEntry gi = new(id: i.GameIDValue, game_type: (GameTypes)i.GameType);
                        ListViewItem lvi = new(gi.ToString())
                        {
                            Tag = gi
                        };
                        ListGames.Items.Add(lvi);
                    }

                    ListLobbies.Items.Clear();
                    foreach (MsgGameList.ListItem i in gameList.Lobbies)
                    {
                        CommonEntry li = new(id: i.GameIDValue, game_type: (GameTypes)i.GameType);
                        ListViewItem lvi = new(li.ToString())
                        {
                            Tag = li
                        };
                        ListLobbies.Items.Add(lvi);
                    }
                }
                else if (msg is MsgLobbyStatus lobbyStatus)
                {
                    LobbyWindow?.UpdateStatus(lobbyStatus);
                }

                read_msg += 1;
            }

            // Check for server failure
            if (Network.GameComms.HasFailed())
            {
#if DEBUG
                Application.Exit();
#else
                Application.Restart();
#endif
            }
        }

        private void ExitClicked(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void RefreshClicked(object sender, EventArgs e)
        {
            // Request a lobby list
            LobbbyCheckTick(null, EventArgs.Empty);
        }

        private void NewLobbyClicked(object sender, EventArgs e)
        {
            GameTypes type;

            if (sender == newLobbyHeartsToolStripMenuItem)
            {
                type = GameTypes.Hearts;
            }
            else if (sender == newLobbyEuchreToolStripMenuItem)
            {
                type = GameTypes.Euchre;
            }
            else
            {
                return;
            }

            RequestLobbyForGame(type);
        }

        private void RequestLobbyForGame(GameTypes type)
        {
            if (type != GameTypes.Invalid)
            {
                // Create the game lobby
                Network.GameComms.SendMessage(new MsgClientRequest(MsgClientRequest.RequestType.NewLobby, data: (int)type));
            }

            // Request a lobby list
            LobbbyCheckTick(null, EventArgs.Empty);
        }

        private void LobbbyCheckTick(object? sender, EventArgs e)
        {
            // Send the message to request lobby status
            Network.GameComms.SendMessage(new MsgClientRequest(MsgClientRequest.RequestType.AvailableGames));
        }

        private void ListLobbies_DoubleClick(object sender, EventArgs e)
        {
            CommonEntry? entry = null;

            foreach (ListViewItem lvi in ListLobbies.SelectedItems)
            {
                entry = (CommonEntry)lvi.Tag;
            }

            if (entry != null)
            {
                // Checkout Lobby Parameters
                LobbyWindow = new JoinLobby(entry.ID);
                LobbyWindow.ShowDialog();
                LobbyWindow = null;

                // Recheck game parameters
                LobbbyCheckTick(null, EventArgs.Empty);
            }
        }

        private void ListGames_DoubleClick(object sender, EventArgs e)
        {
            CommonEntry? entry = null;

            foreach (ListViewItem lvi in ListGames.SelectedItems)
            {
                entry = (CommonEntry)lvi.Tag;
            }

            if (entry != null)
            {
                // Hide the background form
                Hide();
                tmrLobbyCheck.Enabled = false;

                // Checkout Lobby Parameters
                GameWindow = new GameWindow(entry.ID);
                GameWindow.ShowDialog(this);
                GameWindow = null;

                // Re-show the main form
                try
                {
                    tmrLobbyCheck.Enabled = true;
                    Show();
                }
                catch (ObjectDisposedException)
                {
                    // Do nothing, this object has already been disposed
                }
            }
        }

        private void AboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            (new AboutForm()).ShowDialog(this);
        }
    }
}
