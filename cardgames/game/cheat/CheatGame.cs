using cardgames.core;
using static cardgames.core.Language;

namespace cardgames.game.cheat
{
    internal class CheatGame : GameBase<Player>
    {
        public const int DECKCOUNT = 6;
        private readonly decimal[] BETTING_AMOUNTS = [0.01m, 0.05m, 0.1m, 0.2m, 0.33m, 0.5m, 0.75m, 0.9m, 1m];
        public CheatState State { get; set; }
        public CheatGame() : base() { }
        public override List<Player> PlayGame(List<Player> players)
        {
            List<CheatPlayer> cheatPlayers = CheatPlayer.ConvertTo(players);

            State = new(cheatPlayers);

            // do good game 👍

            EndGame();

            Console.WriteLine(T("Util.PressKey"));
            Console.ReadKey(true);

            Language.UnloadGame(GameName);
            players = CheatPlayer.ConvertFrom(State.GetPlayerList());
            return players;
        }
        private protected override void PlayTurn()
        {

        }
        private protected override void EndGame()
        {

        }
    }
}
