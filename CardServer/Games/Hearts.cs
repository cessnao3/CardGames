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

        public Hearts(int game_id, GamePlayer[] players) : base(game_id: game_id, players: players)
        {
            deck = new Deck();
            HandSetup();
        }

        void HandSetup()
        {
            DealHand();
            pass_round_complete = true;
        }

        public override void Action(GamePlayer p, MsgGamePlay msg)
        {
            if (pass_round_complete)
            {
                if (p.Equals(CurrentPlayer()))
                {
                    if (msg.action == GameActions.CardPlay && hands[p].cards.Contains(msg.card))
                    {
                        // Reset the pool if all four cards have been played on the start of the next turn
                        if (center_pool.Count == 4)
                        {
                            center_pool.Clear();
                        }

                        // Play the respective card from the hands
                        hands[p].PlayCard(msg.card);
                        played_cards[p].Add(msg.card);
                        center_pool.Add(p, msg.card);
                        IncrementPlayer();
                    }
                }
            }
            else
            {

            }
        }

        private void DealHand()
        {
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
    }
}
