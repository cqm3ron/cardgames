using cardgames.core;
using cardgames.game.cheat;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

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
            if (hovered)
            {
                Console.ForegroundColor = ConsoleColor.DarkBlue;
            }
            if (selected)
            {
                Console.ForegroundColor = ConsoleColor.Green;
            }
            if (hovered && selected)
            {
                Console.ForegroundColor = ConsoleColor.DarkCyan;
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
            else if (justDisplayTopLine &!faceUp)
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

            if (!faceUp &! justDisplayTopLine)
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
            Stack<Card>[] cardStacks = state.GetCardStacks();

            while (true)
            {
                state.HoverCardFromStack(currentStack, currentCardInStack);

                if (changesMade)
                {
                    Console.SetCursorPosition(0, 0);
                    if (drawSection) state.UnhoverCardFromStack(currentStack, currentCardInStack);
                    DisplayCardStacks(cardStacks, state.GetDeck(), drawSection, state.GetDrawnCards(), drawSectionLeft);
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
                        currentCardInStack = (currentCardInStack - 1);
                        if (currentCardInStack < 0) currentCardInStack = 0;
                        changesMade = true;
                    }
                    else if (inputKey.Key == ConsoleKey.DownArrow || inputKey.Key == ConsoleKey.S)
                    {
                        currentCardInStack = (currentCardInStack + 1);
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
                        state.ToggleSelectionOfNthCardFromStack(currentStack, currentCardInStack);
                    }
                    else
                    {
                        state.AddCardToDrawnCards(state.DrawCard());
                    }

                    changesMade = true;
                }
            }


        }

        public static void DisplayCardStacks(Stack<Card>[] cardStacks, Deck gameDeck, bool inDrawSection, Stack<Card> drawnCards, bool drawSectionLeft)
        {
            int x = Console.GetCursorPosition().Left;
            int y = Console.GetCursorPosition().Top;

            if (inDrawSection && drawSectionLeft) Console.ForegroundColor = ConsoleColor.DarkBlue;
            if (gameDeck.Count > 0) BigCard(gameDeck.GetTopCard());
            else
            {
                if (!drawSectionLeft) Console.ForegroundColor = ConsoleColor.DarkGray;
                BigCardOutline();
            }

            Util.ResetColor();

            Console.SetCursorPosition(x + CARD_WIDTH, y);

            if (inDrawSection &!drawSectionLeft)
            {
                Console.ForegroundColor = ConsoleColor.DarkBlue;
            }

            if (drawnCards.Count > 0) BigCard(drawnCards.Peek());
            else
            {
                if (drawSectionLeft) Console.ForegroundColor = ConsoleColor.DarkGray;
                BigCardOutline();
            }
            Util.ResetColor();

            if (gameDeck.Count == 0)
            {
                while (drawnCards.Count > 0)
                {
                    drawnCards.Peek().TurnFaceDown();
                    gameDeck.AddCard(drawnCards.Pop());
                }
            }

            y += CARD_HEIGHT + 1;

            foreach (Stack<Card> cardStack in cardStacks)
            {
                Console.SetCursorPosition(x, y);
                DisplayIndividualCardStack(cardStack);
                x = Console.GetCursorPosition().Left;
            }
            Console.SetCursorPosition(x - (4*CARD_WIDTH), y - CARD_HEIGHT - 1);

            Console.ForegroundColor = ConsoleColor.DarkGray;
            BigCardOutline(Suits.Hearts); // TODO: store the coords of these piles as variables; they shouldn't change
            BigCardOutline(Suits.Diamonds);
            BigCardOutline(Suits.Clubs);
            BigCardOutline(Suits.Spades);
            Util.ResetColor();
        }
        private static void DisplayIndividualCardStack(Stack<Card> cardStack)
        {
            int xCoord = Console.GetCursorPosition().Left;
            int yCoord = Console.GetCursorPosition().Top;
            for (int cardIndex = cardStack.Count - 1; cardIndex >= 0; cardIndex--)
            {
                Console.SetCursorPosition(xCoord, yCoord);
                
                if (cardIndex == 0)
                {
                    BigCard(cardStack.ElementAt(cardIndex));
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Gray;
                    BigCard(cardStack.ElementAt(cardIndex), true);
                    Util.ResetColor();
                }

                yCoord+=2;
            }
        }

        private static void DisplaySuitStacks()
        {

        }
    }
}
