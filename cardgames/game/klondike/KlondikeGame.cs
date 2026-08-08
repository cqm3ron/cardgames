using cardgames.core;
using cardgames.game.cheat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cardgames.game.klondike
{
    internal class KlondikeGame : GameBase<Player>
    {
        public KlondikeState State { get; set; }
        public KlondikeGame() : base() { }

        public override List<Player> PlayGame(List<Player> _players)
        {
            List<KlondikePlayer> klondikePlayers = KlondikePlayer.ConvertTo(_players);

            State = new(klondikePlayers); // TODO: solitaire should be a singleplayer game only.

            State.SetupDeck(1); // Solitaire always uses one deck of cards.

            State.SetupCards();

            while (true)
            {
                KlondikeDisplay.DisplayKlondikeMenu(State);

                /* GAME PLAN:
                 * 1. allow user to traverse menu [X]
                 * 2. if they select a card, the game should move the card to the most sensible place [X]
                 * 3. the user should be able to choose this also; some kind of manual override? [x]
                 *      > there apparently can be more than two possible moves. this causes me great upset as i am now going to have to add a way to choose which of the possible moves to complete.
                 * 4. ensure the option to draw a new card exists (the pile thingy) [x]
                 * 5. HANDLE EVERYTHING IF EMPTY (IF EMPTY = DISPLAY CARD OUTLINE) [x]
                 * 6. AUTOSOLVE
                 * 7. DETECT WHEN IMPOSSIBLE / GAME IS LOST
                 * 8. ADD BETTING TO START OF GAME (BETTING ON ABILITY TO WIN IF I CANT COME UP WITH A BETTER OPTION)
                 */

                // TODO: ADD BETTING TO START OF GAME (BETTING ON SCORE MAYBE?)
                // TODO: SCORE MECHANICS


                // 1 - FUNCTIONAL

                // TODO: AUTO SOLVER
                // TODO: WIN DETECTION


                // 2 - NON-FUNCTIONAL

                // TODO: MOVE INPUT HANDLING LOGIC OUT OF DISPLAY CLASS - THE DISPLAY CLASS SHOULD ONLY BE RESPONSIBLE FOR DISPLAYING INFORMATION, NOT HANDLING INPUT. MOVE TO GAME CLASS.


                // 3 - BUG FIXES

                // TODO: display keybinding tooltips as it is not immediately obvious how to navigate menu (esp. to the draw pile)
            }

            Console.WriteLine("Util.PressKey");
            Console.ReadKey(true);

            return _players;
        }
        private protected override void PlayTurn()
        {

        }
        private protected override void EndGame()
        {

        }
    }
}
