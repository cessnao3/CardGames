﻿using System;
using System.Collections.Generic;
using System.Text;
using CardGameLibrary.Cards;
using CardGameLibrary.GameParameters;
using CardGameLibrary.Messages;

namespace CardServer.Games
{
    public abstract class GenericTrickGame : GenericGame
    {
        /// <summary>
        /// Defines the current trick count
        /// </summary>
        protected int trick_count = 0;

        /// <summary>
        /// Message to store in a given round to provide as a game update to players
        /// </summary>
        string? TrickMessage { get; set; } = null;

        /// <summary>
        /// Game state to store the leading card in a trick
        /// </summary>
        protected Card? LoadCard { get; set; } = null;

        /// <summary>
        /// Constructs the generic trick-taking game class
        /// </summary>
        /// <param name="game_id">The game ID of the game</param>
        /// <param name="players">The players to add to the game</param>
        public GenericTrickGame(int game_id, GamePlayer[] players) : base(gameId: game_id, players: players)
        {
            // Empty Constructor
        }

        /// <summary>
        /// Deals the deck to the players
        /// </summary>
        protected override void ShuffleAndDeal()
        {
            // Call the base class value
            base.ShuffleAndDeal();

            // Reset game states
            trick_count = 0;
            LoadCard = null;
        }

        /// <summary>
        /// Finishes the trick after all cards have been played
        /// </summary>
        protected virtual void FinishTrick()
        {
            // Skip if the center pool isn't full
            if (!TrickCanBeCompleted()) throw new GameException(GameID, "Trick unable to be finished");

            // Increment the trick count
            trick_count += 1;
        }

        /// <summary>
        /// Determines the number of expected cards in a trick
        /// </summary>
        /// <returns>Returns the number of cards expected in a trick</returns>
        virtual protected int CountCenterCardExpected()
        {
            return Players.Length;
        }

        /// <summary>
        /// Determines whether the center pool of cards is full and the trick can be completed
        /// </summary>
        /// <returns></returns>
        protected bool TrickCanBeCompleted()
        {
            return PlayedCards.Count >= CountCenterCardExpected();
        }

        /// <summary>
        /// Sets the trick message. If the message is already set, does nothing
        /// </summary>
        /// <param name="msg">The new message to set as the trick message</param>
        protected void SetTrickMessage(string? msg = null)
        {
            if (!string.IsNullOrWhiteSpace(msg))
            {
                TrickMessage = msg;
            }
            else
            {
                TrickMessage = null;
            }
        }

        /// <summary>
        /// Provides the current game status message for the
        /// current game state
        /// </summary>
        /// <returns>A string with the current game status</returns>
        protected abstract string GetGameStatusMsg();

        /// <summary>
        /// If true, will append a the trick message to the responding
        /// status message
        /// </summary>
        /// <returns>True if the trick message should be appended</returns>
        protected abstract bool CanShowTrickMessage();

        /// <summary>
        /// Provides the current trick game status
        /// </summary>
        /// <param name="player">The player to get the current status for</param>
        /// <returns>The game status provided in a message that can be sent over the network</returns>
        override public MsgGameStatus GetGameStatus(GamePlayer player)
        {
            // Determine the base class game status
            MsgGameStatus msg = base.GetGameStatus(player: player);
            msg.CurrentGameStatus = GetGameStatusMsg();

            // Determine if the trick message should be appended
            if (CanShowTrickMessage() &&
                TrickMessage != null &&
                TrickMessage.Length > 0)
            {
                msg.CurrentGameStatus = $"{msg.CurrentGameStatus} - {TrickMessage}";
            }

            // Return the resulting message
            return msg;
        }
    }
}
