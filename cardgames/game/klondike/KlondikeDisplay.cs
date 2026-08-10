using cardgames.core;
using static cardgames.game.klondike.KlondikeState;
using static cardgames.core.Language;
using System.Transactions;
using System.Resources;
using System.Runtime.CompilerServices;

namespace cardgames.game.klondike
{
    internal static class KlondikeDisplay
    {
        public const int CARD_WIDTH = 11; // public as it could be useful to know how much space to leave
        public const int CARD_HEIGHT = 7; // as above

        private static int x, y, startX, startY;
        private static bool changesMade = true;
        private static bool totallyRedrawScreen = true;

        private static void BigCardOutline(Suits? suit = null)
        {
            int x = Console.GetCursorPosition().Left;
            int y = Console.GetCursorPosition().Top;
            string[] cardLines;
            if (suit == null)
            {
                cardLines =
                [
                    "┌─────────┐",
                    "│         │",
                    "│         │",
                    "│         │",
                    "│         │",
                    "│         │",
                    "└─────────┘"
                ];
            }
            else
            {
                cardLines =
                [
                    "┌─────────┐",
                    "│         │",
                    "│         │",
                    $"│    {Card.GetSuitSymbol((Suits)suit)}    │",
                    "│         │",
                    "│         │",
                    "└─────────┘"
                ];
            }

            foreach (string line in cardLines)
            {
                Console.SetCursorPosition(x, y);
                Console.Write(line);
                y++;
            }
            Console.SetCursorPosition(x + CARD_WIDTH, y - CARD_HEIGHT);
        }
        private static void BigCard(Card card, bool justDisplayTopLine = false, bool highlightAsTarget = false)
        {
            string rightRank, leftRank;
            string rank = card.GetRankSymbol();
            char suit = card.GetSuitSymbol();
            bool hovered = card.IsHovered;
            bool faceUp = card.IsFaceUp;
            string[] cardLines;

            if (rank == "10")
            {
                rightRank = rank;
                leftRank = rank;
            }
            else
            {
                rightRank = rank + " ";
                leftRank = " " + rank;
            }

            // Choose foreground color based on state
            if (card.IsRed && card.IsFaceUp)
            {
                Console.ForegroundColor = ConsoleColor.Red;
            }

            if (hovered)
            {
                Console.ForegroundColor = ConsoleColor.DarkBlue;
            }

            if (highlightAsTarget)
            {
                Console.ForegroundColor = ConsoleColor.Magenta;
            }

            if (justDisplayTopLine && faceUp)
            {
                cardLines =
                [
                    "┌─────────┐",
                    $"│{leftRank}  {suit}  {rightRank}│",
                    "",
                    "",
                    "",
                    "",
                    ""
                ];
            }
            else if (justDisplayTopLine & !faceUp)
            {
                cardLines =
                [
                    "┌─────────┐",
                    "│ ~~~~~~~ │",
                    "",
                    "",
                    "",
                    "",
                    ""
];
            }
            else
            {
                cardLines =
                [
                    "┌─────────┐",
                    $"│{leftRank}       │",
                    "│         │",
                    $"│    {suit}    │",
                    "│         │",
                    $"│       {rightRank}│",
                    "└─────────┘"
];
            }

            if (!faceUp & !justDisplayTopLine)
            {
                cardLines =
                [
                    "┌─────────┐",
                    "│ ~~~~~~~ │",
                    "│  *   *  │",
                    "│    *    │",
                    "│  *   *  │",
                    "│ ~~~~~~~ │",
                    "└─────────┘"
                ];
            }

            int x = Console.GetCursorPosition().Left;
            int y = Console.GetCursorPosition().Top;

            foreach (string line in cardLines)
            {
                Console.SetCursorPosition(x, y);
                Console.Write(line);
                y++;
            }
            Console.SetCursorPosition(x + CARD_WIDTH, y - CARD_HEIGHT);
            Util.ResetColor();
        }

        public static void DisplayKlondikeMenu(KlondikeState state)
        {
            Console.CursorVisible = false;

            if (changesMade || totallyRedrawScreen)
            {
                if (totallyRedrawScreen)
                {
                    Console.Clear();
                    totallyRedrawScreen = false;
                }
                else
                {
                    Console.SetCursorPosition(0, 0);
                }
                state.UpdateMoves();
                DisplayGameScreen(state);
                changesMade = false;
            }

            HandleInput(state);
        }

        private static void DisplayGameScreen(KlondikeState state)
        {
            startX = Console.GetCursorPosition().Left;
            startY = Console.GetCursorPosition().Top;
            x = Console.GetCursorPosition().Left;
            y = Console.GetCursorPosition().Top;

            DisplayDrawSection(state); // display the draw section in the top-left

            DisplayCardStacks(state); // display the card stacks in the centre of the screen

            DisplaySuitStacks(state); // display the suit stacks in the top-right

            DisplayInformation(state);

        }
        private static void DisplayDrawSection(KlondikeState state)
        {
            Deck gameDeck = state.GetDeck();
            Stack<Card> drawnCards = state.GetDrawnCards();

            if (state.IsInFaceDownDrawPile()) Console.ForegroundColor = ConsoleColor.DarkBlue;
            if (gameDeck.Count > 0)
            {
                if (!state.IsInFaceDownDrawPile()) Console.ForegroundColor = ConsoleColor.White;
                BigCard(gameDeck.GetTopCard());
            }
            else
            {
                if (!state.IsInFaceDownDrawPile()) Console.ForegroundColor = ConsoleColor.DarkGray;
                BigCardOutline();
            }

            Util.ResetColor();

            Console.SetCursorPosition(x + CARD_WIDTH, y);

            if (drawnCards.Count > 0) // if there are drawn cards, display the top one
            {
                if (state.IsInFaceUpDrawPile()) // if the drawn cards are hovered, highlight the top card
                {
                    drawnCards.Peek().Hover();
                }
                else // if the drawn cards are not hovered, unhighlight the top card
                {
                    drawnCards.Peek().Unhover();
                }

                BigCard(drawnCards.Peek()); // display the top card of the drawn cards
            }
            else // if there are no drawn cards, display an empty card outline
            {
                if (state.IsInFaceUpDrawPile()) Console.ForegroundColor = ConsoleColor.DarkBlue; // if the drawn cards are hovered, highlight the empty card outline
                else Console.ForegroundColor = ConsoleColor.DarkGray; // otherwise, display the empty card outline in dark grey
                BigCardOutline(); // display it
            }

            Util.ResetColor();

            y += CARD_HEIGHT + 1;
        }
        private static void DisplayCardStacks(KlondikeState state)
        {
            Stack<Card>[] cardStacks = state.GetCardStacks();
            List<KlondikeMove> moves = state.GetMoves();

            for (int i = 0; i < cardStacks.Length; i++)
            {
                Console.SetCursorPosition(x, y);

                bool isEmptyAndHovered = false; // TODO: remove
                bool displayTopCardAsTarget = false;

                if (state.IsInCardStacks() && cardStacks[i].Count == 0 && i == state.SelectedCardStack) isEmptyAndHovered = true;

                if (moves.Count > 0 && moves[state.SelectedMoveIndex].TargetIndex == i && moves[state.SelectedMoveIndex].Type == KlondikeMove.MoveType.ToCardStack) displayTopCardAsTarget = true;

                DisplayIndividualCardStack(cardStacks[i], isEmptyAndHovered, displayTopCardAsTarget);

                x = Console.GetCursorPosition().Left;
            }
            Console.SetCursorPosition(x - (4 * CARD_WIDTH), y - CARD_HEIGHT - 1);
        }
        private static void DisplaySuitStacks(KlondikeState state)
        {
            Suits[] suits = [Suits.Hearts, Suits.Diamonds, Suits.Clubs, Suits.Spades];
            List<KlondikeMove> moves = state.GetMoves();
            
            for (int i = 0; i < suits.Length; i++) // foreach suit
            {
                Suits suit = suits[i]; // set the suit
                Ranks? nextRank = state.GetNextRankForSuitStack(suit); // get the rank that is needed to add to this suit pile
                if (nextRank == Ranks.Ace) // if it's an ace
                {
                    Console.ForegroundColor = ConsoleColor.DarkGray; // set the foreground colour to dark grey (since we know it must be empty)
                    if (state.IsInSuitStacks() && i == state.SelectedSuitStack)
                    {
                        Console.ForegroundColor = ConsoleColor.DarkBlue;
                    }
                    else if (moves.Count > 0 && moves[state.SelectedMoveIndex].Type == KlondikeMove.MoveType.ToSuitStack && moves[state.SelectedMoveIndex].TargetIndex == i)
                    {
                        Console.ForegroundColor = ConsoleColor.Magenta;
                    }

                    BigCardOutline(suit);
                }
                else
                {
                    bool isAMoveOption = false;
                    if (moves.Count > 0 && moves[state.SelectedMoveIndex].Type == KlondikeMove.MoveType.ToSuitStack && moves[state.SelectedMoveIndex].TargetIndex == i) isAMoveOption = true;
                    BigCard(state.GetTopCardFromSuitStack(suit), highlightAsTarget:isAMoveOption);
                }
                Util.ResetColor();
            }
        }
        private static void DisplayInformation(KlondikeState state)
        {
            int score = state.Score;
            Console.SetCursorPosition(startX + (CARD_WIDTH * 2) + 1, startY);
            Console.Write(T("Klondike.Score"));
            Console.SetCursorPosition(startX + (CARD_WIDTH * 2) + 1, startY + 1);
            Console.Write(new String(' ', CARD_WIDTH - 1));
            Console.SetCursorPosition(startX + (CARD_WIDTH * 2) + 1, startY + 1);
            Console.Write(score);


            Console.SetCursorPosition(startX + (CARD_WIDTH * 7) + 1, startY);
            Console.Write(new String(' ', T("Klondike.Info.SelectMove").Length));
            Console.SetCursorPosition(startX + (CARD_WIDTH * 7) + 1, startY + 1);
            Console.Write(new String(' ', T("Klondike.SelectedMove").Length));
            Console.SetCursorPosition(startX + (CARD_WIDTH * 7) + 1, startY + 2);
            Console.Write(new String(' ', CARD_WIDTH - 1));

            if (state.GetMoves().Count > 1)
            {
                Console.SetCursorPosition(startX + (CARD_WIDTH * 7) + 1, startY);
                Console.Write(T("Klondike.Info.SelectMove"));
                Console.SetCursorPosition(startX + (CARD_WIDTH * 7) + 1, startY + 1);
                Console.Write(T("Klondike.SelectedMove"));
                Console.SetCursorPosition(startX + (CARD_WIDTH * 7) + 1, startY + 2);
                Console.Write($"{state.SelectedMoveIndex + 1}/{state.GetMoves().Count}");
            }

        }
        private static void DisplayIndividualCardStack(Stack<Card> cardStack, bool isEmptyAndHovered = false, bool displayTopCardAsTarget = false)
        {
            int xCoord = Console.GetCursorPosition().Left;
            int yCoord = Console.GetCursorPosition().Top;


            if (cardStack.Count == 0)
            {
                // if the stack is empty and hovered, set it to dark blue. If it is just empty, then dark grey.
                // if the stack is not emtpy and is hovered, this is already handled.
                Console.ForegroundColor = isEmptyAndHovered ? ConsoleColor.DarkBlue : ConsoleColor.DarkGray;
                
                
                if (displayTopCardAsTarget) Console.ForegroundColor = ConsoleColor.Magenta;
                BigCardOutline();
                Util.ResetColor();
                return;
            }

            for (int cardIndex = cardStack.Count - 1; cardIndex >= 0; cardIndex--) // start from the very bottom card (index 0 = top card)
            {
                Console.SetCursorPosition(xCoord, yCoord);

                if (cardIndex == 0) // if its the top card (index 0)
                {
                    BigCard(cardStack.ElementAt(cardIndex), highlightAsTarget: displayTopCardAsTarget); // display it normally, but highlight it if its the target of a move
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Gray;
                    BigCard(cardStack.ElementAt(cardIndex), true); // otherwise just display it "peeking" and slightly dimmer than the top card
                    Util.ResetColor();
                }

                yCoord += 2;
            }
        } // TODO: update to just use state maybe?

        private static void HandleInput(KlondikeState state)
        {
            ConsoleKeyInfo inputKey = Console.ReadKey(true);

            HandleDebugInput(state, inputKey);

            if (Util.affirmatives.Contains(inputKey.Key))
            {
                HandleConfirmationInput(state);
            }
            else if (state.CurrentLocation == Location.CardStacks)
            {
                HandleCardStacksInput(state, inputKey);
            }
            else if (state.CurrentLocation == Location.SuitStacks)
            {
                HandleSuitStacksInput(state, inputKey);
            }
            else if (state.CurrentLocation == Location.DrawPiles)
            {
                HandleDrawPilesInput(state, inputKey);
            }
        }

        private static void HandleCardStacksInput(KlondikeState state, ConsoleKeyInfo inputKey)
        {
            if (Util.scrollRight.Contains(inputKey.Key))
            {
                if (state.MoveStackSelectionRight()) changesMade = true;
            }
            else if (Util.scrollLeft.Contains(inputKey.Key))
            {
                if (state.MoveStackSelectionLeft()) changesMade = true;
            }
            else if (inputKey.Key == ConsoleKey.UpArrow || inputKey.Key == ConsoleKey.W)
            {
                if (state.MoveCardSelectionUp()) changesMade = true;
                //currentCardInStack = currentCardInStack - 1;
                //if (currentCardInStack < 0) currentCardInStack = 0;
                //changesMade = true;
            }
            else if (inputKey.Key == ConsoleKey.DownArrow || inputKey.Key == ConsoleKey.S)
            {
                if (state.MoveCardSelectionDown()) changesMade = true;
                //    currentCardInStack = currentCardInStack + 1;
                //    if (currentCardInStack > cardStacks[currentStack].Count - 1) currentCardInStack = cardStacks[currentStack].Count - 1;
                //    changesMade = true;
            }
            else if (inputKey.Key == ConsoleKey.PageUp)
            {
                if (state.MoveToDrawPile()) changesMade = true;
            }
            else if (inputKey.Key == ConsoleKey.Tab)
            {
                HandleMoveSelectionInput(state, inputKey);
            }
            //if (currentCardInStack > cardStacks[currentStack].Count - 1) currentCardInStack = cardStacks[currentStack].Count - 1;
            //else if (currentCardInStack < 0) currentCardInStack = 0;

        }
        private static void HandleDrawPilesInput(KlondikeState state, ConsoleKeyInfo inputKey)
        {
            if (Util.scrollLeft.Contains(inputKey.Key)) // TODO: move to suit stacks if right arrow pressed while in faceup draw piles
            {
                if (state.IsInFaceUpDrawPile())
                {
                    if (state.MoveToFaceDownDrawPile()) changesMade = true;
                }
            }
            else if (Util.scrollRight.Contains(inputKey.Key))
            {
                if (state.IsInFaceDownDrawPile())
                {
                    if (state.MoveToFaceUpDrawPile()) changesMade = true;
                }
                else if (state.IsInFaceUpDrawPile())
                {
                    if (state.MoveToSuitStacks()) changesMade = true;
                }
            }
            else if (inputKey.Key == ConsoleKey.PageDown)
            {
                if (state.MoveToCardStacks()) changesMade = true;
            }
            else if (inputKey.Key == ConsoleKey.Tab)
            {
                HandleMoveSelectionInput(state, inputKey);
            }
        }
        private static void HandleSuitStacksInput(KlondikeState state, ConsoleKeyInfo inputKey)
        {
            if (Util.scrollLeft.Contains(inputKey.Key))
            {
                if (state.IsInLeftmostSuitStack())
                {
                    if (state.MoveToFaceUpDrawPile()) changesMade = true;
                }
                else
                {
                    if (state.MoveSuitStackLeft()) changesMade = true;
                }
            }
            else if (Util.scrollRight.Contains(inputKey.Key))
            {
                if (state.MoveSuitStackRight()) changesMade = true;
            }
            else if (inputKey.Key == ConsoleKey.PageDown)
            {
                if (state.MoveToCardStacks()) changesMade = true;
            }
            else if (inputKey.Key == ConsoleKey.Tab)
            {
                HandleMoveSelectionInput(state, inputKey);
            }
        }
        private static void HandleMoveSelectionInput(KlondikeState state, ConsoleKeyInfo inputKey)
        {
            // TODO: INFORM USER HOW MANY OPTIONS AND TELL THEM HOW TO SELECT (E.G. "OPTION 1 OF 2; PRESS TAB TO CYCLE FORWARDS OR SHIFT-TAB TO CYCLE BACKWARDS.")
            if (inputKey.Key == ConsoleKey.Tab) // allow selecting which move you want to perform if multiple options
            {
                if (state.SelectNextMove()) changesMade = true;
            }
            else if (inputKey.Modifiers.HasFlag(ConsoleModifiers.Shift) && inputKey.Key == ConsoleKey.Tab)
            {
                if (state.SelectPreviousMove()) changesMade = true;

            }
        }
        private static void HandleConfirmationInput(KlondikeState state)
        {
            if (state.IsInCardStacks())
            {
                Card? currentCard = state.GetCurrentCard(); // get the currently selected card
                if (currentCard == null) return; // if there is no card selected, return
                if (currentCard.IsFaceUp) // otherwise, if face up, make the move if there is an available selected move.
                {
                    if (state.TryMakeSelectedMove()) totallyRedrawScreen = true;
                }

                else if (currentCard == state.GetTopCardFromStack(state.SelectedCardStack)) // if the card is face down and is the top card of the stack, turn it face up
                {
                    currentCard.TurnFaceUp();
                    state.ScoreTurnOverCardStacksCard();
                    changesMade = true;
                }
            }

            else if (state.IsInDrawPile())
            {
                Deck deck = state.GetDeck();
                Stack<Card> drawnCards = state.GetDrawnCards();
                if (state.IsInFaceDownDrawPile())
                {
                    if (deck.Count == 0 && drawnCards.Count > 0)
                    {
                        while (drawnCards.Count > 0)
                        {
                            Card card = drawnCards.Pop();
                            card.TurnFaceDown();
                            deck.AddCard(card);
                        }
                        state.MarkDrawPileAsRestocked();
                        changesMade = true;
                    }
                    else
                    {
                        Card drawn = state.DrawCard();
                        if (drawn != null) state.AddCardToDrawnCards(drawn);
                        changesMade = true;
                    }
                }
                else if (state.IsInFaceUpDrawPile())
                {
                    if (drawnCards.Count > 0)
                    {
                        if (state.TryMakeSelectedMove()) totallyRedrawScreen = true;
                    }
                }
            }

            else if (state.IsInSuitStacks())
            {
                Card? currentCard = state.GetCurrentCard();
                if (currentCard == null) return;
                if (state.TryMakeSelectedMove()) totallyRedrawScreen = true;
                state.HoverCurrentCard();
            }
        }
        private static void HandleDebugInput(KlondikeState state, ConsoleKeyInfo inputKey)
        {
            if (inputKey.Modifiers.HasFlag(ConsoleModifiers.Control) && inputKey.Modifiers.HasFlag(ConsoleModifiers.Alt)) // TODO: remove maybe? check if i can leave this in.
            {
                if (inputKey.Key == ConsoleKey.F7)
                {
                    state.ResetGame();
                    totallyRedrawScreen = true;
                }
                else if (inputKey.Key == ConsoleKey.F8)
                {
                    while (true)
                    {
                        int aceCounter = 0;
                        int targetAces = 4;
                        state.ResetGame();
                        for (int i = 0; i < 7; i++)
                        {
                            if (state.GetTopCardFromStack(i).Rank == Ranks.Ace) aceCounter++;
                        }
                        if (aceCounter == targetAces) break;
                    }
                }
                else if (inputKey.Key == ConsoleKey.S)
                {
                    // TODO: display moves to solve
                }
            }
        }
    }
}
