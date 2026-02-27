using cardgames.core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace cardgames.games.blackjack
{
    internal class BlackjackGame : GameBase
    {
        private const int DECKCOUNT = 6;

        public GameState state { get; set; }
        public Deck deck { get; set; }
        private RuleEngine rules { get; set; }

        public BlackjackGame()
        {

        }

        public override void StartGame(List<Player> players)
        {
            state = new(players);
            deck = new();
            deck.AddStandardDecks(DECKCOUNT);
            deck.Shuffle();
        }

        protected override void PlayTurn()
        {

        }

        protected override void EndGame()
        {

        }
    }
}
