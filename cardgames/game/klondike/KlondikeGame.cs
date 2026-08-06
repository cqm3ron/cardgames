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

            State = new(klondikePlayers);

            State.SetupDeck(1); // Solitaire always uses one deck of cards.

            State.SetupCards();

            while (true)
            {
                KlondikeDisplay.DisplayKlondikeMenu(State);


                /* GAME PLAN:
                 * 1. allow user to traverse menu
                 * 2. if they select a card, the game should move the card to the most sensible place
                 * 3. the user should be able to choose this also; some kind of manual override?
                 * 4. ensure the option to draw a new card exists (the pile thingy)
                 * 5. HANDLE EVERYTHING IF EMPTY (IF EMPTY = DISPLAY CARD OUTLINE)
                 * 6. AUTOSOLVE
                 * 7. DETECT WHEN IMPOSSIBLE / GAME IS LOST
                 * 8. ADD BETTING TO START OF GAME (BETTING ON ABILITY TO WIN IF I CANT COME UP WITH A BETTER OPTION)
                 */

                // TODO: LOOK AT REMOVING SELECTION FUNCTIONALITY
                // TODO: ADD BETTING TO START OF GAME (BETTING ON SCORE MAYBE?)
                // TODO: SCORE MECHANICS
                // TODO: AVOIDING UNCERTAINTY IN CARD MOVEMENT - ALWAYS BEING ABLE TO MAKE ALL POSSIBLE MOVEMENTS.
                // TODO: AUTO SOLVER
                // TODO: SELECTION LIMIT
                // TODO: ADJUST PRIORITIES: PRIORITISE MOVING TO SUIT STACKS OVER MOVING TO OTHER CARD STACKS
                // TODO: MOVE CARDS FROM ACE PILES
                // TODO: HIGHLIGHT MOVES TO ACE PILES
                // TODO: STOP HIGHLIGHTING FOR PREVIOUSLY SELECTED CARD IF MOVED TO DRAW PILE - LOOK AT GLOBAL POSITION VARIABLES IN STATE CLASS RATHER THAN MANAGING IN DISPLAY CLASS MAYBE?
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
