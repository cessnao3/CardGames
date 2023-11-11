using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

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
        [JsonInclude]
        public ActionType Action { get; private set; }

        /// <summary>
        /// Defines the username for the user
        /// </summary>
        [JsonInclude]
        public string Username { get; private set; }

        /// <summary>
        /// Defines the password hash for the user
        /// </summary>
        [JsonInclude]
        public string PasswordHash { get; private set; }

        /// <summary>
        /// Define the JSON empty constructor
        /// </summary>
        [JsonConstructor]
        public MsgLogin() : base(MessageTypeID.UserLogin)
        {
            Username = string.Empty;
            PasswordHash = string.Empty;
        }

        /// <summary>
        /// Constructor to set the server response
        /// </summary>
        public MsgLogin(ActionType action, string user, string password) : base(MessageTypeID.UserLogin)
        {
            Action = action;
            Username = user;
            PasswordHash = password;
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
