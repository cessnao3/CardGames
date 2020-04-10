using System;
using System.Collections.Generic;
using System.Text;

namespace GameLibrary.Network
{
    /// <summary>
    /// Defines the game connection message for the client to connect to the server
    /// </summary>
    public class GameConnectionMessage
    {
        /// <summary>
        /// Defines the types of actions that can be used
        /// </summary>
        public enum ActionType
        {
            NewUser = 0,
            LoginUser = 1
        };

        /// <summary>
        /// Defines the action request from the user
        /// </summary>
        public ActionType action;

        /// <summary>
        /// Defines the username for the user
        /// </summary>
        public string username;

        /// <summary>
        /// Defines the password hash for the user
        /// </summary>
        public string password_hash;
    }
}
