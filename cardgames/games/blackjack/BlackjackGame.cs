using cardgames.core;

namespace cardgames.games.blackjack
{
    internal class BlackjackGame : IGame
    {
        private const int DECKCOUNT = 6;
        private Deck gameDeck;
        private BlackjackParser parser = new();
        private List<BlackjackPlayer> players = [];
        private Dealer dealer = new();

        public BlackjackGame()
        {
            Start();
        }

        public void Start()
        {
            #region Player Setup
            string? input;
            Console.Write("Enter playercount, 2-7: ");
            input = Console.ReadLine();
            input = input ?? "2";
            if (int.TryParse(input, out int playerCount))
            {
                if (playerCount < 2 || playerCount > 7)
                {
                    playerCount = 2;
                }
            }
            else
            {
                playerCount = 2;
            }

            for (int p = 0; p < playerCount; p++)
            {
                Console.Write($"Enter name for player {p+1}: ");
                input = Console.ReadLine();
                if (input == "" || input == null) input = $"Player {p + 1}";
                BlackjackPlayer? newPlayer = new BlackjackPlayer(input);
                players.Add(newPlayer);
                newPlayer = null;
            }
            #endregion

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

            PlayRound();
        }

        public void PlayRound()
        {
            for (int currentPlayer = 0; currentPlayer < players.Count; currentPlayer++)
            {
                players[currentPlayer].TakeTurn(parser, dealer, gameDeck);
            }

            dealer.Draw(gameDeck);

            if (dealer.bust)
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

            else
            {
                foreach (BlackjackPlayer player in players)
                {
                    if (!player.bust)
                    {
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
            GameOver();
        }

        public void GameOver()
        {
            Console.Clear();
            Console.WriteLine($"The dealer had a hand value of {dealer.GetHandValue()}");
            foreach (BlackjackPlayer player in players)
            {
                Console.WriteLine($"Player {player.name} \t - \t {player.GetWinState()} \t - \t Hand value of {player.GetHandValue()}");
            }
        }
    }
}
