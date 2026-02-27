using cardgames.core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cardgames.games.blackjack
{
    internal class BlackjackRules : RuleEngine
    {
        public override void Setup(GameState state)
        {

        }

        public override bool ValidateMove(GameState state)
        {
            return true;
        }

        public override void ApplyMove(GameState state)
        {

        }

        public override bool CheckWin(GameState state)
        {
            return true;
        }
    }
}
