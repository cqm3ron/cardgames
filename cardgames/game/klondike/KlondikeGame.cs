using cardgames.core;
using cardgames.game.cheat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static cardgames.core.Language;

namespace cardgames.game.klondike
{
    internal class KlondikeGame : GameBase<Player>
    {
        public KlondikeState State { get; set; }
        public KlondikeGame() : base()
        {
            MIN_PLAYERS = 1;
            MAX_PLAYERS = 1;
        }

        public override List<Player> PlayGame(List<Player> players)
        {
            List<KlondikePlayer> klondikePlayers = KlondikePlayer.ConvertTo(players);

            State = new(klondikePlayers);

            State.SetupDeck(1); // Solitaire always uses one deck of cards.

            State.SetupCards();

            Betting.BettingMenu(klondikePlayers);

            while (!State.GameOver)
            {
                PlayTurn();
            }

            Util.PressAnyKey();
            Console.Clear();

            EndGame();

            Util.PressAnyKey();
            Console.Clear();

            players = KlondikePlayer.ConvertFrom(State.GetPlayerList());
            return players;
        }
        private protected override void PlayTurn()
        {
            KlondikeDisplay.DisplayKlondikeMenu(State);
        }
        private protected override void EndGame()
        {
            KlondikePlayer player = State.GetPlayerList()[0];
            Money bet = player.DeductBetFromBalance();
            Money betReturnPerCard = bet / 25; // 25 is a fixed value; adjust to change the house edge. This should make a solve return ~2x the player's bet
            Stack<Card>[] suitStacks = State.GetSuitStacks();

            int movedCards = 0;

            for (int stack = 0; stack < suitStacks.Length; stack++)
            {
                foreach (Card card in suitStacks[stack]) movedCards++;
            }

            Money betToReturn = betReturnPerCard * movedCards;
            player.AddToBalance(betToReturn);

            Console.WriteLine(T("Klondike.Score.Info", ("movedCards", movedCards.ToString()), ("moneyAddedToBalance", betToReturn.ToString()), ("balance", player.GetBalance().ToString()), ("bet", bet.ToString())));
        }
    }
}
