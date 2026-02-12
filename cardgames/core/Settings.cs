using cardgames.games.blackjack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cardgames.core
{
    internal static class Settings
    {
        public static bool Betting { get; private set; }
        public static List<Player> players { get; private set; } = [];

        public static void Setup()
        {
            
        }

    }
}
