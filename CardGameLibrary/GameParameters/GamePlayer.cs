using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace CardGameLibrary.GameParameters
{
    /// <summary>
    /// Defines the properties for a specific game player
    /// </summary>
    public class GamePlayer
    {
        /// <summary>
        /// Defines the player user name
        /// </summary>
        [JsonInclude]
        public string Name { get; protected set; }

        /// <summary>
        /// Default parameterless constructor
        /// </summary>
        [JsonConstructor]
        public GamePlayer()
        {
            Name = "";
        }

        /// <summary>
        /// Standard constructor to provide the definition for a player object
        /// </summary>
        /// <param name="name">The player's user name</param>
        public GamePlayer(string name)
        {
            Name = name.ToLower().Trim();
        }

        /// <summary>
        /// Provides the capitalized name
        /// </summary>
        /// <returns>string of the capitalized name</returns>
        public string CapitalizedName
        {
            get
            {
                if (Name.Length > 1)
                {
                    return Name[..1].ToUpper() + Name[1..];
                }
                else
                {
                    return Name.ToUpper();
                }
            }
        }

        /// <summary>
        /// Provide a short three-character name
        /// </summary>
        /// <returns>The short name for the player</returns>
        public string ShortName { get => CapitalizedName[..Math.Min(3, Name.Length)]; }

        /// <summary>
        /// Determines if the object is equal to the player object
        /// </summary>
        /// <param name="obj">The object to compare to</param>
        /// <returns>True if equal</returns>
        public override bool Equals(object? obj)
        {
            if (obj is GamePlayer player)
            {
                return player.Name.ToLower() == Name.ToLower();
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
            return Name.GetHashCode();
        }
    }
}
