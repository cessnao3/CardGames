using CardGameLibrary.Cards;
using System;
using System.Collections.Generic;
using System.Text;

namespace CardGameLibrary.GameParameters
{
    /// <summary>
    /// Defines an action that can be taken by the player
    /// </summary>
    public class GameAction
    {
        /// <summary>
        /// The data parameter to be used in selecting the 
        /// </summary>
        public int data { get; protected set; }

        /// <summary>
        /// The name associated with the action parameter
        /// </summary>
        public string name { get; protected set; }

        /// <summary>
        /// Creates an action parameter
        /// </summary>
        /// <param name="name"></param>
        /// <param name="data"></param>
        public GameAction(string name, int data)
        {
            this.data = data;
            this.name = name;
        }

        /// <summary>
        /// Defines the action to select the clubs suit
        /// </summary>
        public static readonly GameAction select_clubs = new GameAction(
            name: "Clubs",
            data: (int)Card.Suit.Club);

        /// <summary>
        /// Defines the action to select the diamonds suit
        /// </summary>
        public static readonly GameAction select_diamonds = new GameAction(
            name: "Diamonds",
            data: (int)Card.Suit.Diamond);

        /// <summary>
        /// Defines the action to select the hearts suit
        /// </summary>
        public static readonly GameAction select_hearts = new GameAction(
            name: "Hearts",
            data: (int)Card.Suit.Heart);

        /// <summary>
        /// Defines the action to select the spades suit
        /// </summary>
        public static readonly GameAction select_spades = new GameAction(
            name: "Spades",
            data: (int)Card.Suit.Spade);

        /// <summary>
        /// Defines the overall action database to be used for parsing card actions
        /// </summary>
        public static readonly Dictionary<int, GameAction> action_database = new Dictionary<int, GameAction>()
        {
            [select_clubs.data] = select_clubs,
            [select_diamonds.data] = select_diamonds,
            [select_hearts.data] = select_hearts,
            [select_spades.data] = select_spades,
            [EuchreParameters.go_alone.data] = EuchreParameters.go_alone,
            [EuchreParameters.pickup_card.data] = EuchreParameters.pickup_card,
            [EuchreParameters.skip.data] = EuchreParameters.skip,
        };
    }
}
