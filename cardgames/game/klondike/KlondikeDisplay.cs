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

            if (justDisplayTopLine)
            {
                cardLines =
                [
                    "┌─────────┐",
                    "",
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

            if (!faceUp)
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
            int currentStack = 0;
            ConsoleKeyInfo inputKey;
            bool changesMade = true;
            Stack<Card>[] cardStacks = state.GetCardStacks();

            while (true)
            {
                state.HoverTopCardFromStack(currentStack);

                if (changesMade)
                {
                    Console.SetCursorPosition(0, 0);
                    if (drawSection) state.UnhoverTopCardFromStack(currentStack);
                    DisplayCardStacks(cardStacks, state.GetDeck(), drawSection, state.GetDrawnCards(), drawSectionLeft);
                    changesMade = false;
                }
                inputKey = Console.ReadKey(true);

                state.UnhoverTopCardFromStack(currentStack);
                if (!drawSection)
                {
                    if (Util.scrollRight.Contains(inputKey.Key) && currentStack < cardStacks.Length - 1)
                    {
                        currentStack++;
                        changesMade = true;
                    }
                    else if (Util.scrollLeft.Contains(inputKey.Key) && currentStack > 0)
                    {
                        currentStack--;
                        changesMade = true;
                    }
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
                else if (Util.affirmatives.Contains(inputKey.Key))
                {
                    if (!drawSection)
                    {
                        state.ToggleSelectionOfTopCardFromStack(currentStack);
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

                yCoord++;
            }
        }

        private static void DisplaySuitStacks()
        {

        }
    }
}
