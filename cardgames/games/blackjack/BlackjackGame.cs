using cardgames.core;
using cardgames.core.parsers;

namespace cardgames.games.blackjack
{
    internal class BlackjackGame : GameBase
    {
        private const int DECKCOUNT = 6;
        private Deck gameDeck;
        private readonly BlackjackParser parser = new();
        private readonly List<BlackjackPlayer> players = [];
        private readonly Dealer dealer = new();

        public BlackjackGame()
        {
            Start();
        }

        public override void Start()
        {

            DeckSetup();

            PlayRound();
        }

        protected override void PlayRound()
        {
            for (int currentPlayer = 0; currentPlayer < players.Count; currentPlayer++)
            {
                players[currentPlayer].TakeTurn(parser, dealer, gameDeck, players, Betting);
            }

            dealer.Draw(gameDeck);

            CheckWinners();

            GameOver();
        }

        protected override void GameOver()
        {
            Console.Clear();
            Console.WriteLine($"The dealer had a hand value of {dealer.GetHandValue()}");
            foreach (BlackjackPlayer player in players)
            {
                Console.WriteLine($"Player {player.Name} - {player.GetWinState()} with a hand value of {player.GetHandValue()}");
            }
            if (Betting)
            {
                Console.WriteLine("Payouts have been calculated & paid!");
                foreach (BlackjackPlayer player in players)
                {
                    Console.WriteLine($"Player {player.Name} - Bet ${player.GetBet()} - {player.GetWinState()} - New Balance ${player.Balance + player.CalculatePayout()}");
                    player.Payout();
                }
            }

        }

        private void PlayerSetup(bool betting) 
            // TODO: move to cross-game player system so names, bets, etc. can be carried across multiple games.
            // TODO: store players in files; leaderboards?
        {
            int min = 2;
            int max = 7;
            string? input;
            int playerCount = -1;
            while (playerCount == -1)
            {
                Console.Write($"Enter playercount, {min}-{max}: ");
                input = Console.ReadLine();
                if (int.TryParse(input, out playerCount))
                {
                    if (playerCount < min || playerCount > max)
                    {
                        playerCount = -1;
                    }
                }
                else
                {
                    playerCount = -1;
                }
            }

            for (int p = 0; p < playerCount; p++)
            {
                Console.Write($"Enter name for player {p + 1}: ");
                input = Console.ReadLine();
                if (input == "" || input == null) input = $"Player {p + 1}";
                BlackjackPlayer? newPlayer;
                if (betting)
                {
                    newPlayer = new BlackjackPlayer(input, true);
                }
                else
                {
                    newPlayer = new BlackjackPlayer(input);
                }
                players.Add(newPlayer);
                newPlayer = null;
            }
        }

        private void DeckSetup()
        {
            gameDeck = new Deck(DECKCOUNT);
            gameDeck.Shuffle();

            const int cardsToDraw = 2;

            for (int i = 0; i < cardsToDraw; i++)
            {
                dealer.AddCardToHand(gameDeck.Draw());

                foreach (BlackjackPlayer player in players)
                {
                    player.AddCardToHand(gameDeck.Draw());
                }
            }
        }
        private void CheckWinners()
        {
            if (dealer.bust) // if dealer is bust, all non-bust players win
            {
                foreach (BlackjackPlayer player in players)
                {
                    if (!player.bust)
                    {
                        player.Win();
                    }
                    else
                    {
                        player.Lose();
                    }
                }
            }
            else // otherwise, players with a higher hand value than dealer win
            {
                foreach (BlackjackPlayer player in players)
                {
                    if (!player.bust)
                    {
                        if (player.Hand.Count == 5) // 5 card trick
                        {
                            player.Win();
                            continue;
                        }
                        if (player.Hand.Count == 2 && player.GetHandValue() == 21)
                        {
                            player.Win(); // blackjack
                            continue;
                        }

                        if (player.GetHandValue() > dealer.GetHandValue())
                        {
                            player.Win();
                        }
                        else if (player.GetHandValue() == dealer.GetHandValue())
                        {
                            player.Draw();
                        }
                        else
                        {
                            player.Lose();
                        }
                    }
                    else
                    {
                        player.Lose();
                    }
                }
            }
        }
    }
}
