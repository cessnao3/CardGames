using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CardGameLibrary.GameParameters;
using CardGameLibrary.Messages;

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
        List<GamePlayer?> LobbyPlayers { get; } = new();

        /// <summary>
        /// Provides the game type of the lobby for creating a new game
        /// </summary>
        public GameTypes GameType { get; protected set; }

        /// <summary>
        /// Defines the game ID that the game will take when created
        /// </summary>
        public int GameID { get; protected set; }

        /// <summary>
        /// Determines the date time of the game for eventual timeout if a game isn't started
        /// </summary>
        DateTime CreateTime { get; }

        /// <summary>
        /// Constructs a game lobby that will allow players to join/leave into different positions
        /// </summary>
        /// <param name="game_id">The game ID that the game will take</param>
        /// <param name="game_type">The game type for the lobby</param>
        public GameLobby(int game_id, GameTypes game_type)
        {
            // Set input parameters
            GameID = game_id;
            GameType = game_type;

            // Initialize the lobby players as initially empty
            for (int i = 0; i < 4; ++i)
            {
                LobbyPlayers.Add(null);
            }

            // Setup the creation time
            CreateTime = DateTime.UtcNow;
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
                throw new GameException(GameID, "Lobby isn't ready to create the game");
            }
            // Otherwise, attempt to create the game
            else
            {
                // Create the new non-null lobby players
                var lobbyPlayers = LobbyPlayers.Select(x => x ?? throw new NullReferenceException(nameof(LobbyPlayers))).OfType<GamePlayer>().ToList();

                // Create the game based on the game type
                // Otherwise, return null
                return GameType switch
                {
                    GameTypes.Hearts => new Hearts(
                        game_id: GameID,
                        players: lobbyPlayers.ToArray()),
                    GameTypes.Euchre => new Euchre(
                        gameId: GameID,
                        players: lobbyPlayers.ToArray()),
                    _ => throw new GameException(
                        GameID,
                        $"Cannot convert lobby to game for {GameType}")
                };
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
            foreach (GamePlayer? p in LobbyPlayers)
            {
                if (p == null)
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Checks if the lobby is empty or not
        /// </summary>
        /// <returns>True if the lobby is empty and all players are unset</returns>
        public bool IsEmpty()
        {
            // Iterate over each player to see if any are not null/set
            foreach (GamePlayer? p in LobbyPlayers)
            {
                if (p != null) return false;
            }
            return true;
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
            if (LobbyPlayers.Contains(player))
            {
                return false;
            }
            else if (LobbyPlayers[(int)pos] != null)
            {
                return false;
            }
            else
            {
                LobbyPlayers[(int)pos] = player;
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
            for (int i = 0; i < LobbyPlayers.Count; ++i)
            {
                if (player.Equals(LobbyPlayers[i]))
                {
                    LobbyPlayers[i] = null;
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
            return new MsgLobbyStatus(GameID, GameType, LobbyReady(), LobbyPlayers);
        }

        /// <summary>
        /// Determines if the lobby has timed out and can be deleted
        /// </summary>
        /// <returns>True if the lobby has timed out (defualt 15 minutes)</returns>
        public bool Timeout()
        {
            return DateTime.UtcNow - CreateTime > TimeSpan.FromMinutes(15);
        }
    }
}
