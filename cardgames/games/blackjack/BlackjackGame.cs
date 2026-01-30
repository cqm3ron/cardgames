using cardgames.core;

namespace cardgames.games.blackjack
{
    internal class BlackjackGame : GameBase
    {
        bool betting;
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
            YesNoParser ynp = new();
            Console.WriteLine("Do you want to enable betting?");
            string? input = Console.ReadLine();
            if (ynp.TryParseChoice(input ?? "", out YesNoParser.Choice? choice))
            {
                if (choice == YesNoParser.Choice.yes)
                {
                    betting = true;
                    Console.WriteLine("You have enabled betting.");
                }
                else
                {
                    betting = false;
                    Console.WriteLine("You won't be betting on this game.");
                }
            }

            if (betting)
            {
                PlayerSetup(true);
            }
            else
            {
                PlayerSetup(false);
            }

            DeckSetup();

            PlayRound();
        }

        protected override void PlayRound()
        {
            for (int currentPlayer = 0; currentPlayer < players.Count; currentPlayer++)
            {
                players[currentPlayer].TakeTurn(parser, dealer, gameDeck);
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
                // add betting logic here, if they win tell them how much, etc.
            }
        }

        private void PlayerSetup(bool betting)
        {
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
