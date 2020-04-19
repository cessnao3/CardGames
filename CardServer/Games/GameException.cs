using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace CardServer.Games
{
    /// <summary>
    /// Provides an exception specific to game values
    /// </summary>
    public class GameException : Exception
    {
        /// <summary>
        /// The GameID to associate with the exception
        /// </summary>
        int game_id;

        /// <summary>
        /// Creates the GameException class with the provided message
        /// </summary>
        /// <param name="game_id">The Game ID of the exception</param>
        /// <param name="message">The exception message</param>
        public GameException(int game_id, string message) : base(message: message)
        {
            this.game_id = game_id;
        }

        /// <summary>
        /// Provides the message inforamtion for both the game ID and provided message
        /// </summary>
        public override string Message
        {
            get
            {
                return string.Format("Game ID {0:}: {1:}", game_id, base.Message);
            }

        }
    }
}
