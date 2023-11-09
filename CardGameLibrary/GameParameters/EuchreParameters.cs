using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace CardGameLibrary.GameParameters
{
    public class EuchreParameters
    {
        public static readonly GameAction go_alone = new GameAction(
            name: "Go Alone",
            data: 10);

        public static readonly GameAction pickup_card = new GameAction(
            name: "Pickup Card",
            data: 12);

        public static readonly GameAction skip = new GameAction(
            name: "Skip",
            data: 14);
    }
}
