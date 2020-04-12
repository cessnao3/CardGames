using System;
using System.Collections.Generic;
using System.Text;
using GameLibrary.Cards;
using GameLibrary.Games;
using GameLibrary.Messages;

namespace CardServer.Games
{
    public class Hearts : GenericGame
    {
        bool pass_round_complete = false;
        bool first_round = false;
        bool hearts_broken = false;

        Card lead_card = null;
        static readonly Card start_card = new Card(
            suit: Card.Suit.Club,
            value: Card.Value.Two);

        string round_msg = null;

        Dictionary<GamePlayer, int> round_scores = new Dictionary<GamePlayer, int>();

        Dictionary<GamePlayer, List<Card>> passing_cards = new Dictionary<GamePlayer, List<Card>>();

        enum PassDirection
        {
            Left = 0,
            Right = 1,
            Across = 2,
            None = 3
        };

        public Hearts(int game_id, GamePlayer[] players) : base(game_id: game_id, players: players)
        {
            deck = new Deck();
            HandSetup();
        }

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

        void HandSetup()
        {
            DealHand();

            pass_round_complete = CurrentPassDirection() == PassDirection.None;
            first_round = true;
            hearts_broken = false;

            passing_cards.Clear();
            round_scores.Clear();
            foreach (GamePlayer p in players)
            {
                round_scores.Add(p, 0);
                passing_cards.Add(p, new List<Card>());
            }

            if (!pass_round_complete)
            {
                current_player_ind = -1;
            }
            else
            {
                DetermineStartingPlayer();
            }
        }

        void SetCurrentPlayer(GamePlayer p)
        {
            for (int i = 0; i < players.Length; ++i)
            {
                if (players[i].Equals(p))
                {
                    current_player_ind = i;
                    break;
                }
            }
        }

        void DetermineStartingPlayer()
        {
            foreach (GamePlayer p in players)
            {
                if (hands[p].HasCard(new Card(suit: Card.Suit.Club, value: Card.Value.Two)))
                {
                    SetCurrentPlayer(p);
                    break;
                }
            }
        }

        void FinishRound()
        {
            // Skip if the center pool isn't full
            if (center_pool.Count != 4) return;

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
                else if (c.suit == Card.Suit.Spade && c.value == Card.Value.Queen)
                {
                    points_to_add += 13;
                }

                // Determine the new "winning" player of the round
                if (c.suit == winning_card.suit && c.value >= winning_card.value)
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

                // Deal the new round
                round += 1;
                DealHand();
            }
        }

        public override void Action(GamePlayer p, MsgGamePlay msg)
        {
            // Ensure that we are playing a card and the current hand contains the given card
            if (msg.action != GameActions.CardPlay || !hands[p].cards.Contains(msg.card))
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
                        

                    // Check if the first round for the two of clubs
                    if (first_round && !msg.card.Equals(start_card))
                    {
                        card_valid = false;
                        round_msg = "Must lead the two of clubs";
                    }
                    // Check if trying to lead a heart before hearts has been broken
                    else if (
                        lead_card == null &&
                        msg.card.suit == Card.Suit.Heart &&
                        !hearts_broken &&
                        (h.HasCardOfSuit(Card.Suit.Club) || h.HasCardOfSuit(Card.Suit.Diamond) || h.HasCardOfSuit(Card.Suit.Spade)))
                    {
                        card_valid = false;
                        round_msg = "Hearts hasn't been broken yet!";
                    }
                    // Check if a player has a card of the lead suit before playing
                    else if (lead_card != null && lead_card.suit != msg.card.suit && h.HasCardOfSuit(lead_card.suit))
                    {
                        card_valid = false;
                        round_msg = "Must play a card of the lead suit";
                    }

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
                            FinishRound();
                        }
                        else
                        {
                            IncrementPlayer();
                        }

                        // Set the first round as false
                        first_round = false;

                        // Reset the round message
                        round_msg = null;
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

                    for (int i = 0; i < players.Length; ++i)
                    {
                        GamePlayer from_player = players[i];
                        GamePlayer to_player = players[(i + modifier) % players.Length];

                        foreach (Card c in passing_cards[from_player])
                        {
                            hands[to_player].AddCard(c);
                        }

                        hands[to_player].Sort();
                    }

                    pass_round_complete = true;
                    DetermineStartingPlayer();
                }
            }
        }

        private void DealHand()
        {
            foreach (GamePlayer p in players)
            {
                hands[p].Clear();
            }

            deck.Shuffle();
            while (deck.HasNext())
            {
                foreach (GamePlayer p in players)
                {
                    hands[p].AddCard(deck.Next());
                }
            }

            foreach (Hand h in hands.Values)
            {
                h.Sort();
            }
        }

        public override bool IsActive()
        {
            foreach (int s in OverallScores().Values)
            {
                if (s >= 100)
                {
                    return false;
                }
            }

            return true;
        }

        override public MsgGameStatus GetGameStatus()
        {
            MsgGameStatus msg = base.GetGameStatus();
            if (pass_round_complete)
            {
                msg.current_game_status = string.Format(
                    "{0:}'s Turn",
                    CurrentPlayer().CapitalizedName());

                if (round_msg != null && round_msg.Length > 0)
                {
                    msg.current_game_status += " - " + round_msg;
                }
            }
            else
            {
                msg.current_game_status = string.Format(
                    "Passing Cards {0:}",
                    Enum.GetName(typeof(PassDirection), CurrentPassDirection()));
            }

            return msg;
        }
    }
}
