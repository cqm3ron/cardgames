using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cardgames.core
{
    internal abstract class Player
    {
        public string name { get; }
        public List<Card> hand { get; } = [];

        protected Player(string _name)
        {
            name = _name;
        }

        public void AddCardToHand(Card card) => hand.Add(card);

        public void AddCardsToHand(List<Card> cards) => hand.AddRange(cards);
        
        public void RemoveCard(Card card) => hand.Remove(card);
        
        public virtual void SortHand()
        {
            hand.Sort((a, b) =>
            {
                int rankComparison = a.GetRank().CompareTo(b.GetRank());
                if (rankComparison != 0) return rankComparison;
                return a.GetSuit().CompareTo(b.GetSuit());
            });
        }
    }
}
