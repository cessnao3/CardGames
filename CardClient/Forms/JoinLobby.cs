using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CardGameLibrary.GameParameters;
using CardGameLibrary.Messages;

namespace CardClient.Forms
{
    public partial class JoinLobby : Form
    {
        int LobbyID { get; }

        public JoinLobby(int lobby_id)
        {
            InitializeComponent();

            LobbyID = lobby_id;

            StatusUpdateTick(null, EventArgs.Empty);
        }

        public void UpdateStatus(MsgLobbyStatus status)
        {
            if (LobbyID != status.GameID) return;

            string[] dirStrings = new string[]
            {
                "North",
                "East",
                "South",
                "West"
            };

            Button[] buttons = new Button[]
            {
                BtnNorth,
                BtnEast,
                BtnSouth,
                BtnWest
            };

            bool player_is_in = false;
            GamePlayer? player = Network.GameComms.GetPlayer();

            foreach (var otherPlayer in status.Players.Take(buttons.Length))
            {
                if (player?.Equals(otherPlayer) ?? throw new NullReferenceException(nameof(player)))
                {
                    player_is_in = true;
                }
            }

            foreach (var (otherPlayer, btn, dir) in Enumerable.Zip(status.Players, buttons, dirStrings))
            { 
                if (otherPlayer == null)
                {
                    btn.Text = dir;
                    btn.Enabled = !player_is_in;
                }
                else
                {
                    btn.Enabled = false;
                    btn.Text = $"{dir[..1]} {otherPlayer.CapitalizedName[..Math.Min(3, otherPlayer.CapitalizedName.Length)]}";
                }
            }

            BtnLeave.Enabled = player_is_in;
        }

        private void StatusUpdateTick(object? sender, EventArgs e)
        {
            Network.GameComms.SendMessage(new MsgClientRequest(MsgClientRequest.RequestType.LobbyStatus, gameId: LobbyID));
        }

        private void SendJoinRequest(LobbyPositions pos)
        {
            Network.GameComms.SendMessage(new MsgClientRequest(MsgClientRequest.RequestType.JoinLobby, gameId: LobbyID, data: (int)pos));
        }

        private void SendLeaveRequest()
        {
            Network.GameComms.SendMessage(new MsgClientRequest(MsgClientRequest.RequestType.LeaveLobby, gameId: LobbyID));
        }

        private void BtnClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void BtnLeave_Click(object sender, EventArgs e)
        {
            SendLeaveRequest();
        }

        private void BtnDirection_Click(object sender, EventArgs e)
        {
            IReadOnlyDictionary<Button, LobbyPositions> dirs = new Dictionary<Button, LobbyPositions>()
            {
                { BtnNorth, LobbyPositions.North },
                { BtnSouth, LobbyPositions.South },
                { BtnEast, LobbyPositions.East },
                { BtnWest, LobbyPositions.West },
            };

            if (sender is Button btn && dirs.TryGetValue(btn, out var direction))
            {
                SendJoinRequest(direction);
            }
        }
    }
}
