using System;
using System.Collections.Generic;
using System.Text;
using CardGameLibrary.Cards;
using CardGameLibrary.Games;
using CardGameLibrary.Messages;

namespace CardServer.Games
{
    /// <summary>
    /// Defines the hearts-specific parameters for the Hearts game
    /// </summary>
    public class Hearts : GenericGame
    {
        /// <summary>
        /// Game state to check if the passing round is complete
        /// </summary>
        bool pass_round_complete = false;

        /// <summary>
        /// Defines the current trick count
        /// </summary>
        int trick_count = 0;

        /// <summary>
        /// Game state to determine if hearts have been broken
        /// </summary>
        bool hearts_broken = false;

        /// <summary>
        /// Game state to store the leading card in a trick
        /// </summary>
        Card lead_card = null;

        /// <summary>
        /// Game parameter to store the lead card
        /// </summary>
        static readonly Card start_card = new Card(
            suit: Card.Suit.Club,
            value: Card.Value.Two);

        /// <summary>
        /// Defines the special queen of spades card
        /// </summary>
        static readonly Card queen_spades = new Card(
            suit: Card.Suit.Spade,
            value: Card.Value.Queen);

        /// <summary>
        /// Message to store in a given round to provide as a game update to players
        /// </summary>
        string trick_msg = null;

        /// <summary>
        /// Game state to store the individual round scores for players for the current round only
        /// </summary>
        Dictionary<GamePlayer, int> round_scores = new Dictionary<GamePlayer, int>();

        /// <summary>
        /// Game state to store the cards to be passed to the various players
        /// </summary>
        Dictionary<GamePlayer, List<Card>> passing_cards = new Dictionary<GamePlayer, List<Card>>();

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
                throw new ArgumentException("Four players must be provided to a Hearts game");
            }

            // Create the card deck
            deck = new Deck();

            // Setup for the first hand
            SetupNewRound();
        }

        /// <summary>
        /// Determine the current passing direction based on the current round
        /// </summary>
        /// <returns>Provides the passing direction for the current round</returns>
        PassDirection CurrentPassDirection()
        {
            switch (round % 4)
            {
                case 0:
                    return PassDirection.Left;
                case 1:
                    return PassDirection.Right;
                case 2:
                    return PassDirection.Across;
                default:
                    return PassDirection.None;
            }
        }

        /// <summary>
        /// Sets up the game state for a new hand
        /// </summary>
        protected override void SetupNewRound()
        {
            // Clear player hands
            foreach (GamePlayer p in players) hands[p].Clear();

            // Shuffle the deck and deal to each player
            deck.Shuffle();
            while (deck.HasNext())
            {
                foreach (GamePlayer p in players)
                {
                    hands[p].AddCard(deck.Next());
                }
            }

            // Sort the resulting player hands
            foreach (Hand h in hands.Values)
            {
                h.Sort();
            }

            // Setup the game state for the next round
            pass_round_complete = CurrentPassDirection() == PassDirection.None;
            trick_count = 0;
            hearts_broken = false;

            // Setup each of the list game state parameter
            passing_cards.Clear();
            round_scores.Clear();
            foreach (GamePlayer p in players)
            {
                round_scores.Add(p, 0);
                passing_cards.Add(p, new List<Card>());
            }

            // Set the starting player index based on whether the passing round is complete or not
            if (!pass_round_complete)
            {
                current_player_ind = -1;
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
            foreach (GamePlayer p in players)
            {
                if (hands[p].HasCard(start_card))
                {
                    SetCurrentPlayer(p);
                    break;
                }
            }
        }

        /// <summary>
        /// Finishes a trick and updates the game state accordingly
        /// </summary>
        void FinishTrick()
        {
            // Skip if the center pool isn't full
            if (center_pool.Count != 4) return;

            // Increment the trick count
            trick_count += 1;

            // Determine the winner of the suit
            Card winning_card = lead_card;
            int points_to_add = 0;
            GamePlayer winning_player = null;

            foreach (var kvp in center_pool)
            {
                // Extract values
                Card c = kvp.Value;
                GamePlayer p = kvp.Key;

                // Check for points to add
                if (c.suit == Card.Suit.Heart)
                {
                    points_to_add += 1;
                    hearts_broken = true;
                }
                else if (c.Equals(queen_spades))
                {
                    points_to_add += 13;
                }

                // Determine the new "winning" player of the round
                if (c.suit == winning_card.suit &&
                    c.value >= winning_card.value)
                {
                    winning_card = c;
                    winning_player = p;
                }
            }

            // Set up the round point values
            if (winning_player != null)
            {
                round_scores[winning_player] += points_to_add;
                SetCurrentPlayer(winning_player);
            }
            else
            {
                current_player_ind = 0;
            }

            // Clear the lead card
            lead_card = null;

            // Check if the entire round is complete
            bool round_complete = true;
            foreach (Hand h in hands.Values)
            {
                if (h.cards.Count > 0)
                {
                    round_complete = false;
                }
            }

            // Finish the round if the round is complete
            if (round_complete)
            {
                // Add up the scores
                bool moonshot = false;
                GamePlayer moonshot_player = null;

                foreach (var kvp in round_scores)
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
                    foreach (GamePlayer p in scores.Keys)
                    {
                        if (p.Equals(moonshot_player))
                        {
                            scores[p].Add(0);
                        }
                        else
                        {
                            scores[p].Add(26);
                        }
                    }
                }
                else
                {
                    foreach (GamePlayer p in round_scores.Keys)
                    {
                        scores[p].Add(round_scores[p]);
                    }
                }

                // Setup and deal the new round
                FinishRound();
            }
        }

        /// <summary>
        /// Sets the trick message. If the message is already set, does nothing
        /// </summary>
        /// <param name="msg">The new message to set as the trick message</param>
        void SetTrickMessage(string msg)
        {
            if (trick_msg == null || trick_msg.Length == 0)
            {
                trick_msg = msg;
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
            if (!hands[p].cards.Contains(msg.card))
            {
                return;
            }

            // Play the hand if the passing round is complete
            if (pass_round_complete)
            {
                if (p.Equals(CurrentPlayer()))
                {
                    // Boolean to check if the provided card is valid
                    bool card_valid = true;

                    // Extract the current hand
                    Hand h = hands[p];

                    // Perform checks for lead parameters
                    if (lead_card == null)
                    {
                        // Check if trying to lead a heart before hearts has been broken
                        if (msg.card.suit == Card.Suit.Heart &&
                            !hearts_broken &&
                            (h.HasCardOfSuit(Card.Suit.Club) || h.HasCardOfSuit(Card.Suit.Diamond) || h.HasCardOfSuit(Card.Suit.Spade)))
                        {
                            card_valid = false;
                            SetTrickMessage("Hearts hasn't been broken yet!");
                        }
                    }
                    else
                    {
                        // Check if a player has a card of the lead suit before playing
                        if (lead_card.suit != msg.card.suit &&
                            h.HasCardOfSuit(lead_card.suit))
                        {
                            card_valid = false;
                            SetTrickMessage("Must play a card of the lead suit");
                        }
                    }

                    // Check if the first round for the two of clubs
                    if (trick_count == 0)
                    {
                        // Check for the start card
                        if (lead_card == null && !msg.card.Equals(start_card))
                        {
                            card_valid = false;
                            SetTrickMessage("Must lead the two of clubs");
                        }

                        // Check for the queen of spades or hearts on the first round
                        if (msg.card.suit == Card.Suit.Heart ||
                            msg.card.Equals(queen_spades))
                        {
                            // Loop through to check if the player has another card they can play
                            bool has_another_card = false;
                            bool has_queen_of_spades = false;
                            foreach (Card c in h.cards)
                            {
                                if (c.suit != Card.Suit.Heart &&
                                    !c.Equals(queen_spades))
                                {
                                    has_another_card = true;
                                }
                                if (c.Equals(queen_spades))
                                {
                                    has_queen_of_spades = true;
                                }
                            }

                            // Set the message as invalid on teh first round
                            if (has_another_card)
                            {
                                card_valid = false;
                                SetTrickMessage("Cannot play points on the first trick");
                            }
                            // Player must play queen of spades if they have it
                            else if (has_queen_of_spades && !msg.card.Equals(queen_spades))
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
                        if (lead_card == null)
                        {
                            lead_card = msg.card;
                        }

                        // Reset the pool if all four cards have been played on the start of the next turn
                        // and clear the lead card
                        if (center_pool.Count == 4)
                        {
                            center_pool.Clear();
                        }

                        // Play the respective card from the hands
                        h.PlayCard(msg.card);
                        center_pool.Add(p, msg.card);

                        // Set the next player, finishing the round if necessary
                        if (center_pool.Count == 4)
                        {
                            FinishTrick();
                        }
                        else
                        {
                            IncrementPlayer();
                        }

                        // Reset the round message
                        trick_msg = null;
                    }
                }
            }
            // Run through the passing round
            else
            {
                // Check if the player can add a card to the cards to pass
                if (passing_cards[p].Count < 3)
                {
                    // Extract the current hand
                    Hand h = hands[p];

                    // Remove the played card
                    h.PlayCard(msg.card);
                    passing_cards[p].Add(msg.card);
                }

                // Determine if we can complete the passing round
                bool can_complete_round = passing_cards.Count == 4;

                foreach (List<Card> cl in passing_cards.Values)
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
                    for (int i = 0; i < players.Length; ++i)
                    {
                        // Extract the to/from players for passing cards
                        GamePlayer from_player = players[i];
                        GamePlayer to_player = players[(i + modifier) % players.Length];

                        // Add each of the cards from the passing player to the receiving player's hand
                        foreach (Card c in passing_cards[from_player])
                        {
                            hands[to_player].AddCard(c);
                        }

                        // Sort the respective player's hand
                        hands[to_player].Sort();
                    }

                    // Set the passing round as complete and determine the starting player
                    pass_round_complete = true;
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
        /// Provides the current Hearts game status
        /// </summary>
        /// <returns>The game status provided in a message that can be sent over the network</returns>
        override public MsgGameStatus GetGameStatus()
        {
            // Determine the base class game status
            MsgGameStatus msg = base.GetGameStatus();

            // If the passing round is complete, note player turns
            if (!IsActive())
            {
                msg.current_game_status = "Game Over";
            }
            else if (pass_round_complete)
            {
                // Setup the player turn
                msg.current_game_status = string.Format(
                    "{0:}'s Turn",
                    CurrentPlayer().CapitalizedName());

                // Append any trick message to the game status if provided
                if (trick_msg != null && trick_msg.Length > 0)
                {
                    msg.current_game_status = string.Format(
                        "{0:} - {1:}",
                        msg.current_game_status,
                        trick_msg);
                }
            }
            // Otherwise, state the current passing parameters
            else
            {
                msg.current_game_status = string.Format(
                    "Passing Cards {0:}",
                    Enum.GetName(
                        typeof(PassDirection),
                        CurrentPassDirection()));
            }

            // Return the resulting message
            return msg;
        }
    }
}
