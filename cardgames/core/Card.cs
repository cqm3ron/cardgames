using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cardgames.core
{
    internal class Card
    {
        public enum Suit { heart, diamond, spade, club }
        public enum Rank : int { ace = 1, two, three, four, five, six, seven, eight, nine, ten, jack, queen, king }

        private readonly Rank rank;
        private readonly Suit suit;
        private readonly bool isFace;
        public Card(Rank rank, Suit suit)
        {
            this.rank = rank;
            this.suit = suit;

            if (rank == Rank.jack || rank == Rank.queen || rank == Rank.king)
            {
                isFace = true;
            }
            else
            {
                isFace = false;
            }
        }

        private string ParseName()
        {
            string name;
            string rankName = rank switch
            {
                Rank.ace => "Ace",
                Rank.two => "Two",
                Rank.three => "Three",
                Rank.four => "Four",
                Rank.five => "Five",
                Rank.six => "Six",
                Rank.seven => "Seven",
                Rank.eight => "Eight",
                Rank.nine => "Nine",
                Rank.ten => "Ten",
                Rank.jack => "Jack",
                Rank.queen => "Queen",
                Rank.king => "King",
                _ => "Unknown"
            };

            string suitName = suit.ToString();
            suitName = char.ToUpper(suitName[0]) + suitName.Substring(1);
            suitName = suitName + "s";
            name = rankName + " of " + suitName;
            return name;
        }


        // Getters
        public string GetDisplayedRank()
        {
            return rank switch
            {
                Rank.ace => "A",
                Rank.two => "2",
                Rank.three => "3",
                Rank.four => "4",
                Rank.five => "5",
                Rank.six => "6",
                Rank.seven => "7",
                Rank.eight => "8",
                Rank.nine => "9",
                Rank.ten => "10",
                Rank.jack => "J",
                Rank.queen => "Q",
                Rank.king => "K",
                _ => "?"
            };
        }
        public char GetDisplayedSuit()
        {
            return suit switch
            {
                Suit.heart => '♥',
                Suit.diamond => '♦',
                Suit.spade => '♠',
                Suit.club => '♣',
                _ => '?'
            };
        }
        public Rank GetRank() => rank;
        public Suit GetSuit() => suit;
        public bool IsFaceCard() => isFace;
        public override string ToString() => ParseName();
        public string GetName() => ParseName();

        public int GetRankValue() { return (int)rank; }
        public int GetBlackjackValue()
        {
            return rank switch
            {
                Rank.ace => 11,
                Rank.jack => 10,
                Rank.queen => 10,
                Rank.king => 10,
                _ => (int)rank
            };
        }
    }
}
