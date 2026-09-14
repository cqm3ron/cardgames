using static cardgames.core.Language;

namespace cardgames.core
{
    internal abstract class GameBase<TPlayer> where TPlayer : Player
    {

        private protected GameBase()
        {
            //string stupidParsedString = this.GetType().ToString().Split('.')[^1].Replace("Game", ""); // this line was for experimenting to find how to correctly parse the game's name from its class path

            GameName = GetType().ToString().Split('.')[^1].Replace("Game", "");
        }

        public readonly string GameName;
        public int MIN_PLAYERS { get; private protected set; }
        public int MAX_PLAYERS { get; private protected set; }
        public abstract List<Player> PlayGame(List<TPlayer> players);
        private protected abstract void PlayTurn();
        private protected abstract void EndGame();
        public void LoadGame()
        {
            Util.StartLoading(T("Menu.Loading") + GameName);
            Language.LoadGame(GameName);
            Util.FinishLoading();
        }
    }
}
