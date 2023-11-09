using System;
using System.Collections.Generic;
using System.Text;

namespace CardGameLibrary.Messages
{
    /// <summary>
    /// Class to facilitate communication within a game
    /// </summary>
    public class MsgGamePlay : MsgBase
    {
        /// <summary>
        /// Defines the game ID to use
        /// </summary>
        public int GameID { get; set; }

        /// <summary>
        /// Sets up the player requesting the action
        /// </summary>
        public GameParameters.GamePlayer Player { get; set; }

        /// <summary>
        /// Defines the card to play
        /// </summary>
        public Cards.Card Card { get; set; }

        /// <summary>
        /// Constructor to set the server response
        /// </summary>
        public MsgGamePlay() : base(MessageTypeID.GamePlay)
        {
            // Initialize an empty game ID
            GameID = -1;
        }


        /// <summary>
        /// Checks whether message parameters are valid
        /// </summary>
        /// <returns>true if valid</returns>
        public override bool CheckMessage()
        {
            return
                GameID >= 0 &&
                Card != null &&
                Player != null &&
                MessageType == MessageTypeID.GamePlay;
        }
    }
}
