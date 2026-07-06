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
                 * 3. the user should be able to choose this also; some kind of manual override? [CHECK IF THERE CAN EVER BE MORE THAN TWO POSSIBLE MOVES]
                 *      > there apparently can be more than two possible moves. this causes me great upset as i am now going to have to add a way to choose which of the possible moves to complete.
                 * 4. ensure the option to draw a new card exists (the pile thingy)
                 * 5. HANDLE EVERYTHING IF EMPTY (IF EMPTY = DISPLAY CARD OUTLINE)
                 * 6. AUTOSOLVE
                 * 7. DETECT WHEN IMPOSSIBLE / GAME IS LOST
                 * 8. ADD BETTING TO START OF GAME (BETTING ON ABILITY TO WIN IF I CANT COME UP WITH A BETTER OPTION)
                 */

                // TODO LIST 

                // 1 - FUNCTIONAL

                // TODO: NUMBER KEYS 1-4 TO SELECT THE DECKS UP TOP (?)
                    // ALTERNATIVE OPTION CONSIDERED: pgup + arrow keys may be easier
                // TODO: AUTO SOLVER
                // TODO: WIN DETECTION
                // TODO: SELECTION LIMIT
                    // TODO: ? remove selection system entirely - may not be useful in this game ??
                        // |-> continue implementation and see if it still seems necessary by the end

                // MOVEMENT OF CARDS:
                // - A-Q:
                
                // -> for a card in the DRAW PILE
                // --> the card can move to max. 3 spaces:
                // ---> the relevant suit stack if it is the next needed card
                // ---> the first card with a difference of one rank & opposite colour in the card stacks
                // ---> the other card with a difference of one rank & opposite colour in the card stacks
                
                // -> for a card in the CARD STACKS
                // --> the card can move to max. 2 spaces
                // ---> the relevant suit stack if it is the next needed card 
                // ---> the other card with a diff. of one rank & opposite colour in the card stacks


                // - KINGS (SUCK)

                // -> for a card in the DRAW PILE
                // --> an empty space in the card stacks; HOWEVER, there is no reason the user would ever need to move it to a different empty space. could be added as non-functional
                // --> the top of the relevant suit stack, if it is the next needed card

                // -> for a card in the CARD STACKS
                // --> another empty space in the card stacks; would not be a useful move
                // --> top of relevant suit stack if next card needed


                // - POTENTIAL IMPLEMENTATIONS
                

                // -> if a card has multiple possible options; each option is assigned a number; the user is informed of these and told to press the relevant number key.
                // --> however this seems like a lot of work & janky to implement but probably the most user-friendly option
                
                // -> if a card has multiple options, left arrow key moves it to the closest option left and right arrow key moves to the closest option right.
                // --> this could mean that a right arrow key moves it back to stack index 0 due to rollover.
                
                // -> default option SPACE, alternative option ANOTHER KEY
                // --> only allows for two options; default option selection already programmed, then if there is another option it should just take moves[1]. 
                
                // -> could just have three keybinds --> affirmatives = default option, 2 = option 2, 3 = option 3, hardcoded;
                // --> user would need tooltips to tell them what each keybind would do at any given time
                // --> invalid keybinds should be ignored
                // --> this is probably the best option from a programming ease perspective

                // -> tab navigation
                // --> user selects card. space performs the move. pressing tab moves to the next possible move, highlighting that card. at any given point, the selected option is the default.
                // --> tooltip would still be needed to inform user of controls but controls would not change depending on number of cards. Shift tab to move back, tab to move to the next option
                // --> should be prioritised in logical order (i.e. moves[0], moves[1], moves[2] in that order.
                // --> this is probably the best option from a UX 




                // 2 - NON-FUNCTIONAL

                
                
                // 3 - BUG FIXES


                // TODO: bringing cards down from suit piles
                // TODO: prevent cards in the suit stacks from being selected
                // TODO: if card moved, scrolling up requires two presses. Change this to one                
                // TODO: fix bug where if you hover a card in the card stacks that has a valid move, then go to the draw pile and hover over the left side, the move still displays in magenta even though the source card is no longer selected.
                // TODO: display moves to suit piles in magenta (overlay currently only works for moving an Ace to an empty suit pile; make it highlight the top card from that stack in magenta regardless of card count in the given suit stack.
                // TODO: display keybinding tooltips as it is not immediately obvious how to navigate menu (esp. to the draw pile)
                // TODO: remove ability to place Kings on top of Aces (specific override in valid moves function is probably the easiest way to go about this)


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
