using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cardgames.core
{
    internal abstract class RuleEngine
    {
        public abstract void Setup(GameState state);
        public abstract bool ValidateMove(GameState state);
        public abstract void ApplyMove(GameState state);
        public abstract bool CheckWin(GameState state);
    }
}
