using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;

namespace CardGameLibrary.Messages
{
    /// <summary>
    /// Provides information on available game parameters
    /// </summary>
    public class MsgGameList : MsgBase
    {
        /// <summary>
        /// Defines the item parameters for each list
        /// </summary>
        public record ListItem(int GameIDValue, int GameType);

        /// <summary>
        /// Defines the list of lobbies, by ID, that may be joined
        /// </summary>
        [JsonInclude]
        public IReadOnlyList<ListItem> Lobbies { get; private set; }

        /// <summary>
        /// Defines the list of games, by ID, that may be played
        /// </summary>
        [JsonInclude]
        public IReadOnlyList<ListItem> Games { get; private set; }

        /// <summary>
        /// Define the parameterless constructor
        /// </summary>
        [JsonConstructor]
        public MsgGameList() : base(MessageTypeID.GameList)
        {
            Lobbies = Array.Empty<ListItem>();
            Games = Array.Empty<ListItem>();
        }

        /// <summary>
        /// Defines the game list message/response
        /// </summary>
        public MsgGameList(IEnumerable<ListItem> lobbies, IEnumerable<ListItem> games) : base(MessageTypeID.GameList)
        {
            Lobbies = lobbies.ToArray();
            Games = games.ToArray();
        }

        /// <summary>
        /// Determines if the message contains a valid message
        /// </summary>
        /// <returns>True if valid</returns>
        public override bool CheckMessage()
        {
            return
                MessageType == MessageTypeID.GameList &&
                Lobbies != null &&
                Games != null;
        }
    }
}
