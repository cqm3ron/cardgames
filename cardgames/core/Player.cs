using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cardgames.core
{
    internal abstract class Player
    {
        public string Name { get; }
        public double Balance { get; set; } = 100.0;
        public List<Card> Hand { get; } = [];
        protected Player(string _name)
        {
            Name = _name;
        }

        public void AddCardToHand(Card card) => Hand.Add(card);

        public void AddCardsToHand(List<Card> cards) => Hand.AddRange(cards);
        
        public void RemoveCard(Card card) => Hand.Remove(card);
        
        public virtual void SortHand()
        {
            Hand.Sort((a, b) =>
            {
                int rankComparison = a.GetRank().CompareTo(b.GetRank());
                if (rankComparison != 0) return rankComparison;
                return a.GetSuit().CompareTo(b.GetSuit());
            });
        }
    }
}
