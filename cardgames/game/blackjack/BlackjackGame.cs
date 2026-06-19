using cardgames.core;
using static cardgames.core.Language;

namespace cardgames.game.blackjack
{
    internal class BlackjackGame : GameBase<Player> // TODO: show dealer's total when game ends so users know they aren't being scammed lolol
    {
        public const int DECKCOUNT = 6;
        private readonly decimal[] BETTING_AMOUNTS = [0.01m, 0.05m, 0.1m, 0.2m, 0.33m, 0.5m, 0.75m, 0.9m, 1m];
        public BlackjackState State { get; set; } = null!;
        public BlackjackGame() : base() { }
        public override List<Player> PlayGame(List<Player> players)
        {
            // LoadGame(); // moved to application.cs

            List<BlackjackPlayer> blackjackPlayers = BlackjackPlayer.ConvertTo(players);

            State = new(blackjackPlayers);

            foreach (BlackjackPlayer player in State.GetPlayerList())
            {
                Console.Clear();
                List<Money> options = [];
                Money balance = player.GetBalance();
                Console.WriteLine(T("User.Betting.Info", ("name", player.GetName())));
                Console.WriteLine(T("User.Balance") + ": cr" + balance);
                Console.WriteLine(T("User.Ask.Betting"));

                foreach (Money amount in BETTING_AMOUNTS)
                {
                    options.Add(balance * amount);
                }

                List<string> optionsToDisplay = [];

                for (int opt = 0; opt < options.Count; opt++)
                {
                    optionsToDisplay.Add($"{(int)(BETTING_AMOUNTS[opt] * 100)}%: {options[opt].ToString()}");
                }

                int choice = Util.GetChoice([.. optionsToDisplay.ToArray()]);

                player.PlaceBet(options[choice]);
            }

            const int CARDS_TO_DRAW = 2;
            State.SetupDeck(DECKCOUNT);
            State.Deal(CARDS_TO_DRAW);

            while (!State.PlayerTurnsFinished())
            {
                PlayTurn();
                State.NextPlayer();
            }

            State.dealer.Play(State.GetDeck());

            EndGame();

            Console.WriteLine(T("Util.PressKey"));
            Console.ReadKey(true);

            players = BlackjackPlayer.ConvertFrom(State.GetPlayerList());
            return players;
        }

        private protected override void PlayTurn()
        {
            BlackjackPlayer player = State.GetCurrentPlayer();

            string[] options = [T("Blackjack.Hit"), T("Blackjack.Stand")];

            Console.WriteLine(player.CardsInHand);
            Console.WriteLine(player.GetBalance());
            Console.WriteLine(player.Bet);
            Console.WriteLine(player.GetBalance() - player.Bet);
            if (player.CardsInHand <= 2 && player.GetBalance() >= player.Bet * 2)
            {
                options = [T("Blackjack.Hit"), T("Blackjack.Stand"), T("Blackjack.Double")];
            }

            while (!player.Standing && !player.Bust)
            {
                DisplayBlackjackData(player, State);

                int choice = Util.GetChoice(options);

                switch (choice)
                {
                    case 0:
                        player.AddToHand(State.DrawCard());

                        if (player.HandValue > 21)
                        {
                            player.GoBust();
                            DisplayBlackjackData(player, State);
                        }
                        break;

                    case 1:
                        player.Stand();
                        break;

                    case 2:
                        player.Double();
                        break;
                }

            }

            player.EndTurn();

        }

        private void DisplayBlackjackData(BlackjackPlayer player, BlackjackState state)
        {
            Console.Clear();
            Console.WriteLine(T("Blackjack.PlayerHand", ("name", player.GetName())));

            foreach (Card card in player.GetHand())
            {
                Console.WriteLine(card);
            }

            Console.WriteLine($"\n{T("Blackjack.HandValueInfo")}" + player.HandValue);

            Console.WriteLine();

            Console.WriteLine(T("Blackjack.Dealer.FaceUpCard") + ": " + state.dealer.PublicCard);
            if (player.Bust)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(T("Blackjack.Player.WentBust"));
                Util.ResetColour();
            }

            if (!player.Bust)
            {
                Console.WriteLine(T("Blackjack.Player.BustChance", ("playerBustChance", player.GetBustChance(state).ToString())));
            }
        }
        private protected override void EndGame()
        {
            CheckWinners();
            Console.Clear();
            foreach (BlackjackPlayer player in State.GetPlayerList())
            {
                Console.WriteLine(T("Blackjack.Player.WinSummary", ("player", player.GetName()), ("bet", player.Bet.ToString())));
                if (player.Doubled)
                {
                    Console.WriteLine(T("Blackjack.Player.WinSummary.Doubled"));
                }

                if (player.WinState == WinStates.Won)
                {
                    player.AddBetToBalance();

                    if (player.Doubled)
                    {
                        Console.WriteLine(T("Blackjack.Player.WonDouble", ("balance", player.GetBalance().ToString())));
                    }
                    else
                    {
                        Console.WriteLine(T("Blackjack.Player.Won", ("balance", player.GetBalance().ToString())));
                    }

                }
                else if (player.WinState == WinStates.Lost)
                {
                    player.DeductBetFromBalance();

                    if (player.Doubled)
                    {
                        Console.WriteLine(T("Blackjack.Player.LostDouble", ("balance", player.GetBalance().ToString())));
                    }
                    else
                    {
                        Console.WriteLine(T("Blackjack.Player.Lost", ("balance", player.GetBalance().ToString())));
                    }
                }
                else
                {
                    Console.WriteLine(T("Blackjack.Player.Drew"));
                }
            }
        }
        private void CheckWinners()
        {
            if (State.dealer.Bust) // if dealer is bust, all non-bust players win
            {
                foreach (BlackjackPlayer player in State.GetPlayerList())
                {
                    if (!player.Bust)
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
                foreach (BlackjackPlayer player in State.GetPlayerList())
                {
                    if (!player.Bust)
                    {
                        if (player.GetHand().Count == 5) // 5 card trick
                        {
                            player.Win();
                            continue;
                        }
                        if (player.GetHand().Count == 2 && player.HandValue == 21)
                        {
                            player.Win(); // blackjack!
                            continue;
                        }

                        if (player.HandValue > State.dealer.HandValue)
                        {
                            player.Win();
                        }
                        else if (player.HandValue == State.dealer.HandValue)
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