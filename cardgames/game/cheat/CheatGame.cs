using cardgames.core;
using System.Numerics;
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

            State.SetupDeck(DECKCOUNT);
            State.Deal(-1); // deal ALL cards out, not worrying about it being even

            int startingPlayerIndex = State.ChooseStartingPlayer();

            /*
             * PLAN
             * 1. give out hands [x]
             * 2. determine play order [ ]
             * 3. play starter card [ ]
             * 4. first player look; everyone else look away [ ]
             * 5. first player do they thang [ ]
             * 6. first player finish turn CLEAR SCREEN [ ]
             * 7. option to call cheat at any point? [ ]
             *     a. each player gets their own key to press to call cheat perhaps?
             *     b. or just a slower-paced game; display a timed window in which any player can call cheat
             *  8. handle cheat [ ] 
             *  9. next turn [ ]
             *  10. some kinda base case idk what [ ]
             */

            EndGame();

            Console.WriteLine(T("Util.PressKey"));
            Console.ReadKey(true);

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
