using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
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
        [JsonInclude]
        public int GameID { get; private set; }

        /// <summary>
        /// The players to read in
        /// </summary>
        [JsonInclude]
        public IReadOnlyList<GamePlayer?> Players { get; private set; }

        /// <summary>
        /// The game type for the current lobby
        /// </summary>
        [JsonInclude]
        public GameTypes GameType { get; private set; }

        /// <summary>
        /// Determines if the lobby is ready
        /// </summary>
        [JsonInclude]
        public bool LobbyReady { get; private set; }

        /// <summary>
        /// Constructor to setup the lobby status message
        /// </summary>
        [JsonConstructor]
        public MsgLobbyStatus() : base(MessageTypeID.LobbyStatus)
        {
            // Initilize Parameters
            GameID = -1;
            GameType = GameTypes.Invalid;
            LobbyReady = false;
            Players = Array.Empty<GamePlayer?>();
        }

        /// <summary>
        /// Constructor to setup the lobby status message
        /// </summary>
        public MsgLobbyStatus(int gameId, GameTypes gameType, bool lobbyReady, IEnumerable<GamePlayer?> players) : base(MessageTypeID.LobbyStatus)
        {
            // Initilize Parameters
            GameID = gameId;
            GameType = gameType;
            LobbyReady = lobbyReady;
            Players = players.ToList();
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
