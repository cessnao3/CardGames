using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;
using static CardGameLibrary.Cards.Card;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace CardGameLibrary.Cards
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
        [JsonInclude]
        public Suit CardSuit { get; set; }

        /// <summary>
        /// Defines the card's value
        /// </summary>
        [JsonInclude]
        public Value CardValue { get; set; }

        /// <summary>
        /// Sets the data parameter associated with a given card
        /// </summary>
        [JsonInclude]
        public int Data { get; set; }

        /// <summary>
        /// Initializes a card with a suit and value
        /// </summary>
        public Card()
        {
            CardSuit = Suit.Club;
            CardValue = Value.Two;
            Data = -1;
        }

        /// <summary>
        /// Initializes a card with a suit and value
        /// </summary>
        /// <param name="suit">Defines the card's suit</param>
        /// <param name="value">Defines the card's value</param>
        /// <param name="data">Defines any special data associated with the card</param>
        public Card(Suit suit, Value value, int data=-1)
        {
            CardSuit = suit;
            CardValue = value;
            Data = data;
        }

        /// <summary>
        /// Provides a relative card value based on the suit and card value
        /// </summary>
        /// <returns>An integer with a card value so that suits are separated</returns>
        public int CardValueSorting()
        {
            return ((int)CardSuit) * 32 + (int)CardValue;
        }

        /// <summary>
        /// Returns a hash code unique for the current card
        /// </summary>
        /// <returns>Hash code based on the card suit and value</returns>
        public override int GetHashCode()
        {
            return CardValueSorting().GetHashCode();
        }

        /// <summary>
        /// Determines if an object is equal to the card
        /// </summary>
        /// <param name="obj">Object to compare agains</param>
        /// <returns>true if the objects are equal</returns>
        public override bool Equals(object? obj)
        {
            if (obj is Card card)
            {
                return Equals(card);
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
            return CardSuit == c.CardSuit && CardValue == c.CardValue;
        }

        /// <summary>
        /// Provides the default comparison to sort by suit first, then order
        /// </summary>
        /// <param name="c1">The first card to compare</param>
        /// <param name="c2">The second card to compare</param>
        /// <returns>Equal if 0, c1 < c2 if < 0</returns>
        public static int DefaultComparison(Card c1, Card c2)
        {
            return c1.CardValueSorting() - c2.CardValueSorting();
        }

        /// <summary>
        /// Provides the string representation of the card
        /// </summary>
        /// <returns>String of the card name/value</returns>
        public override string ToString()
        {
            return $"{CardValue} of {CardSuit}s";
        }

        /// <summary>
        /// Determines if the card is a special card based on whether the data
        /// parameter is set
        /// </summary>
        /// <returns>True if the card is special and the data parameter is set</returns>
        public bool IsSpecial()
        {
            return Data >= 0;
        }

        /// <summary>
        /// Creates a new special card with the provided data
        /// </summary>
        /// <param name="data">The data parameter to use for setting the special card types</param>
        /// <returns></returns>
        static public Card CreateSpecialCard(int data)
        {
            // Create a new ca
            //
            // rset the data
            return new Card(
                suit: Suit.Club,
                value: Value.Two,
                data: data);
        }
    }
}
