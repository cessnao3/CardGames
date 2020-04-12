using System;
using System.Collections.Generic;
using System.Text;

namespace GameLibrary.Games
{
    /// <summary>
    /// Enum for the game types supported
    /// </summary>
    public enum GameTypes
    {
        Invalid = 0,
        Hearts = 1
    };

    /// <summary>
    /// Defines the lobby positions
    /// </summary>
    public enum LobbyPositions
    {
        Invalid = -1,
        North = 0,
        East = 1,
        South = 2,
        West = 3
    };
}
