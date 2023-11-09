using CardGameLibrary.Cards;
using System;
using System.Collections.Generic;
using System.Text;

namespace CardGameLibrary.Messages
{
    /// <summary>
    /// Provides the game status for Hearts
    /// </summary>
    public class MsgGameStatus : MsgBase
    {
        /// <summary>
        /// Defines the ID of the game
        /// </summary>
        public int GameID { get; set; }

        /// <summary>
        /// Defines the game type of the game
        /// </summary>
        public int GameType { get; set; }

        /// <summary>
        /// The players to read in
        /// </summary>
        public List<GameParameters.GamePlayer> Players { get; set; }

        /// <summary>
        /// The hands for each player
        /// </summary>
        public List<Hand> Hands { get; set; }

        /// <summary>
        /// The center pool of cards for each player
        /// </summary>
        public List<Card> PlayedCardsByPlayer { get; set; }

        /// <summary>
        /// The center cards that can be used for selecting trump or
        /// performing other similar actions
        /// </summary>
        public List<Card> CenterActionCards { get; set; }

        /// <summary>
        /// The current score for each player
        /// </summary>
        public List<int> Scores { get; set; }

        /// <summary>
        /// Defines the current game status for different games
        /// </summary>
        public string CurrentGameStatus { get; set; }

        /// <summary>
        /// The current player needing to play
        /// </summary>
        public int CurrentPlayer { get; set; }

        /// <summary>
        /// Default hearts game status constructor
        /// </summary>
        public MsgGameStatus() : base(MessageTypeID.GameStatus)
        {
            // Initialize the game type and ID
            GameID = -1;
            GameType = -1;
        }

        /// <summary>
        /// Checks that the hearts game status is valid
        /// </summary>
        /// <returns>True if valid</returns>
        public override bool CheckMessage()
        {
            return
                MessageType == MessageTypeID.GameStatus &&
                Players != null &&
                Hands != null &&
                CurrentGameStatus != null &&
                GameID >= 0 &&
                Scores != null &&
                GameType > 0;
        }
    }
}
