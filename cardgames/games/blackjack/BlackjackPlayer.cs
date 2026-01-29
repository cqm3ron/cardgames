using cardgames.core;
using static cardgames.games.blackjack.BlackjackParser;

namespace cardgames.games.blackjack
{
    internal class BlackjackPlayer : Player
    {
        public BlackjackPlayer(string name) : base(name) { }

        public bool standing { get; private set; }
        public bool bust { get; private set; }
        private bool hasWon;
        private bool hasDrawn;
        private bool hasLost;

        public int GetHandValue()
        {
            int total = 0;
            int aces = 0;
            
            foreach (Card card in hand)
            {
                if (card.GetRank() == Card.Rank.ace)
                {
                    aces++;
                    total += 11;
                }
                else
                {
                    total += card.GetBlackjackValue();
                }
                
                while (total > 21 && aces > 0)
                {
                    total -= 10;
                    aces--;
                }
            }

            return total;
        }

        public void TakeTurn(BlackjackParser parser, Dealer dealer, Deck deck)
        {
            while (!standing && !bust)
            {
                Console.Clear();
                if (name.EndsWith('s')) Console.WriteLine($"{name}' turn");
                else Console.WriteLine($"{name}'s turn");
                Console.WriteLine($"Current hand value: {GetHandValue()}");
                Console.WriteLine("Your cards:");
                foreach (Card card in hand)
                {
                    Console.WriteLine(card);
                }
                Console.Write($"Dealer's card: {dealer.GetPublicCard()} (worth ");
                if (dealer.GetPublicCard().GetRank() == Card.Rank.ace)
                {
                    Console.WriteLine("1 or 11)");
                }
                else if (dealer.GetPublicCard().IsFaceCard())
                {
                    Console.WriteLine("10)");
                }
                else
                {
                    Console.WriteLine($"{dealer.GetPublicCard().GetBlackjackValue()})");
                }

                Choice choice;
                choice = GetChoice(parser);

                if (choice == Choice.hit)
                {
                    AddCardToHand(deck.Draw());
                    Console.WriteLine($"You drew: {hand[^1]}");
                    Console.WriteLine($"This brings your total to {GetHandValue()}");
                }
                else if (choice == Choice.stand)
                {
                    standing = true;
                    Console.WriteLine($"You chose to stand with a hand value of {GetHandValue()}.");
                }

                if (GetHandValue() > 21)
                {
                    bust = true;
                    Console.WriteLine("Uh-oh, you went bust! You are out of the game.");
                }
            }
            Console.Write("Press any key to move to the next player's turn: ");
            Console.ReadKey(true);
        }

        public Choice GetChoice(BlackjackParser parser)
        {
            string? input;
            bool successfulParse = false;
            Choice? choice = null;

            while (!successfulParse)
            {
                Console.Write("Hit or Stand? ");
                input = Console.ReadLine();
                successfulParse = parser.TryParseChoice(input ?? "", out choice);
                if (!successfulParse)
                {
                    Console.WriteLine("Invalid choice. Please enter 'Hit' or 'Stand'.");
                }
            }

            return choice.Value;
        }

        public void Win()
        {
            hasWon = true;
            hasDrawn = false;
            hasLost = false;
        }

        public void Draw()
        {
            hasWon = false;
            hasDrawn = true;
            hasLost = false;
        }

        public void Lose()
        {
            hasWon = false;
            hasDrawn = false;
            hasLost = true;
        }

        public string GetWinState()
        {
            if (hasWon)
            {
                return "Won";
            }
            else if (hasDrawn)
            {
                return "Drawn";
            }
            else if (hasLost)
            {
                return "Lost";
            }
            else return "something went wrong! You neither won, lost, nor drew!";
        }
    }
}
