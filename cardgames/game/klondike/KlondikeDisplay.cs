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
        private static void BigCard(Card card, bool justDisplayTopLine = false)
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

            while (true)
            {
                state.HoverCardFromStack(currentStack, currentCardInStack);

                if (changesMade)
                {
                    Console.SetCursorPosition(0, 0);
                    if (drawSection) state.UnhoverCardFromStack(currentStack, currentCardInStack);
                    DisplayCardStacks(cardStacks, state.GetDeck(), drawSection, state.GetDrawnCards(), drawSectionLeft, state);
                    changesMade = false;
                }
                if (totallyRedrawScreen)
                {
                    Console.Clear();
                    if (drawSection) state.UnhoverCardFromStack(currentStack, currentCardInStack);
                    DisplayCardStacks(cardStacks, state.GetDeck(), drawSection, state.GetDrawnCards(), drawSectionLeft, state);
                    totallyRedrawScreen = false;
                    changesMade = false;
                }
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



                if (Util.affirmatives.Contains(inputKey.Key))
                {
                    if (!drawSection)
                    {
                        Card currentCard = state.GetNthCardFromStack(currentStack, currentCardInStack);
                        if (currentCard.IsFaceUp)
                        {
                            if (currentCard.Rank == state.GetNextRankForGivenSuit(currentCard.Suit)) // if the card is one of the next four that the player needs
                            {
                                state.MoveCardToSuitStack(currentCard, true);
                                currentCard.Unhover();
                                totallyRedrawScreen = true; // redraw screen completely to avoid any visual artifacts from the card being removed from the stack

                            }
                            else // default to selecting the card & cards above it
                            {
                                state.ToggleSelectionOfNthCardFromStack(currentStack, currentCardInStack);
                            }
                        }
                        else // if the card is face down, turn the card
                        {
                            if (currentCard == state.GetTopCardFromStack(currentStack)) // only allow turning the top card of a stack
                            {
                                state.GetNthCardFromStack(currentStack, currentCardInStack).TurnFaceUp();
                            }

                        }
                    }
                    else // TODO: implement logic for using the drawn cards, e.g. adding to the suit stacks. (next step!)
                    {
                        Deck deck = state.GetDeck();
                        Stack<Card> drawnCards = state.GetDrawnCards();

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

                    changesMade = true;
                }
            }


        }

        public static void DisplayCardStacks(Stack<Card>[] cardStacks, Deck gameDeck, bool inDrawSection, Stack<Card> drawnCards, bool drawSectionLeft, KlondikeState state)
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

            if (inDrawSection & !drawSectionLeft)
            {
                Console.ForegroundColor = ConsoleColor.DarkBlue;
            }

            if (drawnCards.Count > 0)
            {
                BigCard(drawnCards.Peek());
            }
            else
            {
                if (drawSectionLeft) Console.ForegroundColor = ConsoleColor.DarkGray;
                BigCardOutline();
            }
            Util.ResetColor();

            y += CARD_HEIGHT + 1;

            foreach (Stack<Card> cardStack in cardStacks)
            {
                Console.SetCursorPosition(x, y);

                DisplayIndividualCardStack(cardStack);
                x = Console.GetCursorPosition().Left;
            }
            Console.SetCursorPosition(x - (4 * CARD_WIDTH), y - CARD_HEIGHT - 1);

            

            foreach (Suits suit in Enum.GetValues<Suits>())
            {
                if (state.GetNextRankForGivenSuit(suit) == Ranks.Ace) // if no cards have yet been added to the suit stack, display the outline of the suit stack
                {
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    BigCardOutline(suit);
                }
                else
                {
                    BigCard(state.GetTopCardFromSuitStack(suit));
                }
                Util.ResetColor();
            }
        }
        private static void DisplayIndividualCardStack(Stack<Card> cardStack)
        {
            int xCoord = Console.GetCursorPosition().Left;
            int yCoord = Console.GetCursorPosition().Top;
            for (int cardIndex = cardStack.Count - 1; cardIndex >= 0; cardIndex--) // start from the very bottom card (index 0 = top card)
            {
                Console.SetCursorPosition(xCoord, yCoord);

                if (cardIndex == 0) // if its the top card (index 0)
                {
                    BigCard(cardStack.ElementAt(cardIndex)); // display it big
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

        private static void DisplaySuitStacks()
        {
            //implementation to show the four suit piles
        }
    }
}
