using System;
using System.Collections.Generic;
using System.Text;
using GameLibrary.Cards;

namespace CardServer.Games
{
    abstract class GenericGame
    {
        protected Players.Player[] players;
        protected Deck deck;

        protected int current_player_ind;

        protected Dictionary<Players.Player, List<int>> scores;
        protected Dictionary<Players.Player, Hand> hands;

        protected List<Card> pool;
        protected Dictionary<Players.Player, List<Card>> played_cards;

        public GenericGame(Players.Player[] players)
        {
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
        }

        public Players.Player CurrentPlayer()
        {
            return players[current_player_ind];
        }

        protected void SetPlayer(Players.Player p)
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

        public Dictionary<Players.Player, int> OverallScores()
        {
            Dictionary<Players.Player, int> overall_scores = new Dictionary<Players.Player, int>();

            foreach (Players.Player p in players)
            {
                overall_scores[p] = 0;

                foreach (int s in scores[p])
                {
                    overall_scores[p] += 1;
                }
            }

            return overall_scores;
        }
    }
}
