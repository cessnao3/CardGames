﻿using System;
using System.Collections.Generic;
using System.Text;

namespace GameLibrary.Network
{
    /// <summary>
    /// Defines the actions that can be taken by players in a game
    /// </summary>
    public enum GameActions
    {
        CardPlay = 0,
        CardPass = 1
    };

    /// <summary>
    /// Class to facilitate communication within a game
    /// </summary>
    public class MsgGamePlay : MsgBase
    {
        /// <summary>
        /// Defines the game ID to use
        /// </summary>
        public string game_id = Guid.NewGuid().ToString();

        /// <summary>
        /// Defines the action to be performed by the game
        /// </summary>
        public GameActions action;

        /// <summary>
        /// Defines the card to play
        /// </summary>
        public Cards.Card card;

        /// <summary>
        /// Constructor to set the server response
        /// </summary>
        public MsgGamePlay()
        {
            msg_type = MessageType.GameMessage;
        }

        /// <summary>
        /// Checks whether message parameters are valid
        /// </summary>
        /// <returns>true if valid</returns>
        public override bool CheckMessage()
        {
            return
                game_id != null &&
                card != null &&
                msg_type == MessageType.GameMessage;
        }
    }
}