using System;
using System.Collections.Generic;
using System.Text;
using GameLibrary.Games;
using GameLibrary.Messages;

namespace CardServer.Games
{
    /// <summary>
    /// Provides a game lobby status for creating a new game
    /// </summary>
    public class GameLobby
    {
        /// <summary>
        /// Stores the list of players present in the lobby
        /// </summary>
        List<GamePlayer> lobby_players = new List<GamePlayer>();

        /// <summary>
        /// Provides the game type of the lobby for creating a new game
        /// </summary>
        GameTypes game_type;

        /// <summary>
        /// Defines the game ID that the game will take when created
        /// </summary>
        int game_id;

        /// <summary>
        /// Determines the date time of the game for eventual timeout if a game isn't started
        /// </summary>
        DateTime create_time;

        /// <summary>
        /// Constructs a game lobby that will allow players to join/leave into different positions
        /// </summary>
        /// <param name="game_id">The game ID that the game will take</param>
        /// <param name="game_type">The game type for the lobby</param>
        public GameLobby(int game_id, GameTypes game_type)
        {
            // Set input parameters
            this.game_id = game_id;
            this.game_type = game_type;

            // Initialize the lobby players as initially empty
            for (int i = 0; i < 4; ++i)
            {
                lobby_players.Add(null);
            }

            // Setup the creation time
            create_time = DateTime.UtcNow;
        }

        /// <summary>
        /// Creates the resulting game from the lobby
        /// </summary>
        /// <returns>A new created game, with the lobby game_id, with lobby players</returns>
        public GenericGame CreateGame()
        {
            // Return nothing if the lobby isn't ready
            if (!LobbyReady())
            {
                return null;
            }
            // Otherwise, attempt to create the game
            else
            {
                // Create the game based on the game type
                // Otherwise, return null
                switch (game_type)
                {
                    case GameTypes.Hearts:
                        return new Hearts(
                            game_id: game_id,
                            players: lobby_players.ToArray());
                    default:
                        return null;
                }
            }
        }

        /// <summary>
        /// Check if the lobby is ready for the game to be created
        /// </summary>
        /// <returns>True if the lobby is able to be turned into a game</returns>
        public bool LobbyReady()
        {
            // Check if each player is provided/valid
            // If only one player isn't provided, the lobby will not be ready
            bool lobby_ready = true;
            foreach (GamePlayer p in lobby_players)
            {
                if (p == null)
                {
                    lobby_ready = false;
                }
            }
            return lobby_ready;
        }

        /// <summary>
        /// Allows a player to join a lobby at the given position. If the player already
        /// exists in the lobby, the player's position will be unchanged and will return false
        /// </summary>
        /// <param name="player">The player to add</param>
        /// <param name="pos">The position to add the player to</param>
        /// <returns>True if the player was successfully added</returns>
        public bool JoinLobby(GamePlayer player, LobbyPositions pos)
        {
            if (lobby_players.Contains(player))
            {
                return false;
            }
            else if (lobby_players[(int)pos] != null)
            {
                return false;
            }
            else
            {
                lobby_players[(int)pos] = player;
                return true;
            }
        }

        /// <summary>
        /// Allows a player to leave the lobby
        /// </summary>
        /// <param name="player">The player to remove from the lobby</param>
        /// <returns>True if the player could be successfully removed from the lobby</returns>
        public bool LeaveLobby(GamePlayer player)
        {
            for (int i = 0; i < lobby_players.Count; ++i)
            {
                if (player.Equals(lobby_players[i]))
                {
                    lobby_players[i] = null;
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Provides the lobby status message to send over the network
        /// </summary>
        /// <returns>The lobby status message to send to clients</returns>
        public MsgLobbyStatus GetLobbyStatus()
        {
            MsgLobbyStatus msg = new MsgLobbyStatus()
            {
                game_id = game_id,
                players = new List<GamePlayer>(lobby_players),
                game_type = game_type,
                lobby_ready = LobbyReady()
            };
            return msg;
        }

        /// <summary>
        /// Determines if the lobby has timed out and can be deleted
        /// </summary>
        /// <returns>True if the lobby has timed out (defualt 15 minutes)</returns>
        public bool Timeout()
        {
            return DateTime.UtcNow - create_time > TimeSpan.FromMinutes(15);
        }
    }
}
