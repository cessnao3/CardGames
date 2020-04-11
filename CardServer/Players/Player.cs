using System;
using System.Collections.Generic;
using System.Text;

namespace CardServer.Players
{
    /// <summary>
    /// Provides the definition for a player object
    /// </summary>
    public class Player
    {
        /// <summary>
        /// Defines the player user name
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// Defines the default password hash
        /// </summary>
        public string password_hash { get; set; }

        /// <summary>
        /// Empty constructor for use in serialization
        /// </summary>
        private Player()
        {
            name = null;
            password_hash = null;
        }

        /// <summary>
        /// Standard constructor to provide the definition for a player object
        /// </summary>
        /// <param name="name">The player's user name</param>
        /// <param name="password_hash">The player's password hash</param>
        public Player(string name, string password_hash)
        {
            this.name = name.ToLower().Trim();
            this.password_hash = password_hash.ToLower().Trim();
        }

        /// <summary>
        /// Determines if the object is equal to the player object
        /// </summary>
        /// <param name="obj">The object to compare to</param>
        /// <returns>True if equal</returns>
        public override bool Equals(object obj)
        {
            if (obj is Player)
            {
                return Equals((Player)obj);
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Determines if two player objects are equal
        /// </summary>
        /// <param name="p">The other player object to compare agains</param>
        /// <returns>True if the player names are equal</returns>
        public bool Equals(Player p)
        {
            return p.name.ToLower() == name.ToLower();
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
