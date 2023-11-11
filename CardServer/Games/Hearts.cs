using System;
using System.Collections.Generic;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using CardGameLibrary.Cards;
using CardGameLibrary.GameParameters;
using CardGameLibrary.Messages;

namespace CardServer.Games
{
    /// <summary>
    /// Defines hearts-specific logic
    /// </summary>
    public class Hearts : GenericTrickGame
    {
        /// <summary>
        /// Game state to check if the passing round is complete
        /// </summary>
        bool PassRoundComplete { get; set; } = false;

        /// <summary>
        /// Game state to determine if hearts have been broken
        /// </summary>
        bool HeartsBroken { get; set; } = false;

        /// <summary>
        /// Game parameter to store the lead card
        /// </summary>
        static readonly Card START_CARD = new(
            suit: Card.Suit.Club,
            value: Card.Value.Two);

        /// <summary>
        /// Defines the special queen of spades card
        /// </summary>
        static readonly Card QUEEN_OF_SPADES = new(
            suit: Card.Suit.Spade,
            value: Card.Value.Queen);

        /// <summary>
        /// Game state to store the individual round scores for players for the current round only
        /// </summary>
        Dictionary<GamePlayer, int> RoundScores { get; } = new();

        /// <summary>
        /// Game state to store the cards to be passed to the various players
        /// </summary>
        Dictionary<GamePlayer, List<Card>> PassingCards { get; set; } = new();

        /// <summary>
        /// Definition for the direction that cards should be passed
        /// </summary>
        enum PassDirection
        {
            Left = 0,
            Right = 1,
            Across = 2,
            None = 3
        };

        /// <summary>
        /// Constructor to create the Hearts game
        /// </summary>
        /// <param name="game_id">The Game ID to associate with the Hearts game</param>
        /// <param name="players">The players to add to the hearts game</param>
        public Hearts(int game_id, GamePlayer[] players) : base(game_id: game_id, players: players)
        {
            // Throw an error if an invalid number of players are provided
            if (players.Length != 4)
            {
                throw new GameException(game_id, "Four players must be provided to a Hearts game");
            }

            // Create the card deck
            Deck = new Deck();

            // Setup for the first hand
            SetupNewRound();
        }

        /// <summary>
        /// Determine the current passing direction based on the current round
        /// </summary>
        /// <returns>Provides the passing direction for the current round</returns>
        PassDirection CurrentPassDirection()
        {
            return (Round % 4) switch
            {
                0 => PassDirection.Left,
                1 => PassDirection.Right,
                2 => PassDirection.Across,
                _ => PassDirection.None
            };
        }

        /// <summary>
        /// Sets up the game state for a new hand
        /// </summary>
        protected override void SetupNewRound()
        {
            // Deal out the next hand
            ShuffleAndDeal();

            // Setup the game state for the next round
            PassRoundComplete = CurrentPassDirection() == PassDirection.None;
            HeartsBroken = false;

            // Setup each of the list game state parameter
            PassingCards.Clear();
            RoundScores.Clear();
            foreach (GamePlayer p in Players)
            {
                RoundScores.Add(p, 0);
                PassingCards.Add(p, new List<Card>());
            }

            // Set the starting player index based on whether the passing round is complete or not
            if (!PassRoundComplete)
            {
                CurrentPlayerIndex = -1;
            }
            else
            {
                DetermineStartingPlayer();
            }
        }

        /// <summary>
        /// Determine the starting player based on whichever player has the starting card
        /// </summary>
        void DetermineStartingPlayer()
        {
            foreach (GamePlayer p in Players)
            {
                if (Hands[p].HasCard(START_CARD))
                {
                    SetCurrentPlayer(p);
                    break;
                }
            }
        }

        /// <summary>
        /// Finishes a trick and updates the game state accordingly
        /// </summary>
        protected override void FinishTrick()
        {
            // Call the base class value
            base.FinishTrick();

            // Determine the winner of the suit
            Card winning_card = LoadCard ?? throw new NullReferenceException(nameof(winning_card));
            int points_to_add = 0;
            GamePlayer? winning_player = null;

            foreach (var kvp in PlayedCards)
            {
                // Extract values
                GamePlayer p = kvp.Key;
                Card c = kvp.Value;

                // Check for points to add
                if (c.CardSuit == Card.Suit.Heart)
                {
                    points_to_add += 1;
                    HeartsBroken = true;
                }
                else if (c.Equals(QUEEN_OF_SPADES))
                {
                    points_to_add += 13;
                }

                // Determine the new "winning" player of the round
                if (c.CardSuit == winning_card.CardSuit &&
                    c.CardValue >= winning_card.CardValue)
                {
                    winning_card = c;
                    winning_player = p;
                }
            }

            // Reset the lead card
            LoadCard = null;

            // Set up the round point values
            if (winning_player != null)
            {
                RoundScores[winning_player] += points_to_add;
                SetCurrentPlayer(winning_player);
            }
            else
            {
                throw new GameException(GameID, "Cannot determine winning player");
            }

            // Finish the round if the round is complete
            if (trick_count == 13)
            {
                // Check if a player has a moonshot
                bool moonshot = false;
                GamePlayer? moonshot_player = null;

                foreach (var kvp in RoundScores)
                {
                    if (kvp.Value == 26)
                    {
                        moonshot = true;
                        moonshot_player = kvp.Key;
                    }
                }

                // Add the points based on the moonshot or not
                if (moonshot)
                {
                    foreach (GamePlayer p in Scores.Keys)
                    {
                        if (p.Equals(moonshot_player))
                        {
                            Scores[p].Add(0);
                        }
                        else
                        {
                            Scores[p].Add(26);
                        }
                    }
                }
                else
                {
                    foreach (GamePlayer p in RoundScores.Keys)
                    {
                        Scores[p].Add(RoundScores[p]);
                    }
                }

                // Setup and deal the new round
                FinishRound();
                PlayedCards.Clear();
            }
        }

        /// <summary>
        /// Perform a player action
        /// </summary>
        /// <param name="p">The player requesting the action</param>
        /// <param name="msg">The game play message to respond to</param>
        public override void Action(GamePlayer p, MsgGamePlay msg)
        {
            // Ensure that we are playing a card and the current hand contains the given card
            if (!Hands[p].Cards.Contains(msg.Card))
            {
                throw new GameException(
                    GameID,
                    $"Player hand {p.Name} does not contain card {msg.Card}");
            }

            // Play the hand if the passing round is complete
            if (PassRoundComplete)
            {
                // Skip if the current player isn't the one sending the action
                if (!p.Equals(CurrentPlayer()))
                {
                    return;
                }

                // Boolean to check if the provided card is valid
                bool card_valid = true;

                // Extract the current hand
                Hand h = Hands[p];

                // Perform checks for lead parameters
                if (LoadCard == null)
                {
                    // Check if trying to lead a heart before hearts has been broken
                    if (msg.Card.CardSuit == Card.Suit.Heart &&
                        !HeartsBroken &&
                        (h.HasCardOfSuit(Card.Suit.Club) || h.HasCardOfSuit(Card.Suit.Diamond) || h.HasCardOfSuit(Card.Suit.Spade)))
                    {
                        card_valid = false;
                        SetTrickMessage("Hearts hasn't been broken yet!");
                    }
                }
                else
                {
                    // Check if a player has a card of the lead suit before playing
                    if (LoadCard.CardSuit != msg.Card.CardSuit &&
                        h.HasCardOfSuit(LoadCard.CardSuit))
                    {
                        card_valid = false;
                        SetTrickMessage("Must play a card of the lead suit");
                    }
                }

                // Check if the first round for the two of clubs
                if (trick_count == 0)
                {
                    // Check for the start card
                    if (LoadCard == null && !msg.Card.Equals(START_CARD))
                    {
                        card_valid = false;
                        SetTrickMessage("Must lead the two of clubs");
                    }

                    // Check for the queen of spades or hearts on the first round
                    if (msg.Card.CardSuit == Card.Suit.Heart ||
                        msg.Card.Equals(QUEEN_OF_SPADES))
                    {
                        // Loop through to check if the player has another card they can play
                        bool has_another_card = false;
                        bool has_queen_of_spades = false;
                        foreach (Card c in h.Cards)
                        {
                            if (c.CardSuit != Card.Suit.Heart &&
                                !c.Equals(QUEEN_OF_SPADES))
                            {
                                has_another_card = true;
                            }
                            if (c.Equals(QUEEN_OF_SPADES))
                            {
                                has_queen_of_spades = true;
                            }
                        }

                        // Set the message as invalid on the first round
                        if (has_another_card)
                        {
                            card_valid = false;
                            SetTrickMessage("Cannot play points on the first trick");
                        }
                        // Player must play queen of spades if they have it
                        else if (has_queen_of_spades && !msg.Card.Equals(QUEEN_OF_SPADES))
                        {
                            card_valid = false;
                            SetTrickMessage("Must play the queen of spades");
                        }
                    }
                }

                // Only play the card if the card is actually active
                if (card_valid)
                {
                    // Check if the lead card is valid
                    LoadCard ??= msg.Card;

                    // Clear the last trick
                    if (TrickCanBeCompleted())
                    {
                        PlayedCards.Clear();
                    }

                    // Play the respective card from the hands
                    h.PlayCard(msg.Card);
                    PlayedCards.Add(p, msg.Card);

                    // Set the next player, finishing the round if necessary
                    if (TrickCanBeCompleted())
                    {
                        FinishTrick();
                    }
                    else
                    {
                        IncrementPlayer();
                    }

                    // Reset the round message
                    SetTrickMessage();
                }
            }
            // Run through the passing round
            else
            {
                // Check if the player can add a card to the cards to pass
                if (PassingCards[p].Count < 3)
                {
                    // Extract the current hand
                    Hand h = Hands[p];

                    // Remove the played card
                    h.PlayCard(msg.Card);
                    PassingCards[p].Add(msg.Card);

                    // Clear the center cards if not already cleared
                    if (PlayedCards.Count > 0)
                    {
                        PlayedCards.Clear();
                    }
                }

                // Determine if we can complete the passing round
                bool can_complete_round = PassingCards.Count == 4;

                foreach (List<Card> cl in PassingCards.Values)
                {
                    if (cl.Count != 3)
                    {
                        can_complete_round = false;
                    }
                }

                // Complete the passing round if possible
                if (can_complete_round)
                {
                    // Determine the index modifier to add to the player index for passing 
                    int modifier = 0;

                    switch (CurrentPassDirection())
                    {
                        case PassDirection.Left:
                            modifier = 1;
                            break;
                        case PassDirection.Right:
                            modifier = 3;
                            break;
                        case PassDirection.Across:
                            modifier = 2;
                            break;
                    }

                    // Loop through each of the players
                    for (int i = 0; i < Players.Length; ++i)
                    {
                        // Extract the to/from players for passing cards
                        GamePlayer from_player = Players[i];
                        GamePlayer to_player = Players[(i + modifier) % Players.Length];

                        // Add each of the cards from the passing player to the receiving player's hand
                        foreach (Card c in PassingCards[from_player])
                        {
                            Hands[to_player].AddCard(c);
                        }

                        // Sort the respective player's hand
                        Hands[to_player].Sort();
                    }

                    // Set the passing round as complete and determine the starting player
                    PassRoundComplete = true;
                    DetermineStartingPlayer();
                }
            }
        }

        /// <summary>
        /// Determines if the game is active
        /// </summary>
        /// <returns>True if the game is still playable</returns>
        public override bool IsActive()
        {
            // Loop through each game, returning false if any score is over 100
            foreach (int s in OverallScores().Values)
            {
                if (s >= 100)
                {
                    return false;
                }
            }

            // Otherwise, return true as still playable/active
            return true;
        }

        /// <summary>
        /// Provides the current game type
        /// </summary>
        /// <returns>The game type for the associated game</returns>
        override public GameTypes GetGameType()
        {
            return GameTypes.Hearts;
        }

        /// <summary>
        /// Provides the current game status message for the
        /// current game state
        /// </summary>
        /// <returns>A string with the current game status</returns>
        protected override string GetGameStatusMsg()
        {
            // If the passing round is complete, note player turns
            if (!IsActive())
            {
                return "Game Over";
            }
            // Provide values if the passing round is not complete
            else if (!PassRoundComplete)
            {
                return $"Passing Cards {CurrentPassDirection()}";
            }
            // Otherwise, provide the player's turn
            else
            {
                return $"{CurrentPlayer().CapitalizedName}'s Turn";
            }
        }

        /// <summary>
        /// If true, will append a the trick message to the responding
        /// status message
        /// </summary>
        /// <returns>True if the trick message should be appended</returns>
        protected override bool CanShowTrickMessage()
        {
            return IsActive() && PassRoundComplete;
        }
    }
}
