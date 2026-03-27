using static cardgames.core.Language;

namespace cardgames.core
{
    internal abstract class GameBase<TPlayer> where TPlayer : Player
    {

        private protected GameBase()
        {
            //string stupidParsedString = this.GetType().ToString().Split('.')[^1].Replace("Game", "");

            GameName = GetType().ToString().Split('.')[^1].Replace("Game", "");
        }

        private protected string GameName;
        //public GameState<TPlayer> State { get; protected set; }
        //private protected RuleEngine<TPlayer> Rules { get; set; }
        public abstract List<Player> PlayGame(List<TPlayer> players);
        private protected abstract void PlayTurn();
        private protected abstract void EndGame();
        private protected void LoadGame()
        {
            Util.StartLoading(T("Menu.Loading") + GameName);
            //Console.WriteLine(T("Menu.Loading") + GameName);
            Language.LoadGame(GameName);
            Util.FinishLoading();

        }
    }
}
