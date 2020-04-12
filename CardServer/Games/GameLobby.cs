using System;
using System.Collections.Generic;
using System.Text;
using GameLibrary.Games;
using GameLibrary.Messages;

namespace CardServer.Games
{
    public class GameLobby
    {
        List<GamePlayer> lobby_players = new List<GamePlayer>();

        GameTypes game_type;
        int game_id;
        DateTime create_time;

        public GameLobby(int game_id, GameTypes game_type)
        {
            this.game_id = game_id;
            this.game_type = game_type;

            for (int i = 0; i < 4; ++i)
            {
                lobby_players.Add(null);
            }

            create_time = DateTime.UtcNow;
        }

        public GenericGame CreateGame()
        {
            if (!LobbyReady())
            {
                return null;
            }
            else
            {
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

        public bool LobbyReady()
        {
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

        public bool Timeout()
        {
            return DateTime.UtcNow - create_time > TimeSpan.FromMinutes(15);
        }
    }
}
