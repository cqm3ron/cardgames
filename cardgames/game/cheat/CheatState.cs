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
    internal class CheatState(List<CheatPlayer> _players) : GameState<CheatPlayer>(_players)
    {
        private readonly Deck discard = new();
        private bool gameOver = false;

        public bool IsGameOver() => gameOver;
        public void GameOver() => gameOver = true;

        public void Discard(Card card)
        {
            discard.AddCard(card);
        }
        public int Discard(List<Card> cards)
        {
            discard.AddCards(cards);
            return cards.Count;
        }
        
        public static void Beep()
        {
            Console.Beep(1000, 250);
        }

        public void PrepareDeck()
        {
            Card.DeselectCards(gameDeck);
            Card.DeselectCards(discard);
        }

        public void PlayerPicksUpPile(Player playerToPickUp)
        {
            playerToPickUp.AddToHand(discard.Empty().ToList());
        }
    }
}
