using static cardgames.core.Language;
using static cardgames.core.L10n;

namespace cardgames.core
{
    public enum Suits
    {
        Hearts,
        Diamonds,
        Clubs,
        Spades
    }
    public enum Ranks
    {
        Two = 2,
        Three,
        Four,
        Five,
        Six,
        Seven,
        Eight,
        Nine,
        Ten,
        Jack,
        Queen,
        King,
        Ace
    }
    internal class Card
    {
        public Suits Suit { get; private protected set; }
        public Ranks Rank { get; private protected set; }
        public bool IsFaceCard { get; private protected set; }
        public int Value => (int)Rank;

        // Constructors
        public Card(Suits suit, Ranks rank)
        {
            Suit = suit;
            Rank = rank;
            if (rank == Ranks.King || rank == Ranks.Queen || rank == Ranks.Jack)
            {
                IsFaceCard = true;
            }
            else
            {
                IsFaceCard = false;
            }
        }

        public override string ToString()
        {
            return $"{T($"Rank.{Rank}")} {T(Card_Of)} {T($"Suit.{Suit}")}";
        }

    }
}
