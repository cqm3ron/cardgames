using cardgames.core.extension;

namespace cardgames.core
{
    internal class Deck
    {
        private protected Stack<Card> cards;
        private protected Stack<Card> discardPile;
        private protected static readonly Random rand = new();

        public Deck()
        {
            cards = new();
            discardPile = new();
        }

        public void AddCards(IEnumerable<Card> newCards)
        {
            foreach (Card card in newCards.ToList())
            {
                cards.Push(card);
            }

        }
        public void Shuffle()
        {
            cards.Shuffle(); // Calls the fisher-yates shuffle extension method for stacks (extensions/StackExtensions.cs)
        }
        public Card Draw()
        {
            return cards.Pop();
        }

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

        public void AddToDiscard(Card card)
        {
            discardPile.Push(card);
        }
        public void RefillDeckFromDiscard()
        {
            while (discardPile.Count > 0)
            {
                cards.Push(discardPile.Pop());
            }
            Shuffle();
        }

        // Utility
        private Stack<Card> GenerateStandardDeck()
        {
            cards = new Stack<Card>();

            foreach (Suits suit in Enum.GetValues(typeof(Suits)))
            {
                foreach (Ranks rank in Enum.GetValues(typeof(Ranks)))
                {
                    cards.Push(new Card(suit, rank));
                }
            }

            return cards;
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
    }
}
