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
        /// <summary>
        /// Defines the trump suit
        /// </summary>
        Card.Suit trump;

        /// <summary>
        /// Defines the value that is used for the bowers
        /// </summary>
        static readonly Card.Value bower_value = Card.Value.Jack;

        /// <summary>
        /// Defines the skip index that is used when a player is going alone
        /// </summary>
        int? GoingAloneSkipInd { get; set; }

        /// <summary>
        /// Determines if the bidding round has been completed
        /// </summary>
        bool BiddingComplete { get; set; }

        /// <summary>
        /// Determines if the first round of bidding has been complted
        /// </summary>
        bool FirstBidRoundComplete { get; set; }

        /// <summary>
        /// Determines which player called up trump
        /// </summary>
        GamePlayer? BiddingPlayer { get; set; }

        /// <summary>
        /// Boolean to allow for screwing the dealer
        /// </summary>
        bool ScrewTheDealer { get; } = true;

        /// <summary>
        /// Property to determine whether trump has been selected
        /// </summary>
        bool TrumpSelected
        {
            get { return BiddingPlayer != null; }
        }

        /// <summary>
        /// Storage location to determine the number of tricks taken by each player
        /// </summary>
        Dictionary<GamePlayer, int> TricksTaken { get; } = new();

        /// <summary>
        /// Determines the kitty card that is shown to all players for trump selection
        /// </summary>
        Card? KittyCard { get; set; } = null;

        /// <summary>
        /// Constructs a Euchre game instance
        /// </summary>
        /// <param name="gameId">The game ID of the game</param>
        /// <param name="players">The players to add to the game</param>
        public Euchre(int gameId, GamePlayer[] players) : base(game_id: gameId, players: players)
        {
            // Define the number of cards to deal
            CardDealLimit = 5;

            // Setup the Euchre deck
            Deck = new Deck(
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
                int effective_suit = (int)c.CardSuit;
                int effective_value = (int)c.CardValue;

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
                return c.CardValueSorting();
            }
        }

        /// <summary>
        /// Determines the number of expected cards in a trick
        /// </summary>
        /// <returns>Returns the number of cards expected in a trick</returns>
        override protected int CountCenterCardExpected()
        {
            return (GoingAloneSkipInd >= 0) ? 3 : 4;
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
            if (GoingAloneSkipInd == CurrentPlayerIndex)
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
            Card.Suit bower_suit = trump switch
            {
                Card.Suit.Club => Card.Suit.Spade,
                Card.Suit.Spade => Card.Suit.Club,
                Card.Suit.Diamond => Card.Suit.Heart,
                Card.Suit.Heart => Card.Suit.Diamond,
                _ => throw new GameException(
                    GameID,
                    $"Unknown trump suit {trump} provided"),
            };

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
            return Players[Round % Players.Length];
        }

        /// <summary>
        /// Sets up the game state for a new hand
        /// </summary>
        protected override void SetupNewRound()
        {
            // Deal the hand to players
            ShuffleAndDeal();

            // Setup the game state
            LoadCard = null;

            // Setup bidding states
            BiddingComplete = false;
            FirstBidRoundComplete = false;
            GoingAloneSkipInd = -1;
            BiddingPlayer = null;

            // Initialize trump
            trump = Card.Suit.Club;

            // Reset tricks so far
            TricksTaken.Clear();
            foreach (GamePlayer p in Players)
            {
                TricksTaken.Add(p, 0);
            }

            // Add a value to the center cards
            CenterCards.Clear();
            KittyCard = Deck?.Next() ?? throw new NullReferenceException(nameof(Deck));
            CenterCards.Add(KittyCard);

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
                return c.CardSuit;
            }
        }

        /// <summary>
        /// Provides the index of the partner player
        /// </summary>
        /// <param name="player_index">The current player's index</param>
        /// <returns>The partner player's index</returns>
        int PartnerPlayer(int player_index)
        {
            return (player_index + 2) % Players.Length;
        }

        /// <summary>
        /// Sorts players' hands
        /// </summary>
        void SortHands()
        {
            foreach (Hand hp in Hands.Values)
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
                    game_id: GameID,
                    message: "Player cannot perform an action until their turn");
            }

            if (BiddingComplete)
            {
                // Boolean to check if the provided card is valid
                bool card_valid = true;

                // Extract the current hand
                Hand h = Hands[p];

                // Perform checks for following suit
                if (LoadCard != null)
                {
                    // The player must play the lead suit if available
                    if (
                        EffectiveSuit(LoadCard) != EffectiveSuit(msg.Card) &&
                        h.HasCardWith(x => EffectiveSuit(x) == EffectiveSuit(LoadCard)))
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
                    LoadCard ??= msg.Card;

                    // Clear the last trick
                    if (TrickCanBeCompleted())
                    {
                        PlayedCards.Clear();
                    }

                    // Play the respective card from the hand
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
                }
            }
            else
            {
                // Determine if trump has been selected and the dealer must discard a card
                if (Dealer().Equals(p) &&
                    Hands[Dealer()].Cards.Count > 5 &&
                    TrumpSelected)
                {
                    // Discard the card that the dealer plays
                    // Set bidding to complete
                    if (Hands[Dealer()].HasCard(msg.Card))
                    {
                        Hands[Dealer()].PlayCard(msg.Card);
                        BiddingComplete = true;
                    }
                }

                // Only track special cards played for bidding-specific rounds
                if (msg.Card.IsSpecial())
                {
                    // Allow skipping from any 
                    if (msg.Card.Data == EuchreParameters.skip.data)
                    {
                        // Check if the dealer is the current player for defining parameters
                        if (Dealer().Equals(CurrentPlayer()))
                        {
                            if (!FirstBidRoundComplete)
                            {
                                // Set the first bidding round to complete and increment
                                FirstBidRoundComplete = true;
                                IncrementPlayer();
                            }
                            else if (!ScrewTheDealer)
                            {
                                // Re-deal the hand, with a new dealer
                                Round += 1;
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
                    if (!FirstBidRoundComplete)
                    {
                        // Allow the dealer to pickup the kitty-card
                        if (msg.Card.Data == EuchreParameters.pickup_card.data ||
                            msg.Card.Data == EuchreParameters.go_alone.data)
                        {
                            // Set trump as selected
                            BiddingPlayer = p;
                            trump = KittyCard?.CardSuit ?? throw new NullReferenceException(nameof(KittyCard));
                            CenterCards.Clear();

                            // Allow the player to go it alone
                            if (msg.Card.Data == EuchreParameters.go_alone.data)
                            {
                                GoingAloneSkipInd = PartnerPlayer(CurrentPlayerIndex);
                            }

                            // Set the dealer as the current player so they may discard
                            // However, if the dealer is selected as the going-alone player, don't make them pick up the card
                            if (GoingAloneSkipInd.HasValue && Players[GoingAloneSkipInd.Value].Equals(Dealer()))
                            {
                                BiddingComplete = true;
                            }
                            else
                            {
                                // Add the kitty-card to the dealer's hand
                                Hands[Dealer()].AddCard(KittyCard);
                                SetCurrentPlayer(Dealer());
                                SortHands();
                            }
                        }
                    }
                    // Perform steps for the second bidding round
                    else
                    {
                        // Allow players to select trump
                        if (msg.Card.Data == EuchreParameters.go_alone.data)
                        {
                            GoingAloneSkipInd = PartnerPlayer(CurrentPlayerIndex);
                        }
                        if (msg.Card.Data == GameAction.select_clubs.data)
                        {
                            BiddingPlayer = p;
                            BiddingComplete = true;
                            trump = Card.Suit.Club;
                        }
                        else if (msg.Card.Data == GameAction.select_diamonds.data)
                        {
                            BiddingPlayer = p;
                            BiddingComplete = true;
                            trump = Card.Suit.Diamond;
                        }
                        else if (msg.Card.Data == GameAction.select_hearts.data)
                        {
                            BiddingPlayer = p;
                            BiddingComplete = true;
                            trump = Card.Suit.Heart;
                        }
                        else if (msg.Card.Data == GameAction.select_spades.data)
                        {
                            BiddingPlayer = p;
                            BiddingComplete = true;
                            trump = Card.Suit.Spade;
                        }
                    }
                }

                // Perform any last-minute items before finishing the bidding round
                if (BiddingComplete)
                {
                    // Adjust player hands for going alone
                    if (GoingAloneSkipInd.HasValue)
                    {
                        Hands[Players[GoingAloneSkipInd.Value]].Cards.Clear();
                    }

                    // Sort each hand
                    SortHands();

                    // Reset the center cards
                    CenterCards.Clear();

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
            Card winningCard = LoadCard ?? throw new GameException(GameID, "unexpected null winning card found");
            GamePlayer? winningPlayer = null;

            foreach (var kvp in PlayedCards)
            {
                // Extract values
                GamePlayer p = kvp.Key;
                Card c = kvp.Value;

                // Check for points to add
                if ((EffectiveSuit(winningCard) == EffectiveSuit(c) && EuchreComparer(c, winningCard) >= 0) ||
                    (!IsTrumpCard(winningCard) && IsTrumpCard(c)))
                {
                    winningCard = c;
                    winningPlayer = p;
                }
            }

            // Add tricks taken so far
            TricksTaken[winningPlayer ?? throw new GameException(GameID, "unable to find winning player")] += 1;

            // Reset the lead card
            LoadCard = null;

            // Set the next player to the winning player
            SetCurrentPlayer(winningPlayer);

            // Finish the round if it is complete
            if (trick_count == 5)
            {
                // Add up tricks
                int ns_tricks = TricksTaken[Players[0]] + TricksTaken[Players[2]];
                int ew_tricks = TricksTaken[Players[1]] + TricksTaken[Players[3]];

                // Bidding Partnership
                int bidding_partnership = -1;
                for (int i = 0; i < Players.Length; ++i)
                {
                    if (BiddingPlayer?.Equals(Players[i]) ?? throw new GameException(GameID, "unable to find bidding player"))
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
                        if (GoingAloneSkipInd < 0)
                        {
                            points_to_add = 2;
                        }
                        else
                        {
                            points_to_add = 4;
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
                for (int i = 0; i < Players.Length; ++i)
                {
                    if (i % 2 == partnership_winning)
                    {
                        Scores[Players[i]].Add(points_to_add);
                    }
                    else
                    {
                        Scores[Players[i]].Add(0);
                    }
                }

                // Finish the round
                FinishRound();
                PlayedCards.Clear();
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
            else if (!BiddingComplete)
            {
                string response_str;

                if (TrumpSelected)
                {
                    response_str = $"Selected trump {trump}, {CurrentPlayer().ShortName} must discard a card";
                }
                else if (FirstBidRoundComplete)
                {
                    response_str = $"{CurrentPlayer().ShortName} may select trump";
                }
                else
                {
                    response_str = $"{CurrentPlayer().ShortName} may order-up the displayed card";
                }

                return $"{response_str} (Dealer {Dealer().ShortName})";
            }
            // Otherwise, provide the player's turn
            else
            {
                return $"{CurrentPlayer().ShortName}'s Turn (Dealer {Dealer().ShortName}, Trump {trump})";
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
            if (!BiddingComplete &&
                !TrumpSelected &&
                CurrentPlayer().Equals(player))
            {
                // Create  copy of the list
                List<Card> center_cards = new(msg.CenterActionCards);

                // Add an action card to both pass, go alone, and pick up the card
                if (!FirstBidRoundComplete)
                {
                    center_cards.Add(Card.CreateSpecialCard(EuchreParameters.go_alone.data));
                    center_cards.Add(Card.CreateSpecialCard(EuchreParameters.pickup_card.data));
                    center_cards.Add(Card.CreateSpecialCard(EuchreParameters.skip.data));
                }
                else
                {
                    if (KittyCard == null) throw new NullReferenceException(nameof(KittyCard));

                    if (KittyCard.CardSuit != Card.Suit.Club) center_cards.Add(Card.CreateSpecialCard(GameAction.select_clubs.data));
                    if (KittyCard.CardSuit != Card.Suit.Diamond) center_cards.Add(Card.CreateSpecialCard(GameAction.select_diamonds.data));
                    if (KittyCard.CardSuit != Card.Suit.Heart) center_cards.Add(Card.CreateSpecialCard(GameAction.select_hearts.data));
                    if (KittyCard.CardSuit != Card.Suit.Spade) center_cards.Add(Card.CreateSpecialCard(GameAction.select_spades.data));
                    if (GoingAloneSkipInd < 0)
                    {
                        center_cards.Add(Card.CreateSpecialCard(EuchreParameters.go_alone.data));

                        if (!ScrewTheDealer || !CurrentPlayer().Equals(Dealer()))
                        {
                            center_cards.Add(Card.CreateSpecialCard(EuchreParameters.skip.data));
                        }
                    }
                }

                // Reset the card message value
                msg.CenterActionCards = center_cards;
            }

            // Return the resulting message
            return msg;
        }
    }
}
