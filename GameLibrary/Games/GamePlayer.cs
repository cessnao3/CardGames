using System;
using System.Collections.Generic;
using System.Text;

namespace GameLibrary.Games
{
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
