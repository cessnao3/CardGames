using System;
using System.Collections.Generic;
using System.Text;

namespace GameLibrary.Network
{
    public enum GameActions
    {
        Play = 0,
        Pass = 1
    };

    public class GamePlayMessage
    {
        public string game_id = Guid.NewGuid().ToString();
        public GameActions action;
        public Cards.Card card;
    }
}
