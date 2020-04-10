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
        public string name { get; set; }
        public string password_hash { get; set; }

        private Player()
        {
            name = null;
            password_hash = null;
        }

        public Player(string name, string password_hash)
        {
            this.name = name;
            this.password_hash = password_hash;
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
