using System;
using System.Collections.Generic;
using System.Text;

namespace CardGameLibrary.GameParameters
{
    /// <summary>
    /// Enum for the game types supported
    /// </summary>
    public enum GameTypes
    {
        Invalid = 0,
        Hearts = 1,
        Euchre = 2
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
