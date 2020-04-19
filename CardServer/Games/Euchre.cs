using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net.Security;
using System.Text;
using CardGameLibrary.Cards;
using CardGameLibrary.GameParameters;
using CardGameLibrary.Messages;
using CardServer.Players;

namespace CardServer.Games
{
    /// <summary>
    /// Defines Euchre-specific game logic
    /// </summary>
    public class Euchre : GenericTrickGame
    {
        Card.Suit trump;
        static readonly Card.Value bower_value = Card.Value.Jack;

        int going_alone_skip_ind = -1;

        bool bidding_complete;
        bool first_bid_round_complete;
        GamePlayer bidding_player;

        bool screw_the_dealer = true;

        bool TrumpSelected
        {
            get
            {
                return bidding_player != null;
            }
        }

        Dictionary<GamePlayer, int> tricks_taken;

        Card kitty_card = null;

        /// <summary>
        /// Constructs a Euchre game instance
        /// </summary>
        /// <param name="game_id">The game ID of the game</param>
        /// <param name="players">The players to add to the game</param>
        public Euchre(int game_id, GamePlayer[] players) : base(game_id: game_id, players: players)
        {
            // Define the number of cards to deal
            card_deal_limit = 5;

            // Setup the Euchre deck
            deck = new Deck(
                values: new Card.Value[]
                {
                    Card.Value.Nine,
                    Card.Value.Ten,
                    Card.Value.Jack,
                    Card.Value.Queen,
                    Card.Value.King,
                    Card.Value.Ace
                });

            // Setup for the first hand
            SetupNewRound();
        }

        /// <summary>
        /// Provides a card value for comparison specific to Euchre
        /// </summary>
        /// <param name="c">The card to get the value for</param>
        /// <returns>An integer corresponding with the card value</returns>
        public int EuchreCardValue(Card c)
        {
            // Only provide Euchre sorting if trump is selected
            // Otherwise, provide the default card value
            if (TrumpSelected)
            {
                // Extract the current suit and value for the card
                int effective_suit = (int)c.suit;
                int effective_value = (int)c.value;

                // Adjust the effective suit for trump
                if (IsTrumpCard(c))
                {
                    effective_suit = 5;
                }

                // Set the right bower above all other cards
                if (c.Equals(GetRightBower()))
                {
                    effective_value = (int)Card.Value.Ace + 2;
                }
                // Check if the card is the left bower
                else if (c.Equals(GetLeftBower()))
                {
                    // Set the left bower above all other cards except the right bower
                    effective_value = (int)Card.Value.Ace + 1;
                }

                // Return the resulting card value
                return effective_value + effective_suit * 32;
            }
            else
            {
                return c.CardValue();
            }
        }

        /// <summary>
        /// Determines the number of expected cards in a trick
        /// </summary>
        /// <returns>Returns the number of cards expected in a trick</returns>
        override protected int CountCenterCardExpected()
        {
            return (going_alone_skip_ind >= 0) ? 3 : 4;
        }

        /// <summary>
        /// Determines if the provided card is part of trump
        /// </summary>
        /// <param name="card">The card to check</param>
        /// <returns>True if the card is part of trump</returns>
        public bool IsTrumpCard(Card card)
        {
            return EffectiveSuit(card) == trump;
        }

        /// <summary>
        /// Provide a comparer to be used in sorting Euchre cards
        /// </summary>
        /// <param name="c1">The first card to compare</param>
        /// <param name="c2">The second card to compare</param>
        /// <returns>Equal if 0, c1 < c2 if < 0</returns>
        public int EuchreComparer(Card c1, Card c2)
        {
            return EuchreCardValue(c1) - EuchreCardValue(c2);
        }

        /// <summary>
        /// Performs incrementing to include support for player skipping
        /// </summary>
        public override void IncrementPlayer()
        {
            // Perform the standard player increment
            base.IncrementPlayer();

            // If a player is being skipped for going alone, increment again
            if (going_alone_skip_ind == current_player_ind)
            {
                base.IncrementPlayer();
            }
        }

        /// <summary>
        /// Provides the left bower associated with the suit provided
        /// </summary>
        /// <returns>The left bower associated with the current trump suit</returns>
        public Card GetLeftBower()
        {
            Card.Suit bower_suit;

            switch (trump)
            {
                case Card.Suit.Club:
                    bower_suit = Card.Suit.Spade;
                    break;
                case Card.Suit.Spade:
                    bower_suit = Card.Suit.Club;
                    break;
                case Card.Suit.Diamond:
                    bower_suit = Card.Suit.Heart;
                    break;
                case Card.Suit.Heart:
                    bower_suit = Card.Suit.Diamond;
                    break;
                default:
                    throw new GameException(
                        game_id,
                        string.Format(
                            "Unknown trump suit {0:} provided",
                            Enum.GetName(
                                enumType: typeof(Card.Suit),
                                trump)));
            }

            return new Card(
                suit: bower_suit,
                value: bower_value);
        }

        /// <summary>
        /// Provides the right bower associated with the suit provided
        /// </summary>
        /// <returns>The right bower associated with the current trump suit</returns>
        public Card GetRightBower()
        {
            return new Card(
                suit: trump,
                value: bower_value);
        }

        /// <summary>
        /// Determines the player who deals the current round
        /// </summary>
        /// <returns>Dealer player</returns>
        GamePlayer Dealer()
        {
            return players[round % players.Length];
        }

        /// <summary>
        /// Sets up the game state for a new hand
        /// </summary>
        protected override void SetupNewRound()
        {
            // Deal the hand to players
            ShuffleAndDeal();

            // Setup the game state
            lead_card = null;

            // Setup bidding states
            bidding_complete = false;
            first_bid_round_complete = false;
            going_alone_skip_ind = -1;
            bidding_player = null;

            // Initialize trump
            trump = Card.Suit.Club;

            // Reset tricks so far
            tricks_taken = new Dictionary<GamePlayer, int>();
            foreach (GamePlayer p in players)
            {
                tricks_taken.Add(p, 0);
            }

            // Add a value to the center cards
            center_cards.Clear();
            kitty_card = deck.Next();
            center_cards.Add(kitty_card);

            // Set the correct player to start
            SetCurrentPlayer(Dealer());
            IncrementPlayer();
        }

        /// <summary>
        /// Provides the effective suit for the provided card.
        /// </summary>
        /// <param name="c">The card to get the effective suit of</param>
        /// <returns>
        /// By default, provides the card's suit. However, for the left-bower, provides
        /// the trump suit
        /// </returns>
        Card.Suit EffectiveSuit(Card c)
        {
            if (c.Equals(GetLeftBower()))
            {
                return trump;
            }
            else
            {
                return c.suit;
            }
        }

        /// <summary>
        /// Provides the index of the partner player
        /// </summary>
        /// <param name="player_index">The current player's index</param>
        /// <returns>The partner player's index</returns>
        int PartnerPlayer(int player_index)
        {
            return (player_index + 2) % players.Length;
        }

        /// <summary>
        /// Sorts players' hands
        /// </summary>
        void SortHands()
        {
            foreach (Hand hp in hands.Values)
            {
                hp.Sort(comparison: EuchreComparer);
            }
        }

        /// <summary>
        /// Defines the actions to take in the Euchre game
        /// </summary>
        /// <param name="p">The player performing the action</param>
        /// <param name="msg">The card game action</param>
        public override void Action(GamePlayer p, MsgGamePlay msg)
        {
            // Skip if the current player isn't the one sending the action
            if (!p.Equals(CurrentPlayer()))
            {
                throw new GameException(
                    game_id: game_id,
                    message: "Player cannot perform an action until their turn");
            }

            if (bidding_complete)
            {
                // Boolean to check if the provided card is valid
                bool card_valid = true;

                // Extract the current hand
                Hand h = hands[p];

                // Perform checks for following suit
                if (lead_card != null)
                {
                    // The player must play the lead suit if available
                    if (
                        EffectiveSuit(lead_card) != EffectiveSuit(msg.card) &&
                        h.HasCardWith(x => EffectiveSuit(x) == EffectiveSuit(lead_card)))
                    {
                        card_valid = false;
                        SetTrickMessage("Player must follow suit");
                    }
                }

                // Only play the card if it is valid
                if (card_valid)
                {
                    // Reset the trick message
                    SetTrickMessage();

                    // Check if the lead card is valid
                    if (lead_card == null)
                    {
                        lead_card = msg.card;
                    }

                    // Clear the last trick
                    if (TrickCanBeCompleted())
                    {
                        played_cards.Clear();
                    }

                    // Play the respective card from the hand
                    h.PlayCard(msg.card);
                    played_cards.Add(p, msg.card);

                    // Set the next player, finishing the round if necessary
                    if (TrickCanBeCompleted())
                    {
                        FinishTrick();
                    }
                    else
                    {
                        IncrementPlayer();
                    }
                }
            }
            else
            {
                // Determine if trump has been selected and the dealer must discard a card
                if (Dealer().Equals(p) &&
                    hands[Dealer()].cards.Count > 5 &&
                    TrumpSelected)
                {
                    // Discard the card that the dealer plays
                    // Set bidding to complete
                    if (hands[Dealer()].HasCard(msg.card))
                    {
                        hands[Dealer()].PlayCard(msg.card);
                        bidding_complete = true;
                    }
                }

                // Only track special cards played for bidding-specific rounds
                if (msg.card.IsSpecial())
                {
                    // Allow skipping from any 
                    if (msg.card.data == EuchreParameters.skip.data)
                    {
                        // Check if the dealer is the current player for defining parameters
                        if (Dealer().Equals(CurrentPlayer()))
                        {
                            if (!first_bid_round_complete)
                            {
                                // Set the first bidding round to complete and increment
                                first_bid_round_complete = true;
                                IncrementPlayer();
                            }
                            else if (!screw_the_dealer)
                            {
                                // Re-deal the hand, with a new dealer
                                round += 1;
                                SetupNewRound();
                                return;
                            }
                        }
                        else
                        {
                            // By default, increment the player
                            IncrementPlayer();
                        }
                    }

                    // Perform steps for the first bidding round
                    if (!first_bid_round_complete)
                    {
                        // Allow the dealer to pickup the kitty-card
                        if (msg.card.data == EuchreParameters.pickup_card.data ||
                            msg.card.data == EuchreParameters.go_alone.data)
                        {
                            // Set trump as selected
                            bidding_player = p;
                            trump = kitty_card.suit;
                            center_cards.Clear();

                            // Allow the player to go it alone
                            if (msg.card.data == EuchreParameters.go_alone.data)
                            {
                                going_alone_skip_ind = PartnerPlayer(current_player_ind);
                            }

                            // Set the dealer as the current player so they may discard
                            // However, if the dealer is selected as the going-alone player, don't make them pick up the card
                            if (going_alone_skip_ind >= 0 && players[going_alone_skip_ind].Equals(Dealer()))
                            {
                                bidding_complete = true;
                            }
                            else
                            {
                                // Add the kitty-card to the dealer's hand
                                hands[Dealer()].AddCard(kitty_card);
                                SetCurrentPlayer(Dealer());
                                SortHands();
                            }
                        }
                    }
                    // Perform steps for the second bidding round
                    else
                    {
                        // Allow players to select trump
                        if (msg.card.data == EuchreParameters.go_alone.data)
                        {
                            going_alone_skip_ind = PartnerPlayer(current_player_ind);
                        }
                        if (msg.card.data == GameAction.select_clubs.data)
                        {
                            bidding_player = p;
                            bidding_complete = true;
                            trump = Card.Suit.Club;
                        }
                        else if (msg.card.data == GameAction.select_diamonds.data)
                        {
                            bidding_player = p;
                            bidding_complete = true;
                            trump = Card.Suit.Diamond;
                        }
                        else if (msg.card.data == GameAction.select_hearts.data)
                        {
                            bidding_player = p;
                            bidding_complete = true;
                            trump = Card.Suit.Heart;
                        }
                        else if (msg.card.data == GameAction.select_spades.data)
                        {
                            bidding_player = p;
                            bidding_complete = true;
                            trump = Card.Suit.Spade;
                        }
                    }
                }

                // Perform any last-minute items before finishing the bidding round
                if (bidding_complete)
                {
                    // Adjust player hands for going alone
                    if (going_alone_skip_ind >= 0)
                    {
                        hands[players[going_alone_skip_ind]].cards.Clear();
                    }

                    // Sort each hand
                    SortHands();

                    // Reset the center cards
                    center_cards.Clear();

                    // Set the player to the left of the dealer
                    SetCurrentPlayer(Dealer());
                    IncrementPlayer();
                }
            }
        }

        /// <summary>
        /// Finishes a trick after all cards have been played
        /// </summary>
        override protected void FinishTrick()
        {
            // Call the base value
            base.FinishTrick();

            // Determine the winner of the suit
            Card winning_card = lead_card;
            GamePlayer winning_player = null;

            foreach (var kvp in played_cards)
            {
                // Extract values
                GamePlayer p = kvp.Key;
                Card c = kvp.Value;

                // Check for points to add
                if ((EffectiveSuit(winning_card) == EffectiveSuit(c) && EuchreComparer(c, winning_card) >= 0) ||
                    (!IsTrumpCard(winning_card) && IsTrumpCard(c)))
                {
                    winning_card = c;
                    winning_player = p;
                }
            }

            // Add tricks taken so far
            tricks_taken[winning_player] += 1;

            // Reset the lead card
            lead_card = null;

            // Set the next player to the winning player
            SetCurrentPlayer(winning_player);

            // Finish the round if it is complete
            if (trick_count == 5)
            {
                // Add up tricks
                int ns_tricks = tricks_taken[players[0]] + tricks_taken[players[2]];
                int ew_tricks = tricks_taken[players[1]] + tricks_taken[players[3]];

                // Bidding Partnership
                int bidding_partnership = -1;
                for (int i = 0; i < players.Length; ++i)
                {
                    if (bidding_player.Equals(players[i]))
                    {
                        bidding_partnership = i % 2;
                        break;
                    }
                }

                // Determine the winning number of tricks
                int winning_count = Math.Max(
                    ns_tricks,
                    ew_tricks);

                // Determine the points to add
                int partnership_winning;
                int points_to_add;

                // Check if the bidding partership won
                if ((ns_tricks > ew_tricks && bidding_partnership == 0) ||
                    (ew_tricks > ns_tricks && bidding_partnership == 1))
                {
                    // Bidding partnership has won
                    partnership_winning = bidding_partnership;

                    // Check for a march
                    if (winning_count == 5)
                    {
                        if (going_alone_skip_ind < 0)
                        {
                            points_to_add = 2;
                        }
                        else
                        {
                            points_to_add = 5;
                        }
                    }
                    else
                    {
                        points_to_add = 1;
                    }
                }
                // Otherwise, defending partnership has won
                else
                {
                    // Defending partnership has won
                    partnership_winning = (bidding_partnership + 1) % 2;
                    points_to_add = 2;
                }

                // Add in the points
                for (int i = 0; i < players.Length; ++i)
                {
                    if (i % 2 == partnership_winning)
                    {
                        scores[players[i]].Add(points_to_add);
                    }
                    else
                    {
                        scores[players[i]].Add(0);
                    }
                }

                // Finish the round
                FinishRound();
                played_cards.Clear();
            }
        }

        /// <summary>
        /// Determines if the game is active
        /// </summary>
        /// <returns>True if the game is still playable</returns>
        public override bool IsActive()
        {
            // Loop through each game, returning false if any score is over 11
            foreach (int s in OverallScores().Values)
            {
                if (s >= 10)
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
            return GameTypes.Euchre;
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
            else if (!bidding_complete)
            {
                string response_str;

                if (TrumpSelected)
                {
                    response_str = string.Format(
                        "Selected trump {0:}, {1:} must discard a card",
                        CurrentPlayer().ShortName(),
                        Enum.GetName(
                            typeof(Card.Suit),
                            trump));
                }
                else if (first_bid_round_complete)
                {
                    response_str = string.Format(
                        "{0:} may select trump",
                        CurrentPlayer().ShortName());
                }
                else
                {
                    response_str = string.Format(
                        "{0:} may order-up the displayed card",
                        CurrentPlayer().ShortName());
                }

                return string.Format(
                    "{0:} (Dealer {1:})",
                    response_str,
                    Dealer().ShortName());
            }
            // Otherwise, provide the player's turn
            else
            {
                return string.Format(
                    "{0:}'s Turn (Dealer {1:}, Trump {2:})",
                    CurrentPlayer().ShortName(),
                    Dealer().ShortName(),
                    Enum.GetName(
                        trump.GetType(),
                        trump));
            }
        }

        /// <summary>
        /// If true, will append a the trick message to the responding
        /// status message
        /// </summary>
        /// <returns>True if the trick message should be appended</returns>
        protected override bool CanShowTrickMessage()
        {
            return IsActive();
        }

        /// <summary>
        /// Provides the current Euchre game status
        /// </summary>
        /// <param name="player">The player to get the current status for</param>
        /// <returns>The game status provided in a message that can be sent over the network</returns>
        override public MsgGameStatus GetGameStatus(GamePlayer player)
        {
            // Determine the base class game status
            MsgGameStatus msg = base.GetGameStatus(player: player);

            // Determine how to adjust for the bidding for the current player
            if (!bidding_complete &&
                !TrumpSelected &&
                CurrentPlayer().Equals(player))
            {
                // Create  copy of the list
                List<Card> center_cards = new List<Card>(msg.center_action_cards);

                // Add an action card to both pass, go alone, and pick up the card
                if (!first_bid_round_complete)
                {
                    center_cards.Add(Card.CreateSpecialCard(EuchreParameters.go_alone.data));
                    center_cards.Add(Card.CreateSpecialCard(EuchreParameters.pickup_card.data));
                    center_cards.Add(Card.CreateSpecialCard(EuchreParameters.skip.data));
                }
                else
                {
                    if (kitty_card.suit != Card.Suit.Club) center_cards.Add(Card.CreateSpecialCard(GameAction.select_clubs.data));
                    if (kitty_card.suit != Card.Suit.Diamond) center_cards.Add(Card.CreateSpecialCard(GameAction.select_diamonds.data));
                    if (kitty_card.suit != Card.Suit.Heart) center_cards.Add(Card.CreateSpecialCard(GameAction.select_hearts.data));
                    if (kitty_card.suit != Card.Suit.Spade) center_cards.Add(Card.CreateSpecialCard(GameAction.select_spades.data));
                    if (going_alone_skip_ind < 0)
                    {
                        center_cards.Add(Card.CreateSpecialCard(EuchreParameters.go_alone.data));

                        if (!screw_the_dealer || !CurrentPlayer().Equals(Dealer()))
                        {
                            center_cards.Add(Card.CreateSpecialCard(EuchreParameters.skip.data));
                        }
                    }
                }

                // Reset the card message value
                msg.center_action_cards = center_cards;
            }

            // Return the resulting message
            return msg;
        }
    }
}
