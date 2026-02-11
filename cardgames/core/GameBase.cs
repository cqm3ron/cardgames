namespace cardgames.core
{
    public abstract class GameBase : IGame
    {
        public abstract void Start();
        protected abstract void PlayRound();
        protected virtual void GameOver() { }
    }
}