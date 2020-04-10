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
        public string name { get; private set; }

        public Player(string name)
        {
            this.name = name;
        }

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

        public override int GetHashCode()
        {
            return this.name.GetHashCode();
        }

        public bool Equals(Player p)
        {
            return p.name == this.name;
        }
    }
}
