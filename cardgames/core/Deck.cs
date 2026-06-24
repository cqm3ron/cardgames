using cardgames.core.extension;

namespace cardgames.core
{
    internal class Deck
    {
        private protected Stack<Card> cards;
        private protected static readonly Random rand = new();

        public Deck()
        {
            cards = []; // generates an empty deck
        }

        public void AddCard(Card newCard) => cards.Push(newCard);

        public void AddCards(IEnumerable<Card> newCards)
        {
            foreach (Card card in newCards.ToList())
            {
                cards.Push(card);
            }
        }

        public void Shuffle() => cards.Shuffle(); // Calls the fisher-yates shuffle extension method for stacks (extensions/StackExtensions.cs)

        public Card Draw() => cards.Pop();

        public Card[,] Deal(int playerCount, int cardsPerPlayer = -1)
        {
            if (cardsPerPlayer == -1)
            {
                cardsPerPlayer = cards.Count / playerCount;
            }

            Card[,] hands = new Card[playerCount, cardsPerPlayer];

            for (int player = 0; player < playerCount; player++)
            {
                for (int card = 0; card < cardsPerPlayer; card++)
                {
                    hands[player, card] = Draw();
                }
            }

            return hands;

        }

        // Utility
        private Stack<Card> GenerateStandardDeck()
        {
            Stack<Card> newDeck = new Stack<Card>();

            foreach (Suits suit in Enum.GetValues<Suits>())
            {
                foreach (Ranks rank in Enum.GetValues<Ranks>())
                {
                    newDeck.Push(new Card(suit, rank));
                }
            }

            return newDeck;
        }
        public void AddStandardDecks(int count = 1)
        {
            for (int i = 0; i < count; i++)
            {
                AddCards(GenerateStandardDeck());
            }
        }

        public void RemoveCard(Card card)
        {
            cards.RemoveFirstOccurrence(card);
        }

        public Stack<Card> GetCards()
        {
            return cards;
        }

        public int Count => cards.Count;

        public Stack<Card> Empty()
        {
            return cards.Empty();
        }
    }
}
