using System;
using System.Collections.Generic;
using System.Text;
using CardGameLibrary.GameParameters;

namespace CardGameLibrary.Messages
{
    /// <summary>
    /// Defines a message to provide lobby status to clients
    /// </summary>
    public class MsgLobbyStatus : MsgBase
    {
        /// <summary>
        /// Defines the ID of the game
        /// </summary>
        public int GameID { get; set; }

        /// <summary>
        /// The players to read in
        /// </summary>
        public List<GamePlayer> Players { get; set; }

        /// <summary>
        /// The game type for the current lobby
        /// </summary>
        public GameTypes GameType { get; set; }

        /// <summary>
        /// Determines if the lobby is ready
        /// </summary>
        public bool LobbyReady { get; set; }

        /// <summary>
        /// Constructor to setup the lobby status message
        /// </summary>
        public MsgLobbyStatus() : base(MessageTypeID.LobbyStatus)
        {
            // Initilize Parameters
            GameID = -1;
            GameType = GameTypes.Invalid;
            LobbyReady = false;
        }

        /// <summary>
        /// Checks if the provided message has been marked as invalid
        /// </summary>
        /// <returns></returns>
        public override bool CheckMessage()
        {
            return
                MessageType == MessageTypeID.LobbyStatus &&
                GameID >= 0 &&
                Players != null &&
                GameType != GameTypes.Invalid;
        }
    }
}
