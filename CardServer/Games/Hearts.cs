using System;
using System.Collections.Generic;
using System.Text;

namespace CardServer.Games
{
    class Hearts : GenericGame
    {
        public Hearts(Players.Player[] players) : base(players: players)
        {
            deck = new GameLibrary.Cards.Deck();
            HandSetup();
        }

        void HandSetup()
        {
            DealHand();
        }

        public void DealHand()
        {
            while (deck.HasNext())
            {
                foreach (Players.Player p in players)
                {
                    hands[p].AddCard(deck.Next());
                }
            }
        }
    }
}
