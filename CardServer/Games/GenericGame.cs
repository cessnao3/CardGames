using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CardGameLibrary.Cards;
using CardGameLibrary.GameParameters;
using CardGameLibrary.Messages;

namespace CardServer.Games
{
    /// <summary>
    /// Provides details for a generic game type
    /// </summary>
    public abstract class GenericGame
    {
        /// <summary>
        /// Stores the game ID
        /// </summary>
        public int GameID { get; protected set; }

        /// <summary>
        /// Provides the players for the given game
        /// </summary>
        public GamePlayer[] Players { get; protected set; }

        /// <summary>
        /// Provides the deck of cards used in the game
        /// </summary>
        protected Deck? Deck { get; set; }

        /// <summary>
        /// Stores the current player index
        /// </summary>
        protected int CurrentPlayerIndex { get; set; }

        /// <summary>
        /// Provides the player scores for each round of the game
        /// </summary>
        protected Dictionary<GamePlayer, List<int>> Scores { get; set; }

        /// <summary>
        /// Stores each player's hand
        /// </summary>
        protected Dictionary<GamePlayer, Hand> Hands { get; set; }

        /// <summary>
        /// Keeps track of each round
        /// </summary>
        protected int Round { get; set; } = 0;

        /// <summary>
        /// Stores the center pool of played cards
        /// </summary>
        protected Dictionary<GamePlayer, Card> PlayedCards { get; set; }

        /// <summary>
        /// Provides the cards that should be visible to all players
        /// </summary>
        protected List<Card> CenterCards { get; set; }

        /// <summary>
        /// Determines when the last game status update was sent out
        /// </summary>
        DateTime LastUpdateSent { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Defines the maximum number of cards to deal to the players
        /// If 0 or negative, will deal the maximum number of cards until
        /// the deck is empty
        /// </summary>
        protected int CardDealLimit { get; set; } = 0;

        /// <summary>
        /// Constructs the generic game class
        /// </summary>
        /// <param name="gameId">The game ID of the game</param>
        /// <param name="players">The players to add to the game</param>
        public GenericGame(int gameId, GamePlayer[] players)
        {
            // Store the game ID
            GameID = gameId;

            // Check the number of players
            if (players == null || players.Length != 4)
            {
                throw new GameException(gameId, "Must provide four players to game");
            }

            // Loop through each player to ensure that there are no duplicates
            for (int i = 0; i < players.Length; ++i)
            {
                for (int j = i + 1; j < players.Length; ++j)
                {
                    if (players[i] == players[j])
                    {
                        throw new GameException(gameId, "Cannot have duplicate players in the same game");
                    }
                }
            }

            // Save the players
            Players = players;

            // Initialize the game states
            PlayedCards = new Dictionary<GamePlayer, Card>();
            Scores = new Dictionary<GamePlayer, List<int>>();
            Hands = new Dictionary<GamePlayer, Hand>();
            CenterCards = new List<Card>();

            foreach (GamePlayer p in this.Players)
            {
                Scores.Add(p, new List<int>());
                Hands.Add(p, new Hand());
            }
        }

        /// <summary>
        /// Class to be called to setup a new round
        /// </summary>
        protected abstract void SetupNewRound();

        /// <summary>
        /// Deals the deck to the players
        /// </summary>
        protected virtual void ShuffleAndDeal()
        {
            // Ensure a deck is provided
            if (Deck == null) throw new NullReferenceException(nameof(Deck));

            // Clear player hands
            foreach (GamePlayer p in Players) Hands[p].Clear();

            // Shuffle the deck and deal to each player
            Deck.Shuffle();
            int card_count = 0;
            while (Deck.HasNext())
            {
                // Add a card to each player
                foreach (GamePlayer p in Players)
                {
                    Hands[p].AddCard(Deck.Next() ?? throw new NullReferenceException("unable to get next card"));
                }

                // Increment the card count
                card_count += 1;

                // Stop dealing if we have reached the card limit
                if (CardDealLimit > 0 && card_count >= CardDealLimit) break;
            }

            // Sort the resulting player hands
            foreach (Hand h in Hands.Values)
            {
                h.Sort();
            }
        }

        /// <summary>
        /// Perform a player action
        /// </summary>
        /// <param name="p">The player requesting the action</param>
        /// <param name="msg">The game play message to respond to</param>
        public abstract void Action(GamePlayer p, MsgGamePlay msg);

        /// <summary>
        /// Called to finish a round, increment the round counter, and setup for the new roudn
        /// </summary>
        protected virtual void FinishRound()
        {
            Round += 1;

            if (IsActive())
            {
                SetupNewRound();
            }
        }

        /// <summary>
        /// Returns the current player
        /// </summary>
        /// <returns>The player associated with the current player index</returns>
        public GamePlayer CurrentPlayer()
        {
            return Players[CurrentPlayerIndex];
        }

        /// <summary>
        /// Detemrines if the game contains the provided player
        /// </summary>
        /// <param name="player">The player to check if contained within</param>
        /// <returns>True if the player is part of the game</returns>
        public bool ContainsPlayer(GamePlayer player)
        {
            foreach (GamePlayer p in Players)
            {
                if (p.Equals(player))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Increments the player index to the default next player
        /// </summary>
        public virtual void IncrementPlayer()
        {
            CurrentPlayerIndex = (CurrentPlayerIndex + 1) % Players.Length;
        }

        /// <summary>
        /// Sets the current player index based on the input game player
        /// </summary>
        /// <param name="p">The new player to set as the current player index</param>
        protected void SetCurrentPlayer(GamePlayer p)
        {
            for (int i = 0; i < Players.Length; ++i)
            {
                if (Players[i].Equals(p))
                {
                    CurrentPlayerIndex = i;
                    return;
                }
            }

            throw new GameException(GameID, "Cannot set player who isn't in the game");
        }

        /// <summary>
        /// Provides the overall summed scores for each player for each round
        /// </summary>
        /// <returns>A dictionary of the player scores, as a single integer, by player key</returns>
        public Dictionary<GamePlayer, int> OverallScores()
        {
            // Create the overall dictionary
            Dictionary<GamePlayer, int> overall_scores = new();

            // Loop through each player to add each round score to the overall score
            foreach (GamePlayer p in Players)
            {
                overall_scores[p] = 0;

                foreach (int s in Scores[p])
                {
                    overall_scores[p] += s;
                }
            }

            // Return the results
            return overall_scores;
        }

        /// <summary>
        /// Provides the current game type
        /// </summary>
        /// <returns>The game type for the associated game</returns>
        public abstract GameTypes GetGameType();

        /// <summary>
        /// Provides the current game status
        /// </summary>
        /// <param name="player">The player to get the current status for</param>
        /// <returns>The game status provided in a message that can be sent over the network</returns>
        virtual public MsgGameStatus GetGameStatus(GamePlayer player)
        {
            LastUpdateSent = DateTime.UtcNow;

            List<Hand> playerHands = new();
            List<Card?> poolValues = new();
            List<int> playerScores = new();

            foreach (GamePlayer p in Players)
            {
                playerHands.Add(Hands[p]);

                if (PlayedCards.ContainsKey(p)) poolValues.Add(PlayedCards[p]);
                else poolValues.Add(null);

                playerScores.Add(OverallScores()[p]);
            }

             return new MsgGameStatus(
                gameId: GameID,
                gameType: (int)GetGameType(),
                players: Players.ToList(),
                hands: playerHands,
                status: string.Empty,
                currentPlayer: CurrentPlayerIndex,
                scores: playerScores,
                playedCards: poolValues,
                centerCards: CenterCards);
        }

        /// <summary>
        /// Determines if the game is active
        /// </summary>
        /// <returns>True if the game is still playable</returns>
        public abstract bool IsActive();

        /// <summary>
        /// Determines the game is not longer active and has timed and
        /// and may be deleted
        /// </summary>
        /// <returns>True if the game has timed out</returns>
        public bool Timeout()
        {
            return
                !IsActive() &&
                DateTime.UtcNow - LastUpdateSent > TimeSpan.FromMinutes(5);
        }
    }
}
