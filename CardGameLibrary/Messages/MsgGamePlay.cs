using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

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
        [JsonInclude]
        public int GameID { get; private set; }

        /// <summary>
        /// Sets up the player requesting the action
        /// </summary>
        [JsonInclude]
        public GameParameters.GamePlayer Player { get; private set; }

        /// <summary>
        /// Defines the card to play
        /// </summary>
        [JsonInclude]
        public Cards.Card Card { get; private set; }

        /// <summary>
        /// Constructor to set the server response
        /// </summary>
        [JsonConstructor]
        public MsgGamePlay() : base(MessageTypeID.GamePlay)
        {
            // Initialize an empty game ID
            GameID = -1;
            Player = new(string.Empty);
            Card = Cards.Card.CreateSpecialCard(-1);
        }

        /// <summary>
        /// Constructor to set the server response
        /// </summary>
        public MsgGamePlay(int gameId, GameParameters.GamePlayer player, Cards.Card card) : base(MessageTypeID.GamePlay)
        {
            // Initialize an empty game ID
            GameID = gameId;
            Player = player;
            Card = card;
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
