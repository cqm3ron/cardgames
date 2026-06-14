using cardgames.core;
using System.Security.Cryptography;
using static cardgames.core.Language;

namespace cardgames.game.cheat
{
    internal class CheatDisplay
    {
        public const int CARD_WIDTH = 11; // public as it could be useful to know how much space to leave
        public const int CARD_HEIGHT = 7; // as above

        public static void DisplayScrollMenu(List<Card> cards, int currentCardIndex)
        {
            Console.Clear();
            Console.WriteLine(T("Cheat.Menu.Instructions"));
            Console.WriteLine(T("Cheat.Player.YourHand"));
            int x = Console.GetCursorPosition().Left;
            int y = Console.GetCursorPosition().Top;
            DisplayThreeCardView(cards, currentCardIndex);
            Console.SetCursorPosition(x + (2 * CARD_WIDTH) + 2 , y);
            SelectedCards(cards);
            Console.SetCursorPosition(x, y + CARD_HEIGHT + 1);
            OwnedCards(cards);
        }

        private static void DisplayThreeCardView(List<Card> cards, int currentCardIndex)
        {
            if (currentCardIndex < 0) currentCardIndex = 0; // ensure the index is within bounds
            else if (currentCardIndex >= cards.Count) currentCardIndex = cards.Count - 1; // ensure the index is within bounds

            Card middleCard = cards[currentCardIndex]; // get the card to display proud and centre
            Card? lastCard = null, nextCard = null;
            if (currentCardIndex > 0) lastCard = cards[currentCardIndex - 1]; // get the previous card to show a preview
            if (currentCardIndex < cards.Count - 1) nextCard = cards[currentCardIndex + 1]; // get the next card to show a preview 

            ShortCard(false, lastCard); // display a preview of the previous card on the left
            BigCard(middleCard); // display the current card
            Console.SetCursorPosition(Console.GetCursorPosition().Left + (CARD_WIDTH / 2) - (CARD_WIDTH / 2), Console.GetCursorPosition().Top + 1); // re-align the cursor
            ShortCard(true, nextCard); // display a preview of the next card on the right
        }
        private static void BigCard(Card card)
        {
            string rightRank, leftRank;
            string rank = card.GetRankSymbol();
            char suit = card.GetSuitSymbol();
            bool selected = card.IsSelected;

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
            if (selected)
            {
                Console.ForegroundColor = ConsoleColor.Green;
            }

            string[] cardLines =
            [
                "┌─────────┐",
                $"│{leftRank}       │",
                "│         │",
                $"│    {suit}    │",
                "│         │",
                $"│       {rightRank}│",
                "└─────────┘"
            ];

            int x = Console.GetCursorPosition().Left;
            int y = Console.GetCursorPosition().Top;

            if (selected) Console.ForegroundColor = ConsoleColor.Green;
            foreach (string line in cardLines)
            {
                Console.SetCursorPosition(x, y);
                Console.Write(line);
                y++;
            }
            Console.SetCursorPosition(x + CARD_WIDTH, y - CARD_HEIGHT);
            Util.ResetColor();
        }
        private static void ShortCard(bool rightSide, Card? card) // left = false; right = true. Easier than adding an enum & safer than passing a string.
        {
            if (card != null) // not empty card
            {
                string rank = card.GetRankSymbol();
                char suit = card.GetSuitSymbol();
                bool selected = card.IsSelected;
                if (selected) Console.ForegroundColor = ConsoleColor.Green;


                if (rightSide) // right side
                {
                    if (rank != "10") rank += " ";

                    string[] cardLines =
                    [
                        "─────┐",
                    "     │",
                    $"{suit}    │",
                    $"   {rank}│",
                    "─────┘"
                    ];

                    int x = Console.GetCursorPosition().Left;
                    int y = Console.GetCursorPosition().Top;

                    foreach (string line in cardLines)
                    {
                        Console.SetCursorPosition(x, y);
                        Console.Write(line);
                        y++;
                    }
                    Console.SetCursorPosition(x, y - (CARD_HEIGHT - 2));
                }
                else // left side
                {
                    if (rank != "10") rank = " " + rank;

                    string[] cardLines =
                    [
                        "┌─────",
                    $"│{rank}   ",
                    $"│    {suit}",
                    "│     ",
                    "└─────"
                    ];

                    int x = Console.GetCursorPosition().Left;
                    int y = Console.GetCursorPosition().Top + 1;

                    foreach (string line in cardLines)
                    {
                        Console.SetCursorPosition(x, y);
                        Console.Write(line);
                        y++;
                    }
                    Console.SetCursorPosition(x + (int)Math.Ceiling(CARD_WIDTH / 2.0f), y - (CARD_HEIGHT - 1));
                }

                Util.ResetColour();
            }
            else // empty card
            {
                int x = Console.GetCursorPosition().Left;
                int y = Console.GetCursorPosition().Top;
                for (int i = 0; i < CARD_HEIGHT - 2; i++)
                {
                    Console.SetCursorPosition(x, y);
                    Console.Write(new string(' ', (int)Math.Ceiling(CARD_WIDTH / 2.0f)));
                    y++;
                }
                Console.SetCursorPosition(x + (int)Math.Ceiling(CARD_WIDTH / 2.0f), y - (CARD_HEIGHT - 2));
            }
        }

        private static void SelectedCards(List<Card> cards)
        {
            Console.Write(T("Cheat.Cards.Selected") + ":");
            Console.SetCursorPosition(Console.GetCursorPosition().Left - T("Cheat.Cards.Selected").Length, Console.GetCursorPosition().Top + 1); // accounts for length of localised text
            List<Card> selectedCards = Card.GetSelectedCards(cards);
            foreach (Card card in selectedCards)
            {
                Console.Write(card);
                Console.SetCursorPosition(Console.GetCursorPosition().Left - card.ToString().Length, Console.GetCursorPosition().Top + 1);
            }
        }
        private static void OwnedCards(List<Card> cards)
        {
            Console.WriteLine(T("Cheat.Cards.Owned"));
            int[] cardCounter = new int[13];
            foreach (Card card in cards)
            {
                cardCounter[(int)card.Rank - 2]++;
            }

            for (int i = 0; i < 13; i++)
            {
                if (cardCounter[i] == 4)
                {
                    Console.ForegroundColor = ConsoleColor.Cyan;
                }
                else if (cardCounter[i] == 0)
                {
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                }
                Console.WriteLine($"{(Ranks)(i + 2)}: {cardCounter[i]}");
                Util.ResetColor();
            }
        }
    }
}