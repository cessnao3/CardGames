using System;
using System.Collections.Generic;
using System.Text;
using GameLibrary.Games;

namespace GameLibrary.Messages
{
    /// <summary>
    /// Defines a message to provide lobby status to clients
    /// </summary>
    public class MsgLobbyStatus : MsgBase
    {
        /// <summary>
        /// Defines the ID of the game
        /// </summary>
        public int game_id = -1;

        /// <summary>
        /// The players to read in
        /// </summary>
        public List<GamePlayer> players;

        /// <summary>
        /// The game type for the current lobby
        /// </summary>
        public GameTypes game_type = GameTypes.Invalid;

        /// <summary>
        /// Determines if the lobby is ready
        /// </summary>
        public bool lobby_ready = false;

        /// <summary>
        /// Constructor to setup the lobby status message
        /// </summary>
        public MsgLobbyStatus() : base(MessageType.LobbyStatus)
        {
            // Empty Constructor
        }

        /// <summary>
        /// Checks if the provided message has been marked as invalid
        /// </summary>
        /// <returns></returns>
        public override bool CheckMessage()
        {
            return
                msg_type == MessageType.LobbyStatus &&
                game_id >= 0 &&
                players != null &&
                game_type != GameTypes.Invalid;
        }
    }
}
