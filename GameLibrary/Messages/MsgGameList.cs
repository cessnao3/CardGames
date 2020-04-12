using System;
using System.Collections.Generic;
using System.Text;

namespace GameLibrary.Messages
{
    /// <summary>
    /// Provides information on available game parameters
    /// </summary>
    public class MsgGameList : MsgBase
    {
        /// <summary>
        /// Defines the list of lobbies, by ID, that may be joined
        /// </summary>
        public List<int> lobbies;

        /// <summary>
        /// Defines the list of games, by ID, that may be played
        /// </summary>
        public List<int> games;

        /// <summary>
        /// Defines the game list message/response
        /// </summary>
        public MsgGameList() : base(MessageType.GameList)
        {
            // Empty Constructor
        }

        /// <summary>
        /// Determines if the message contains a valid message
        /// </summary>
        /// <returns>True if valid</returns>
        public override bool CheckMessage()
        {
            return
                msg_type == MessageType.GameList &&
                lobbies != null &&
                games != null;
        }
    }
}
