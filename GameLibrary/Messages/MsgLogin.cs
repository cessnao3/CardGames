using System;
using System.Collections.Generic;

namespace GameLibrary.Messages
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
        public ActionType action;

        /// <summary>
        /// Defines the username for the user
        /// </summary>
        public string username;

        /// <summary>
        /// Defines the password hash for the user
        /// </summary>
        public string password_hash;

        /// <summary>
        /// Constructor to set the server response
        /// </summary>
        public MsgLogin() : base(MessageType.UserLogin)
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
                username != null &&
                password_hash != null &&
                msg_type == MessageType.UserLogin;
        }
    }
}
