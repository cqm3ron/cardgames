using cardgames.core;
using System.Numerics;

namespace cardgames.games.blackjack
{
    internal class Dealer
    {
        public bool bust { get; private set; }
        private List<Card> hand { get; } = [];
        
        public void AddCardToHand(Card card) => hand.Add(card);
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
        public Card GetPublicCard()
        {
            return hand[0];
        }
        public void Draw(Deck deck)
        {
            while (GetHandValue() < 17)
            {
                AddCardToHand(deck.Draw());
            }

            if (GetHandValue() > 21)
            {
                bust = true;
            }
        }
    }
}
