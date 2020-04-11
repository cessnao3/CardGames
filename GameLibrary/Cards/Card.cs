using System;
using System.Collections.Generic;
using System.Text;

namespace GameLibrary.Cards
{
    /// <summary>
    /// Defines a card instance that may be used in play
    /// </summary>
    public class Card
    {
        /// <summary>
        /// Defines the available card suits
        /// </summary>
        public enum Suit
        {
            Club = 1,
            Diamond = 2,
            Spade = 3,
            Heart = 4
        };

        /// <summary>
        /// Provides the available card values
        /// </summary>
        public enum Value
        {
            Two = 1,
            Three = 2,
            Four = 3,
            Five = 4,
            Six = 5,
            Seven = 6,
            Eight = 7,
            Nine = 8,
            Ten = 9,
            Jack = 10,
            Queen = 11,
            King = 12,
            Ace = 13
        };

        /// <summary>
        /// Defines the card's suit
        /// </summary>
        public Suit suit { get; protected set; }

        /// <summary>
        /// Defines the card's value
        /// </summary>
        public Value value { get; protected set; }

        /// <summary>
        /// Initializes a card with a suit and value
        /// </summary>
        /// <param name="suit">Defines the card's suit</param>
        /// <param name="value">Defines the card's value</param>
        public Card(Suit suit, Value value)
        {
            this.suit = suit;
            this.value = value;
        }

        /// <summary>
        /// Provides a relative card value based on the suit and card value
        /// </summary>
        /// <returns>An integer with a card value so that suits are separated</returns>
        protected int CardValue()
        {
            return ((int)suit) * 16 + (int)value;
        }

        /// <summary>
        /// Returns a hash code unique for the current card
        /// </summary>
        /// <returns>Hash code based on the card suit and value</returns>
        public override int GetHashCode()
        {
            return CardValue().GetHashCode();
        }

        /// <summary>
        /// Determines if an object is equal to the card
        /// </summary>
        /// <param name="obj">Object to compare agains</param>
        /// <returns>true if the objects are equal</returns>
        public override bool Equals(object obj)
        {
            if (obj is Card)
            {
                return Equals((Card)obj);
            }
            else
            {
                return base.Equals(obj);
            }
        }

        /// <summary>
        /// Determines if one card is equal to the current instance
        /// </summary>
        /// <param name="c">The other card to compare against</param>
        /// <returns>true if the card's have equal values and suit</returns>
        public bool Equals(Card c)
        {
            return suit == c.suit && value == c.value;
        }

        /// <summary>
        /// Provides the default comparison to sort by suit first, then order
        /// </summary>
        /// <param name="c1">the first card to compare</param>
        /// <param name="c2">the second card to compare</param>
        /// <returns>Equal if 0, c1 < c2 if < 0</returns>
        public static int DefaultComparison(Card c1, Card c2)
        {
            return c1.CardValue() - c2.CardValue();
        }

        public override string ToString()
        {
            return string.Format(
                "{0:s} of {1:s}s",
                value.ToString(),
                suit.ToString());
        }
    }
}
