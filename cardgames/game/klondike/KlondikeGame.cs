using cardgames.core;
using cardgames.game.cheat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static cardgames.core.Language;

namespace cardgames.game.klondike
{
    internal class KlondikeGame : GameBase<Player>
    {
        public KlondikeState State { get; set; }
        public KlondikeGame() : base()
        {
            MIN_PLAYERS = 1;
            MAX_PLAYERS = 1;
        }

        public override List<Player> PlayGame(List<Player> players)
        {
            List<KlondikePlayer> klondikePlayers = KlondikePlayer.ConvertTo(players);

            State = new(klondikePlayers);

            State.SetupDeck(1); // Solitaire always uses one deck of cards.

            State.SetupCards();

            Betting.BettingMenu(klondikePlayers);

            while (!State.GameOver)
            {
                KlondikeDisplay.DisplayKlondikeMenu(State);

                // TODO: ADD BETTING TO START OF GAME (BETTING ON SCORE MAYBE?)
                // TODO: SCORE MECHANICS


                // 1 - FUNCTIONAL

                // TODO: AUTO SOLVER
                // TODO: WIN DETECTION


                // 2 - NON-FUNCTIONAL

                // TODO: MOVE INPUT HANDLING LOGIC OUT OF DISPLAY CLASS - THE DISPLAY CLASS SHOULD ONLY BE RESPONSIBLE FOR DISPLAYING INFORMATION, NOT HANDLING INPUT. MOVE TO GAME OR STATE CLASS.

                
                // 3 - BUG FIXES

                // TODO: display keybinding tooltips as it is not immediately obvious how to navigate menu (esp. to the draw pile)
            }

            Util.PressAnyKey();

            players = KlondikePlayer.ConvertFrom(State.GetPlayerList());
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
