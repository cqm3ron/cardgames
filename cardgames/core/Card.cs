using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Security;
using System.Text;
using System.Threading.Tasks;

namespace cardgames.core
{
    public enum Suits // TODO: add language support (sigh)
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
            this.Suit = suit;
            this.Rank = rank;
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
            return $"{Rank} of {Suit}"; // and language support here too (double sigh)
        }

    }
}
