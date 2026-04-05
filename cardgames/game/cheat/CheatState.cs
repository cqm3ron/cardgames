using cardgames.core;
using cardgames.game.blackjack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cardgames.game.cheat
{
    internal class CheatState : GameState<CheatPlayer>
    {
        public CheatState(List<CheatPlayer> _players) : base(_players)
        {

        }
    }
}
