using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cardgames.core
{
    internal abstract class GameBase
    {
        public GameState state { get; protected set; }
        public Deck deck { get; protected set; }
        protected RuleEngine rules { get; set; }

        public abstract void StartGame(List<Player> players);
        protected abstract void PlayTurn();
        protected abstract void EndGame();
    }
}
