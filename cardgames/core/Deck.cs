using static cardgames.core.Card;

namespace cardgames.core
{
    internal class Deck
    {
        protected static readonly Random rand = new();
        protected List<Card> cards = [];

        public Deck(int deckCount = 1)
        {
            cards.Clear();
            GenerateDeck(deckCount);
        }

        public int Count
        {
            get { return cards.Count; }
        }

        public List<Card> GetCards()
        {
            return cards;
        }

        public void Shuffle() // Fisher-Yates shuffle
        {
            int j;
            for (int i = 0; i < cards.Count - 2; i++)
            {
                Card temp;
                j = rand.Next(i, cards.Count);
                temp = cards[j];
                cards[j] = cards[i];
                cards[i] = temp;
            }
        }
        private List<Card> GenerateDeck()
        {
            List<Card> cards = [];
            for (int rank = 1; rank <= 13; rank++)
            {
                cards.Add(new Card((Rank)rank, Suit.heart));
                cards.Add(new Card((Rank)rank, Suit.diamond));
                cards.Add(new Card((Rank)rank, Suit.spade));
                cards.Add(new Card((Rank)rank, Suit.club));
            }
            return cards;
        }
        private void GenerateDeck(int n)
        {
            List<Card> cardsToAdd = [];
            for (int i = 0; i < n; i++)
            {
                cardsToAdd.AddRange(GenerateDeck());
            }
            this.cards = cardsToAdd;
        }
        public Card Draw()
        {
            Card card = this.cards[0];
            this.cards.RemoveAt(0);
            return card;
        }
        public Stack<Card> DrawMultiple(int count)
        {
            Stack<Card> cards = [];
            for (int i = 0; i < count; i++)
            {
                cards.Push(this.cards[0]);
                this.cards.RemoveAt(0);
            }
            return cards;
        }
    }
}
