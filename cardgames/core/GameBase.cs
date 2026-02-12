using cardgames.core.parsers;

namespace cardgames.core
{
    public abstract class GameBase : IGame
    {
        protected bool Betting { get; set; }
        public abstract void Start();
        protected abstract void PlayRound();
        protected virtual void GameOver() { }
    }
}