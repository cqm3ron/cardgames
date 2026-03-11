using cardgames.core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cardgames.games.blackjack
{
    internal class BlackjackDealer
    {
        public bool Bust { get; private set; }
        private List<Card> Hand { get; } = [];
        public Card PublicCard => Hand[0];
        public int HandValue => CalculateHandValue();

        public void AddCardToHand(Card card) => Hand.Add(card);
        private int CalculateHandValue()
        {
            int total = 0;
            int aces = 0;

            foreach (Card card in Hand)
            {
                if (card.Rank == Ranks.Ace)
                {
                    aces++;
                    total += 11;
                }
                else
                {
                    total += (card.IsFaceCard) switch
                    {
                        true => 10,
                        false => (int)card.Rank
                    };

                }


                while (total > 21 && aces > 0)
                {
                    total -= 10;
                    aces--;
                }

            }

            return total;
        }

        public void Play(Deck deck)
        {
            while (CalculateHandValue() < 17)
            {
                AddCardToHand(deck.Draw());
            }
            if (CalculateHandValue() > 21)
            {
                Bust = true;
            }
        }
    }
}
