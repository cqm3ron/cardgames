using cardgames.core;
using System.Numerics;
using static cardgames.core.Language;

namespace cardgames.games.blackjack
{
    internal class BlackjackGame : GameBase<Player>
    {
        public const int DECKCOUNT = 6;
        public BlackjackState State { get; set; }
        private BlackjackParser Parser { get; set; } // unused for now
        
        public BlackjackGame() : base() { }

        public override List<Player> PlayGame(List<Player> players)
        {
            LoadGame();

            // maybe add a totally rad loading bar here (but only once ive finished the actual programming LOL)

            List<BlackjackPlayer> blackjackPlayers = BlackjackPlayer.ConvertTo(players);

            State = new(blackjackPlayers);
            Parser = new();

            foreach (BlackjackPlayer player in State.GetPlayerList())
            {
                List<double> options = [];
                double balance = player.GetBalance();
                Console.WriteLine(T("User.Betting.Info") + player.GetName());
                Console.WriteLine(T("User.Balance") + ": cr" + balance);
                Console.WriteLine(T("User.Ask.Betting"));

                options.Add(Math.Round(balance * 0.01, 2));
                options.Add(Math.Round(balance * 0.05, 2));
                options.Add(Math.Round(balance * 0.1, 2));
                options.Add(Math.Round(balance * 0.2, 2));
                options.Add(Math.Round(balance * 0.33, 2));
                options.Add(Math.Round(balance * 0.5, 2));
                options.Add(Math.Round(balance * 0.75, 2));
                options.Add(Math.Round(balance * 0.9, 2));
                options.Add(Math.Round(balance, 2));

                List<string> optionsToDisplay = [];
                
                foreach (double option in options)
                {
                    optionsToDisplay.Add($"{Math.Round((option / balance) * 100, 0)}%: "+ option.ToString());
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

            Language.UnloadGame(GameName);
            players = BlackjackPlayer.ConvertFrom(State.GetPlayerList());
            return players;
        }

        private protected override void PlayTurn()
        {
            BlackjackPlayer player = State.GetCurrentPlayer();

            string[] options = [T("Blackjack.Hit"), T("Blackjack.Stand"), T("Blackjack.Double")];

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
            Console.WriteLine(T("Blackjack.PlayerHand", new Dictionary<string, string> { { "name" /* name is the param ident, doesn't need replacing w translation key */, player.GetName() } }));

            foreach (Card card in player.GetHand())
            {
                Console.WriteLine(card);
            }

            Console.WriteLine($"\n{T("Blackjack.HandValueInfo")}" + player.HandValue);

            Console.WriteLine();

            Console.WriteLine($"Dealer's face-up card is: {state.dealer.PublicCard}"); // TODO: lang

            if (player.Bust)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Uh-oh! You went bust."); // TODO: lang
                Util.ResetColour();
            }

            if (!player.Bust)
            {
                Console.WriteLine($"/!\\ If you choose to hit, you have a {player.GetBustChance(State)}% chance of going bust!"); // TODO: lang
            }
        }
        
        private protected override void EndGame()
        {
            CheckWinners();

            foreach (BlackjackPlayer player in State.GetPlayerList())
            {
                Console.WriteLine(T("Blackjack.Player.WinSummary", new Dictionary<string, string> { { "player", player.GetName() }, { "bet", player.Bet.ToString() } }));
                if (player.Doubled) Console.WriteLine("Blackjack.Player.WinSummary.Doubled");

                if (player.WinState == WinStates.Won)
                {
                    player.AddBetToBalance();

                    if (player.Doubled)
                    {
                        Console.WriteLine(T("Blackjack.Player.Won.Double", new Dictionary<string, string> { { "balance", player.GetBalance().ToString() } }));
                    }
                    else
                    {
                        Console.WriteLine(T("Blackjack.Player.Won", new Dictionary<string, string> { { "balance", player.GetBalance().ToString() } }));
                    }

                }
                else if (player.WinState == WinStates.Lost)
                {
                    player.DeductBetFromBalance();

                    if (player.Doubled)
                    {
                        Console.WriteLine(T("Blackjack.Player.Lost.Double", new Dictionary<string, string> { { "balance", player.GetBalance().ToString() } }));
                    }
                    else
                    {
                        Console.WriteLine(T("Blackjack.Player.Lost", new Dictionary<string, string> { { "balance", player.GetBalance().ToString() } }));
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