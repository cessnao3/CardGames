using System;
using System.Collections.Generic;
using System.Text;

namespace GameLibrary.Messages
{
    /// <summary>
    /// Provides the game status for Hearts
    /// </summary>
    public class MsgGameStatus : MsgBase
    {
        /// <summary>
        /// Defines the ID of the game
        /// </summary>
        public int game_id = -1;

        /// <summary>
        /// The players to read in
        /// </summary>
        public List<Games.GamePlayer> players;

        /// <summary>
        /// The hands for each player
        /// </summary>
        public List<Cards.Hand> hands;

        /// <summary>
        /// The center pool of cards for each player
        /// </summary>
        public List<Cards.Card> center_pool;

        /// <summary>
        /// The current score for each player
        /// </summary>
        public List<int> scores;

        /// <summary>
        /// Defines the current game status for different games
        /// </summary>
        public string current_game_status;

        /// <summary>
        /// The current player needing to play
        /// </summary>
        public int current_player;

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
                current_game_status != null &&
                game_id >= 0 &&
                scores != null;
        }
    }
}
