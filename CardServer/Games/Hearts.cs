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

        public Hearts(GamePlayer[] players) : base(players: players)
        {
            deck = new Deck();
            HandSetup();
        }

        void HandSetup()
        {
            DealHand();
            pass_round_complete = false;
        }

        public override void Action(GamePlayer p, MsgGamePlay msg)
        {
            if (pass_round_complete)
            {
                if (p == CurrentPlayer())
                {
                    if (msg.action == GameActions.CardPlay)
                    {
                        if (hands[p].cards.Contains(msg.card))
                        {
                            hands[p].PlayCard(msg.card);
                            played_cards[p].Add(msg.card);
                            pool.Add(msg.card);
                        }
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
