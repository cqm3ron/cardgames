using cardgames.core;
using cardgames.game.blackjack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace cardgames.game.cheat
{
    internal class CheatState : GameState<CheatPlayer>
    {
        Deck discard = new Deck();

        public CheatState(List<CheatPlayer> _players) : base(_players)
        {
        }
    }
}
