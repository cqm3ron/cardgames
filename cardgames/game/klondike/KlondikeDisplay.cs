using cardgames.core;
using System.Transactions;

namespace cardgames.game.klondike
{
    internal class KlondikeDisplay
    {
        public const int CARD_WIDTH = 11; // public as it could be useful to know how much space to leave
        public const int CARD_HEIGHT = 7; // as above

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
            bool selected = card.IsSelected;
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

            if (hovered && selected)
            {
                Console.ForegroundColor = ConsoleColor.DarkCyan;
            }
            else if (selected)
            {
                Console.ForegroundColor = ConsoleColor.Green;
            }
            else if (hovered)
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
            bool drawSection = false;
            bool drawSectionLeft = true;
            Console.CursorVisible = false;
            int currentStack = 0; // 0 leftmost stack; higher = further right.
            int currentCardInStack = 0; // 0 top; higher = deeper into stack.
            ConsoleKeyInfo inputKey;
            bool changesMade = true;
            bool totallyRedrawScreen = true;
            Stack<Card>[] cardStacks = state.GetCardStacks();
            List<(KlondikeState.MoveType, int)> moves;
            int selectedMoveIndex = 0;

            while (true)
            {
                state.HoverCardFromStack(currentStack, currentCardInStack);

                if (state.GetDrawnCards().Count > 0 && drawSection &! drawSectionLeft)
                {
                    moves = state.GetPermissableMoves(state.GetDrawnCards().Peek(), true);
                }
                else
                {
                    moves = state.GetPermissableMoves(state.GetNthCardFromStack(currentStack, currentCardInStack), false, currentStack, currentCardInStack);
                }

                if (changesMade)
                {
                    Console.SetCursorPosition(0, 0);
                    if (drawSection) state.UnhoverCardFromStack(currentStack, currentCardInStack);
                    DisplayCardStacks(cardStacks, state.GetDeck(), drawSection, state.GetDrawnCards(), drawSectionLeft, state, currentStack, moves, selectedMoveIndex);
                    changesMade = false;
                }
                if (totallyRedrawScreen)
                {
                    Console.Clear();
                    if (drawSection) state.UnhoverCardFromStack(currentStack, currentCardInStack);
                    DisplayCardStacks(cardStacks, state.GetDeck(), drawSection, state.GetDrawnCards(), drawSectionLeft, state, currentStack, moves, selectedMoveIndex);
                    totallyRedrawScreen = false;
                    changesMade = false;
                }

                // TODO: REMOVE TESTING BLOCK BELOW

                if (!drawSection)
                {
                    foreach (var x in state.GetPermissableMoves(state.GetHoveredCard(), false, currentStack, currentCardInStack))
                    {
                        Console.WriteLine(x.Item1 + ", " + x.Item2);
                    }
                }
                else
                {
                    foreach (var x in state.GetPermissableMoves(state.GetHoveredCard(), true))
                    {
                        Console.WriteLine(x.Item1 + ", " + x.Item2);
                    }
                }


                if (state.GetPermissableMoves(state.GetHoveredCard(), false, currentStack, currentCardInStack).Count == 0 && state.GetPermissableMoves(state.GetHoveredCard(), true).Count == 0) Console.WriteLine("                                             ");

                // END OF TESTING BLOCK

                inputKey = Console.ReadKey(true);

                state.UnhoverCardFromStack(currentStack, currentCardInStack);
                if (!drawSection)
                {
                    if (Util.scrollRight.Contains(inputKey.Key) && currentStack < cardStacks.Length - 1)
                    {
                        currentStack++;
                        currentCardInStack = cardStacks[currentStack].Count - 1;
                        changesMade = true;
                    }
                    else if (Util.scrollLeft.Contains(inputKey.Key) && currentStack > 0)
                    {
                        currentStack--;
                        currentCardInStack = cardStacks[currentStack].Count - 1;
                        changesMade = true;
                    }
                    else if (inputKey.Key == ConsoleKey.UpArrow || inputKey.Key == ConsoleKey.W)
                    {
                        currentCardInStack = currentCardInStack - 1;
                        if (currentCardInStack < 0) currentCardInStack = 0;
                        changesMade = true;
                    }
                    else if (inputKey.Key == ConsoleKey.DownArrow || inputKey.Key == ConsoleKey.S)
                    {
                        currentCardInStack = currentCardInStack + 1;
                        if (currentCardInStack > cardStacks[currentStack].Count - 1) currentCardInStack = cardStacks[currentStack].Count - 1;
                        changesMade = true;
                    }
                    if (currentCardInStack > cardStacks[currentStack].Count - 1) currentCardInStack = cardStacks[currentStack].Count - 1;
                    else if (currentCardInStack < 0) currentCardInStack = 0;

                }
                else
                {
                    if (Util.scrollRight.Contains(inputKey.Key))
                    {
                        drawSectionLeft = false;
                        changesMade = true;
                    }
                    else if (Util.scrollLeft.Contains(inputKey.Key))
                    {
                        drawSectionLeft = true;
                        changesMade = true;
                    }
                }

                if (inputKey.Key == ConsoleKey.PageUp)
                {
                    drawSection = true;
                    changesMade = true;
                }
                else if (inputKey.Key == ConsoleKey.PageDown)
                {
                    drawSection = false;
                    changesMade = true;
                }



                // TODO: INFORM USER HOW MANY OPTIONS AND TELL THEM HOW TO SELECT (E.G. "OPTION 1 OF 2; PRESS TAB TO CYCLE FORWARDS OR SHIFT-TAB TO CYCLE BACKWARDS.")
                if (inputKey.Key == ConsoleKey.Tab) // allow selecting which move you want to perform if multiple options
                {
                    if (moves.Count > 0)
                    {
                        selectedMoveIndex++;
                        if (selectedMoveIndex >= moves.Count) selectedMoveIndex = 0;
                        changesMade = true;
                    }
                }
                else if (inputKey.Modifiers.HasFlag(ConsoleModifiers.Shift) && inputKey.Key == ConsoleKey.Tab)
                {
                    if (moves.Count > 0)
                    {
                        selectedMoveIndex--;
                        if (selectedMoveIndex < 0) selectedMoveIndex = moves.Count - 1;
                        changesMade = true;
                    }
                }


                if (Util.affirmatives.Contains(inputKey.Key))
                {
                    if (!drawSection)
                    {
                        Card currentCard = state.GetNthCardFromStack(currentStack, currentCardInStack);

                        if (currentCard == null) continue;

                        if (currentCard.IsFaceUp)
                        {
                            if (moves.Count > 0)
                            {
                                state.MakeMove(moves[selectedMoveIndex].Item1, currentCard, moves[selectedMoveIndex].Item2, false, currentStack, currentCardInStack);
                                currentCard.Unhover();
                                totallyRedrawScreen = true;
                            }
                            else
                            {
                                state.ToggleSelectionOfNthCardAndAboveFromStack(currentStack, currentCardInStack);
                            }
                        }
                        else if (currentCard == state.GetTopCardFromStack(currentStack))
                        {
                            currentCard.TurnFaceUp();
                        }
                    }
                    else // TODO: permissable moves from draw section
                    {
                        Deck deck = state.GetDeck();
                        Stack<Card> drawnCards = state.GetDrawnCards();

                        if (drawSectionLeft)
                        {
                            if (deck.Count == 0 && drawnCards.Count > 0)
                            {
                                while (drawnCards.Count > 0)
                                {
                                    Card card = drawnCards.Pop();
                                    card.TurnFaceDown();
                                    deck.AddCard(card);
                                }
                            }
                            else
                            {
                                Card drawn = state.DrawCard();
                                if (drawn != null) state.AddCardToDrawnCards(drawn);
                            }
                        }
                        else
                        {
                            if (drawnCards.Count > 0)
                            {
                                Card currentCard = drawnCards.Peek();

                                if (moves.Count > 0)
                                {
                                    state.MakeMove(moves[selectedMoveIndex].Item1, currentCard, moves[selectedMoveIndex].Item2, true);
                                    currentCard.Unhover();
                                    totallyRedrawScreen = true;
                                }
                            }
                        }
                    }
                    
                    changesMade = true;
                }
            }


        }

        public static void DisplayCardStacks(Stack<Card>[] cardStacks, Deck gameDeck, bool inDrawSection, Stack<Card> drawnCards, bool drawSectionLeft, KlondikeState state, int currentStack, List<(KlondikeState.MoveType, int)> moves, int selectedMoveIndex)
        {
            int x = Console.GetCursorPosition().Left;
            int y = Console.GetCursorPosition().Top;

            if (inDrawSection && drawSectionLeft) Console.ForegroundColor = ConsoleColor.DarkBlue;
            if (gameDeck.Count > 0)
            {
                BigCard(gameDeck.GetTopCard());
            }
            else
            {
                if (!drawSectionLeft) Console.ForegroundColor = ConsoleColor.DarkGray;
                BigCardOutline();
            }

            Util.ResetColor();

            Console.SetCursorPosition(x + CARD_WIDTH, y);

            if (drawnCards.Count > 0)
            {
                if (inDrawSection & !drawSectionLeft)
                {
                    drawnCards.Peek().Hover();
                }
                else
                {
                    drawnCards.Peek().Unhover();
                }

                BigCard(drawnCards.Peek());
            }
            else
            {
                if (drawSectionLeft) Console.ForegroundColor = ConsoleColor.DarkGray;
                else if (inDrawSection &! drawSectionLeft) Console.ForegroundColor = ConsoleColor.DarkBlue;
                BigCardOutline();
            }
            Util.ResetColor();

            y += CARD_HEIGHT + 1;

            for (int i = 0; i < cardStacks.Length; i++)
            {
                Console.SetCursorPosition(x, y);

                bool isStackSelected = false;
                bool displayTopCardAsTarget = false;

                if (i == currentStack &! inDrawSection) isStackSelected = true;
                
                if (moves.Count > 0 && moves[selectedMoveIndex].Item2 == i && moves[selectedMoveIndex].Item1 == KlondikeState.MoveType.ToCardStack) displayTopCardAsTarget = true;

                DisplayIndividualCardStack(cardStacks[i], isStackSelected, displayTopCardAsTarget);

                x = Console.GetCursorPosition().Left;
            }
            Console.SetCursorPosition(x - (4 * CARD_WIDTH), y - CARD_HEIGHT - 1);



            Suits[] suits = [Suits.Hearts, Suits.Diamonds, Suits.Clubs, Suits.Spades];
            for (int i = 0; i < suits.Length; i++)
            {
                Suits suit = suits[i];

                if (state.GetNextRankForSuitStack(suit) == Ranks.Ace)
                {
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    if (moves.Count > 0 && moves[selectedMoveIndex].Item1 == KlondikeState.MoveType.ToSuitStack && moves[selectedMoveIndex].Item2 == i)
                    {
                        Console.ForegroundColor = ConsoleColor.Magenta;
                    }
                    BigCardOutline(suit);
                }
                else
                {
                    BigCard(state.GetTopCardFromSuitStack(suit));
                }
                Util.ResetColor();
            }
        }
        private static void DisplayIndividualCardStack(Stack<Card> cardStack, bool isSelected = false, bool displayTopCardAsTarget = false)
        {
            int xCoord = Console.GetCursorPosition().Left;
            int yCoord = Console.GetCursorPosition().Top;


            if (cardStack.Count == 0)
            {
                Console.ForegroundColor = isSelected ? ConsoleColor.DarkBlue : ConsoleColor.DarkGray; // if selected, dark blue, otherwise dark grey
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
                    BigCard(cardStack.ElementAt(cardIndex), highlightAsTarget:displayTopCardAsTarget); // display it big & magenta
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Gray;
                    BigCard(cardStack.ElementAt(cardIndex), true); // otherwise just display it "peeking" and slightly dimmer than the top card
                    Util.ResetColor();
                }

                yCoord += 2;
            }
        }
    }
}
