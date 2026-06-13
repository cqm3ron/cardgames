using static cardgames.core.Language;

namespace cardgames.core
{
    public enum Suits : int
    {
        Hearts = 0x2665, // unicode chars for suit symbols
        Diamonds = 0x2666,
        Clubs = 0x2663,
        Spades = 0x2660
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
        public bool IsSelected = false; // some games need users to "select" cards, such as cheat. This can be used for such games.

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

        // Methods for games implementing standard text displays

        public override string ToString()
        {
            return $"{T($"Rank.{Rank}")} {T("Card.Of")} {T($"Suit.{Suit}")}";
        }

        // Methods for games implementing ascii art displays

        public char GetSuitSymbol()
        {
            return char.ConvertFromUtf32((int)Suit)[0];
        }

        public string GetRankSymbol()
        {
            if ((int)Rank <= 10)
            {
                return ((int)Rank).ToString();
            }

            switch (Rank)
            {
                case Ranks.Jack:
                    return "J";
                case Ranks.Queen:
                    return "Q";
                case Ranks.King:
                    return "K";
                case Ranks.Ace:
                    return "A";
                default:
                    return "?"; // should never happen
            }
        }

        // Static symbol-from-enum methods for games implementing ascii art displays
        public static char GetSuitSymbol(Suits suit) => char.ConvertFromUtf32((int)suit)[0];
        public static string GetRankSymbol(Ranks rank)
        {
            if ((int)rank <= 10)
            {
                return ((int)rank).ToString();
            }
            switch (rank)
            {
                case Ranks.Jack:
                    return "J";
                case Ranks.Queen:
                    return "Q";
                case Ranks.King:
                    return "K";
                case Ranks.Ace:
                    return "A";
                default:
                    return "?"; // should never happen
            }
        }


        // Methods for games implementing selection mechanics

        public void Select()
        {
            IsSelected = true;
        }
        public void Deselect()
        {
            IsSelected = false;
        }
        public void ToggleSelect()
        {
            IsSelected = !IsSelected;
        }

        // Static methods for games implementing selection mechanics
        public static int GetSelectedCardCount(List<Card> cards)
        {
            return cards.Count(c => c.IsSelected);
        }
        public static List<Card> GetSelectedCards(List<Card> cards)
        {
            return [.. cards.Where(c => c.IsSelected)];
        }
    }
}
