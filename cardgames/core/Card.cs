using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Security;
using System.Text;
using System.Threading.Tasks;

namespace cardgames.core
{
    public enum Suit
    {
        Hearts,
        Diamonds,
        Clubs,
        Spades
    }
    public enum Rank
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
        public Suit suit { get; protected set; }
        public Rank rank { get; protected set; }
        public int Value => (int)rank;

        // Constructors
        public Card(Suit suit, Rank rank)
        {
            this.suit = suit;
            this.rank = rank;
        }

        public override string ToString()
        {
            return $"{rank} of {suit}";
        }

    }
}
