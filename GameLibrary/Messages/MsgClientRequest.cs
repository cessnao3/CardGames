using System;
using System.Collections.Generic;
using System.Text;

namespace GameLibrary.Messages
{
    /// <summary>
    /// Defines a client message request
    /// </summary>
    public class MsgClientRequest : MsgBase
    {
        /// <summary>
        /// Request types available to the client
        /// </summary>
        public enum RequestType
        {
            AvailableGames = 0,
            GameStatus = 10,
            LobbyStatus = 20,
            NewLobby = 30,
            JoinLobby = 35,
            LeaveLobby = 36
        };

        /// <summary>
        /// The type of information to request from the client
        /// </summary>
        public RequestType request;
        
        /// <summary>
        /// The GameID to request information for
        /// </summary>
        public int game_id;

        /// <summary>
        /// A specific data parameter that may or may not be used by
        /// messages
        /// </summary>
        public int data = -1;

        /// <summary>
        /// Constructor to set the server response
        /// </summary>
        public MsgClientRequest() : base(MessageType.ClientRequest)
        {
            // Define an initial invalid game_id
            game_id = -1;
        }

        /// <summary>
        /// Checks whether the message type is valid
        /// </summary>
        /// <returns></returns>
        public override bool CheckMessage()
        {
            return msg_type == MessageType.ClientRequest;
        }
    }
}
