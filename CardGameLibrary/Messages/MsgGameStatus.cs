using CardGameLibrary.Cards;
using CardGameLibrary.GameParameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;

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
        [JsonInclude]
        public int GameID { get; private set; }

        /// <summary>
        /// Defines the game type of the game
        /// </summary>
        [JsonInclude]
        public int GameType { get; private set; }

        /// <summary>
        /// The players to read in
        /// </summary>
        [JsonInclude]
        public IReadOnlyList<GamePlayer> Players { get; private set; }

        /// <summary>
        /// The hands for each player
        /// </summary>
        [JsonInclude]
        public IReadOnlyList<Hand> Hands { get; private set; }

        /// <summary>
        /// The center pool of cards for each player
        /// </summary>
        [JsonInclude]
        public IReadOnlyList<Card?> PlayedCardsByPlayer { get; private set; }

        /// <summary>
        /// The center cards that can be used for selecting trump or
        /// performing other similar actions
        /// </summary>
        [JsonInclude]
        public IReadOnlyList<Card> CenterActionCards { get; set; }

        /// <summary>
        /// The current score for each player
        /// </summary>
        [JsonInclude]
        public IReadOnlyList<int> Scores { get; private set; }

        /// <summary>
        /// Defines the current game status for different games
        /// </summary>
        [JsonInclude]
        public string CurrentGameStatus { get; set; }

        /// <summary>
        /// The current player needing to play
        /// </summary>
        [JsonInclude]
        public int CurrentPlayer { get; private set; }

        /// <summary>
        /// Default hearts game status constructor
        /// </summary>
        [JsonConstructor]
        public MsgGameStatus() : base(MessageTypeID.GameStatus)
        {
            // Initialize the game type and ID
            GameID = -1;
            GameType = -1;

            Players = Array.Empty<GamePlayer>();
            Hands = Array.Empty<Hand>();
            CurrentGameStatus = string.Empty;
            CurrentPlayer = -1;
            Scores = Array.Empty<int>();
            PlayedCardsByPlayer = Array.Empty<Card?>();
            CenterActionCards = Array.Empty<Card>();
        }

        /// <summary>
        /// Default hearts game status constructor
        /// </summary>
        public MsgGameStatus(
            int gameId,
            int gameType,
            IEnumerable<GamePlayer> players,
            IEnumerable<Hand> hands,
            string status,
            int currentPlayer,
            IEnumerable<int> scores,
            IEnumerable<Card?> playedCards,
            IEnumerable<Card> centerCards) : base(MessageTypeID.GameStatus)
        {
            // Initialize the game type and ID
            GameID = gameId;
            GameType = gameType;
            Players = players.ToArray();
            Hands = hands.ToArray();
            CurrentGameStatus = status;
            CurrentPlayer = currentPlayer;
            Scores = scores.ToArray();
            PlayedCardsByPlayer = playedCards.ToArray();
            CenterActionCards = centerCards.ToArray();
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
