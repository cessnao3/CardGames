using System;
using System.Collections.Generic;
using System.Text;

namespace CardGameLibrary.Cards
{
    /// <summary>
    /// Provides a deck of cards that may be used 
    /// </summary>
    public class Deck
    {
        /// <summary>
        /// Defines the list of cards to play with
        /// </summary>
        public Card[] Cards { get; protected set; }

        /// <summary>
        /// Defines the next card to deal
        /// </summary>
        protected int CurrentCardIndex { get; set; } = 0;

        /// <summary>
        /// Provides random number generation for shuffling
        /// </summary>
        private static Random Rng { get; } = new Random();

        /// <summary>
        /// Property to return the number of cards in the deck
        /// </summary>
        public int Length
        {
            get { return Cards.Length; }
        }

        /// <summary>
        /// Provides a deck with only the cards with the value/suit combinations
        /// provided
        /// </summary>
        /// <param name="values">The cards to generate a deck with. If null, uses all available</param>
        /// <param name="suits">The suits to generate a deck with. If null, uses all available</param>
        public Deck(
            Card.Value[]? values = null,
            Card.Suit[]? suits = null)
        {
            // Create a list to add the cards to
            List<Card> cardList = new();

            // Replace suits or values if null
            suits ??= (Card.Suit[])Enum.GetValues(typeof(Card.Suit));
            values ??= (Card.Value[])Enum.GetValues(typeof(Card.Value));

            // Create the cards for the provided suits and values
            foreach (Card.Suit s in suits)
            {
                foreach (Card.Value v in values)
                {
                    cardList.Add(new Card(suit: s, value: v));
                }
            }

            // Append to an array
            Cards = cardList.ToArray();
        }

        /// <summary>
        /// Sets the deck to include only the cards provided
        /// </summary>
        /// <param name="cards">The cards to include in the deck</param>
        public Deck(Card[] cards)
        {
            // Simply set the card provided
            this.Cards = cards;
        }

        /// <summary>
        /// Determines if there are duplicate cards in the deck
        /// </summary>
        /// <returns>true if contains a duplicate</returns>
        public bool HasDuplicateCards()
        {
            // Check for any duplicates, return true if so
            for (int i = 0; i < Cards.Length; ++i)
            {
                for (int j = i + 1; j < Cards.Length; ++i)
                {
                    if (Cards[i] == Cards[j]) return true;
                }
            }

            // Otherwise, return false by default
            return false;
        }

        /// <summary>
        /// Shuffles the deck and resets the current card index to be
        /// able to re-deal the deck
        /// </summary>
        public void Shuffle()
        {
            // Check the length of the array
            int n = Cards.Length;

            // Perform a Fisher-Yates shuffle
            while (n > 1)
            {
                // Get the next random number in the sequence
                n--;
                int k = Rng.Next(n + 1);

                // Perform a swap
                (Cards[n], Cards[k]) = (Cards[k], Cards[n]);
            }

            // Reset the card index
            CurrentCardIndex = 0;
        }

        /// <summary>
        /// Determines if there is another available card to deal
        /// </summary>
        /// <returns>true if a card can be dealt</returns>
        public bool HasNext()
        {
            return CurrentCardIndex < Cards.Length;
        }

        /// <summary>
        /// Provides the next card in the deck if available
        /// </summary>
        /// <returns>next Card if available, otherwise null</returns>
        public Card? Next()
        {
            return HasNext() ? Cards[CurrentCardIndex++] : null;
        }
    }
}
