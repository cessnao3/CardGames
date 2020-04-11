using System;
using System.Collections.Generic;
using System.Text;
using GameLibrary.Cards;
using GameLibrary.Games;
using GameLibrary.Messages;

namespace CardServer.Games
{
    public abstract class GenericGame
    {
        public int game_id { get; protected set; }

        protected GamePlayer[] players;
        protected Deck deck;

        protected int current_player_ind;

        protected Dictionary<GamePlayer, List<int>> scores;
        protected Dictionary<GamePlayer, Hand> hands;

        protected int round = 0;

        protected List<Card> pool;
        protected Dictionary<GamePlayer, List<Card>> played_cards;

        public GenericGame(int game_id, GamePlayer[] players)
        {
            this.game_id = game_id;

            for (int i = 0; i < players.Length; ++i)
            {
                for (int j = i + 1; j < players.Length; ++j)
                {
                    if (players[i] == players[j])
                    {
                        throw new ArgumentException("Cannot have duplicate players in the same game");
                    }
                }
            }

            pool = new List<Card>();

            this.players = players;
            scores = new Dictionary<GamePlayer, List<int>>();
            hands = new Dictionary<GamePlayer, Hand>();
            played_cards = new Dictionary<GamePlayer, List<Card>>();

            foreach (GamePlayer p in this.players)
            {
                scores.Add(p, new List<int>());
                hands.Add(p, new Hand());
                played_cards.Add(p, new List<Card>());
            }
        }

        public abstract void Action(GamePlayer p, MsgGamePlay msg);

        public GamePlayer CurrentPlayer()
        {
            return players[current_player_ind];
        }

        public void IncrementPlayer()
        {
            current_player_ind = (current_player_ind + 1) % players.Length;
        }

        protected void SetPlayer(GamePlayer p)
        {
            for (int i = 0; i < players.Length; ++i)
            {
                if (p == players[i])
                {
                    current_player_ind = i;
                    return;
                }
            }

            throw new ArgumentException("Cannot set player who isn't in the game");
        }

        public Dictionary<GamePlayer, int> OverallScores()
        {
            Dictionary<GamePlayer, int> overall_scores = new Dictionary<GamePlayer, int>();

            foreach (GamePlayer p in players)
            {
                overall_scores[p] = 0;

                foreach (int s in scores[p])
                {
                    overall_scores[p] += s;
                }
            }

            return overall_scores;
        }

        virtual public MsgGameStatus GetGameStatus()
        {
            List<Hand> player_hands = new List<Hand>();

            foreach (GamePlayer p in players)
            {
                player_hands.Add(hands[p]);
            }

            MsgGameStatus status_val = new MsgGameStatus()
            {
                players = new List<GamePlayer>(players),
                hands = player_hands,
                current_game_status = "",
                current_player = current_player_ind,
                game_id = game_id
            };

            return status_val;
        }

        public abstract bool IsActive();
    }
}
