using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text;
using System.Text.Json.Serialization;

namespace CardGameLibrary.Messages
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
        [JsonInclude]
        public RequestType Request { get; private set; }

        /// <summary>
        /// The GameID to request information for
        /// </summary>
        [JsonInclude]
        public int GameID { get; private set; }

        /// <summary>
        /// A specific data parameter that may or may not be used by
        /// messages
        /// </summary>
        [JsonInclude]
        public int Data { get; private set; }

        /// <summary>
        /// Constructor to set the server response
        /// </summary>
        [JsonConstructor]
        public MsgClientRequest() : base(MessageTypeID.ClientRequest)
        {
            // Define an initial invalid game_id
            GameID = -1;
            Data = -1;
        }

        /// <summary>
        /// Constructor to set server response
        /// </summary>
        /// <param name="request"></param>
        /// <param name="gameId"></param>
        /// <param name="data"></param>
        public MsgClientRequest(RequestType request, int gameId = -1, int data = -1) : base(MessageTypeID.ClientRequest)
        {
            Request = request;
            GameID = gameId;
            Data = data;
        }

        /// <summary>
        /// Checks whether the message type is valid
        /// </summary>
        /// <returns></returns>
        public override bool CheckMessage()
        {
            return MessageType == MessageTypeID.ClientRequest;
        }
    }
}
