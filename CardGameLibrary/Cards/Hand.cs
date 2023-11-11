using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace CardGameLibrary.Cards
{
    /// <summary>
    /// Provides a hand that contains a card value
    /// </summary>
    public class Hand
    {
        /// <summary>
        /// Defines the cards present in the player hand
        /// </summary>
        [JsonInclude]
        public List<Card> Cards { get; set; }

        /// <summary>
        /// Initializes an empty player hand
        /// </summary>
        public Hand()
        {
            Cards = new List<Card>();
        }

        /// <summary>
        /// Sorts the cards based on the default card comparison
        /// </summary>
        /// <param name="comparison">The comparison function to use in sorting the cards</param>
        public void Sort(Comparison<Card>? comparison = null)
        {
            Cards.Sort(comparison ?? Card.DefaultComparison);
        }

        /// <summary>
        /// Adds a card to the hand if not null
        /// </summary>
        /// <param name="c">The card to add to the hand</param>
        public void AddCard(Card c)
        {
            if (c == null) throw new ArgumentNullException("Card may not be null", nameof(c));
            Cards.Add(c);
        }

        /// <summary>
        /// Plays a card if present in the hand, throwing an error if the card doesn't exist in the player's hand
        /// </summary>
        /// <param name="c">The card to play/remove</param>
        public void PlayCard(Card c)
        {
            if (!Cards.Remove(c))
            {
                throw new ArgumentException("Cannot play a card that is not in the hand", nameof(c));
            }
        }

        /// <summary>
        /// Determines if the player has a card of the given suit
        /// </summary>
        /// <param name="suit">The suit to check</param>
        /// <returns>True if the player has a card of the given suit</returns>
        public bool HasCardOfSuit(Card.Suit suit)
        {
            return HasCardWith(x => x.CardSuit == suit);
        }

        /// <summary>
        /// Determines if the player has a card that matches the given predicate
        /// </summary>
        /// <param name="predicate">The predicate, of the form f(Card), to use</param>
        /// <returns>True if any of the provided cards return true</returns>
        public bool HasCardWith(Predicate<Card> predicate)
        {
            foreach (Card c in Cards)
            {
                if (predicate(c))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Determines if the player has a specific card
        /// </summary>
        /// <param name="card">The card to check</param>
        /// <returns>True if the player has the requested card</returns>
        public bool HasCard(Card card)
        {
            foreach (Card c in Cards)
            {
                if (c.Equals(card))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Clears the player's hand, resetting to an empty hand
        /// </summary>
        public void Clear()
        {
            Cards.Clear();
        }
    }
}
