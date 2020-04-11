using System;
using System.Collections.Generic;
using System.Text;

namespace GameLibrary.Messages
{
    /// <summary>
    /// Provides the game status for Hearts
    /// </summary>
    class MsgGameStatus : MsgBase
    {
        /// <summary>
        /// The players to read in
        /// </summary>
        List<Games.GamePlayer> players;

        /// <summary>
        /// The hands for each player
        /// </summary>
        List<Cards.Hand> hands;

        /// <summary>
        /// Defines the current game status for different games
        /// </summary>
        string current_game_status;

        /// <summary>
        /// The current player needing to play
        /// </summary>
        int current_player;

        /// <summary>
        /// Default hearts game status constructor
        /// </summary>
        public MsgGameStatus() : base(MessageType.GameStatus)
        {
            // Empty Constructor
        }

        /// <summary>
        /// Checks that the hearts game status is valid
        /// </summary>
        /// <returns>True if valid</returns>
        public override bool CheckMessage()
        {
            return
                msg_type == MessageType.GameStatus &&
                players != null &&
                hands != null &&
                current_game_status != null;

        }
    }
}
