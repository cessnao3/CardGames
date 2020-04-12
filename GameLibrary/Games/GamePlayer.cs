using System;
using System.Collections.Generic;
using System.Text;

namespace GameLibrary.Games
{
    /// <summary>
    /// Defines the properties for a specific game player
    /// </summary>
    public class GamePlayer
    {
        /// <summary>
        /// Defines the player user name
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// Default parameterless constructor
        /// </summary>
        private GamePlayer()
        {
            name = null;
        }

        /// <summary>
        /// Standard constructor to provide the definition for a player object
        /// </summary>
        /// <param name="name">The player's user name</param>
        public GamePlayer(string name)
        {
            this.name = name.ToLower().Trim();
        }

        /// <summary>
        /// Provides the capitalized name
        /// </summary>
        /// <returns>string of the capitalized name</returns>
        public string CapitalizedName()
        {
            if (name.Length > 1)
            {
                return name.Substring(0, 1).ToUpper() + name.Substring(1, name.Length - 1);
            }
            else
            {
                return name.ToUpper();
            }
        }

        /// <summary>
        /// Determines if the object is equal to the player object
        /// </summary>
        /// <param name="obj">The object to compare to</param>
        /// <returns>True if equal</returns>
        public override bool Equals(object obj)
        {
            if (obj is GamePlayer)
            {
                return ((GamePlayer)obj).name.ToLower() == name.ToLower();
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Provide the hash code associated with the player name
        /// </summary>
        /// <returns>name hash code</returns>
        public override int GetHashCode()
        {
            return name.GetHashCode();
        }
    }
}
