using cardgames.core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cardgames.games.blackjack
{
    internal class BlackjackState : GameState
    {
        private Deck gameDeck;
        private List<Card> dealerHand;
        private List<Player> bustPlayers;
        private List<Player> standingPlayers;

        public BlackjackState(List<Player> _players) : base(_players) { }
    }
}
