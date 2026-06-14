using cardgames.core;
using cardgames.game.blackjack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace cardgames.game.cheat
{
    internal class CheatState : GameState<CheatPlayer>
    {
        Deck discard = new();

        public CheatState(List<CheatPlayer> _players) : base(_players)
        {
        }

        public void Discard(Card card)
        {
            discard.AddCard(card);
        }
        public int Discard(List<Card> cards)
        {
            discard.AddCards(cards);
            return cards.Count;
        }
    }
}
