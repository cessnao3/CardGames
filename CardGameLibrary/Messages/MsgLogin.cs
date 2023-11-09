using System;
using System.Collections.Generic;

namespace CardGameLibrary.Messages
{
    /// <summary>
    /// Defines the game connection message for the client to connect to the server
    /// </summary>
    public class MsgLogin : MsgBase
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
        public ActionType Action { get; set; }

        /// <summary>
        /// Defines the username for the user
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// Defines the password hash for the user
        /// </summary>
        public string PasswordHash { get; set; }

        /// <summary>
        /// Constructor to set the server response
        /// </summary>
        public MsgLogin() : base(MessageTypeID.UserLogin)
        {
            // Empty Constructor
        }

        /// <summary>
        /// Checks whether message parameters are valid
        /// </summary>
        /// <returns>true if valid</returns>
        public override bool CheckMessage()
        {
            return
                Username != null &&
                PasswordHash != null &&
                MessageType == MessageTypeID.UserLogin;
        }
    }
}
