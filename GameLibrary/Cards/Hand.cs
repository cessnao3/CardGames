using System;
using System.Collections.Generic;
using System.Text;

namespace GameLibrary.Cards
{
    /// <summary>
    /// Provides a hand that contains a card value
    /// </summary>
    public class Hand
    {
        public List<Card> cards { get; private set; }

        public Hand()
        {
            cards = new List<Card>();
        }

        public void Sort()
        {
            cards.Sort(Cards.Card.DefaultComparison);
        }

        public void AddCard(Card c)
        {
            cards.Add(c);
        }

        public void PlayCard(Card c)
        {
            if (!cards.Remove(c))
            {
                throw new ArgumentException("Cannot play a card that is not in the hand");
            }
        }
    }
}
