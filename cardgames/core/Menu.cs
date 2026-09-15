using System.Reflection;
using static cardgames.core.Language;

namespace cardgames.core
{
    internal static class Menu
    {
        private const string GAMES_FOLDER_PATH = "..\\..\\..\\game\\";
        private const string LEADERBOARD_PATH = "..\\..\\..\\data\\leaderboard.md"; // In a real production environment, this data would be stored in %appdata% or similar location, in order to prevent people tampering with the user data or breaking things accidentally. However, for the purposes of this project, the data will be stored in a folder within the project directory, to make it easier to access and manage during development and testing.

        public static List<Player> StartupMenu(List<Player>? players = null)
        {
            bool goToGames = false;

            players ??= []; // if players is null, set players to the empty list

            while (!goToGames)
            {
                Player? player = null;
                string[] options;
                Console.Clear();
                Console.WriteLine($"### {T("Menu.Welcome")} ###");

                (int, int) cursorPos = Console.GetCursorPosition();
                Console.SetCursorPosition(0, 0);
                Util.WriteLineBackwards(T("Menu.LoggedIn"));
                foreach (Player _player in players)
                {
                    Util.WriteLineBackwards(_player.GetName());
                }
                Console.SetCursorPosition(cursorPos.Item1, cursorPos.Item2);

                if (players.Count == 0)
                {
                    options = [T("Menu.Login"), T("Menu.CreateUser"), T("Menu.LanguageSelect"), T("Menu.Exit")];
                    int choice = Util.GetChoice(options);
                    switch (choice)
                    {
                        case 0:
                            Console.WriteLine();
                            Console.WriteLine(T("Menu.Input.Username"));
                            string username = Console.ReadLine();

                            if (string.IsNullOrEmpty(username)) break;
                            if (string.IsNullOrWhiteSpace(username)) break;

                            Console.WriteLine(T("Menu.Input.Password"));
                            string password = Util.GetPassword()!;
                            player = Player.LogIn(username, password);

                            Util.PressAnyKey();
                            break;

                        case 1:
                            player = new Player(true);
                            break;

                        case 2:
                            Console.WriteLine();
                            SelectLanguage();
                            break;

                        case 3:
                            Player.SavePlayers(players);
                            Environment.Exit(0);
                            break;

                        default:
                            throw new NotImplementedException();
                    }
                }
                else if (players.Count > 0 && players.Count < Application.MAX_PLAYERS)
                {
                    options = [T("Menu.AddPlayer"), T("Menu.CreateUser"), T("Menu.GameSelection"), T("Menu.ViewLeaderboard"), T("Menu.RechargeBalance"), T("Menu.LanguageSelect"), T("Menu.Logout"), T("Menu.Exit")];
                    int choice = Util.GetChoice(options);
                    switch (choice)
                    {
                        case 0:
                            Console.WriteLine();
                            Console.WriteLine(T("Menu.Input.Username"));
                            string username = Console.ReadLine();
                            
                            if (string.IsNullOrEmpty(username)) break;
                            if (string.IsNullOrWhiteSpace(username)) break;

                            bool loggedInAlready = false;

                            foreach (Player playerToCheck in players)
                            {
                                if (playerToCheck.GetUsername() == username)
                                {
                                    Console.WriteLine(T("Err.AlreadyLoggedIn"));
                                    Util.PressAnyKey();
                                    loggedInAlready = true;
                                    break;
                                }
                            }

                            if (loggedInAlready)
                            {
                                break;
                            }

                            Console.WriteLine(T("Menu.Input.Password"));
                            string password = Util.GetPassword();
                            player = Player.LogIn(username, password);

                            Util.PressAnyKey();
                            break;

                        case 1:
                            player = new Player(true);
                            break;

                        case 2:
                            goToGames = true;
                            foreach (Player playerToCheckBalance in players)
                            {
                                if (playerToCheckBalance.GetBalance() <= 0)
                                {
                                    Console.WriteLine(T("Err.PlayerBankrupt", ("name", playerToCheckBalance.GetName())));
                                    Util.PressAnyKey();
                                    goToGames = false;
                                    break;
                                }
                            }
                            break;

                        case 3:
                            LeaderboardMenu();
                            break;

                        case 4:
                            RechargeBalance(players);
                            break;

                        case 5:
                            Console.WriteLine();
                            SelectLanguage();
                            break;

                        case 6:
                            if (players.Count > 0) // condition is here just in case, but this should never be an option if there are no players logged in
                            {
                                Console.Clear();
                                Console.WriteLine(T("Menu.LogoutSelect"));
                                Player[] playerOptions = players.ToArray();
                                string[] playerNames = new string[playerOptions.Length + 1]; // 1 greater than the number of players to accommodate a back button
                                foreach (Player playerToGetNameOf in playerOptions)
                                {
                                    playerNames[players.IndexOf(playerToGetNameOf)] = playerToGetNameOf.GetName();
                                }
                                playerNames[playerNames.Length - 1] = T("Menu.Back");
                                int playerChoice = Util.GetChoice(playerNames);
                                if (playerChoice == playerNames.Length - 1) break;
                                Player playerToLogout = players[playerChoice];
                                players.Remove(playerToLogout.LogOut());
                                Console.WriteLine(T("Menu.LoggedOut", ("name", playerToLogout.GetName())));
                                Util.PressAnyKey();
                            }
                            break;

                        case 7:
                            Environment.Exit(0);
                            break;

                        default:
                            throw new NotImplementedException();
                    }
                }
                else if (players.Count == Application.MAX_PLAYERS)
                {
                    options = [T("Menu.GameSelection"), T("Menu.ViewLeaderboard"), T("Menu.RechargeBalance"), T("Menu.LanguageSelect"), T("Menu.Logout"), T("Menu.Exit")];
                    int choice = Util.GetChoice(options);
                    switch (choice)
                    {
                        case 0:
                            goToGames = true;
                            foreach (Player playerToCheckBalance in players)
                            {
                                if (playerToCheckBalance.GetBalance() <= 0)
                                {
                                    Console.WriteLine(T("Err.PlayerBankrupt", ("name", playerToCheckBalance.GetName())));
                                    Util.PressAnyKey();
                                    goToGames = false;
                                    break;
                                }
                            }
                            break;

                        case 1:
                            LeaderboardMenu();
                            break;

                        case 2:
                            RechargeBalance(players);
                            break;

                        case 3:
                            Console.WriteLine();
                            SelectLanguage();
                            break;

                        case 4:
                            if (players.Count > 0) // condition is here just in case, but this should never be an option if there are no players logged in
                            {
                                Console.Clear();
                                Console.WriteLine(T("Menu.LogoutSelect"));
                                Player[] playerOptions = players.ToArray();
                                string[] playerNames = new string[playerOptions.Length];
                                foreach (Player playerToGetNameOf in playerOptions)
                                {
                                    playerNames[players.IndexOf(playerToGetNameOf)] = playerToGetNameOf.GetName();
                                }
                                Player playerToLogout = players[Util.GetChoice(playerNames)];
                                players.Remove(playerToLogout.LogOut());
                                Console.WriteLine(T("Menu.LoggedOut", ("name", playerToLogout.GetName())));
                                Util.PressAnyKey();
                            }
                            break;

                        case 5:
                            Environment.Exit(0);
                            break;

                        default:
                            throw new NotImplementedException();
                    }
                }

                if (player != null)
                {
                    players.Add(player);
                }
            }
            return players;
        }

        private static void RechargeBalance(List<Player> players)
        {
            List<string> options = [];
            List<int> playerOptions = [];

            Console.Clear();
            Console.WriteLine(T("Menu.Players.Bankrupt"));
            foreach (Player player in players)
            {
                if (player.GetBalance() <= 0)
                {
                    options.Add(T("Player.ToRecharge", ("name", player.GetName()), ("balance", player.GetBalance().ToString())));
                    playerOptions.Add(players.IndexOf(player));
                }
            }

            options.Add(T("Menu.Back"));

            int option = Util.GetChoice([.. options]);

            if (option == options.Count - 1) return;
            else players[playerOptions[option]].RechargeBalance();
        }

        public static GameBase<Player>? GameMenu(List<Player> players)
        {
            Dictionary<string, string> games = ImportGames();

            games.Add(T("Menu.Back"), "__BACK__");

            Console.Clear();

            (int, int) cursorPos = (Console.CursorLeft, Console.CursorTop);
            string[] options = [.. games.Keys];
            int selected = 0;
            GameBase<Player>? game;

            while (!Console.KeyAvailable)
            {
                Console.CursorVisible = false;
                Console.SetCursorPosition(cursorPos.Item1, cursorPos.Item2);
                for (int i = 0; i < games.Count; i++)
                {
                    if (i == selected)
                    {
                        Console.ForegroundColor = ConsoleColor.Cyan;
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                    Console.WriteLine($"> {options[i]}");
                }

                Util.ResetColor();

                object choice = GetGameChoice(games, options, ref selected);

                object emptyObject = new();

                if (choice != null && choice.GetType() == emptyObject.GetType())
                {
                    Console.Clear();
                    Console.CursorVisible = true;
                    return null;
                }

                game = (GameBase<Player>?)choice;

                if (game != null)
                {
                    Console.Clear();
                    Console.CursorVisible = true;

                    if (players.Count <= game.MAX_PLAYERS && players.Count >= game.MIN_PLAYERS) return game;
                    else if (players.Count > game.MAX_PLAYERS)
                    {
                        Console.Clear();
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine(T("Err.TooManyPlayers", ("max", game.MAX_PLAYERS.ToString())));
                        Util.ResetColor();
                        Util.PressAnyKey();
                        Console.Clear();
                    }
                    else if (players.Count < game.MIN_PLAYERS)
                    {
                        Console.Clear();
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine(T("Err.NotEnoughPlayers", ("min", game.MIN_PLAYERS.ToString())));
                        Util.ResetColor();
                        Util.PressAnyKey();
                        Console.Clear();
                    }
                }
            }

            return null;
        }

        private static object? GetGameChoice(Dictionary<string, string> games, string[] options, ref int selected)
        {
            ConsoleKeyInfo key = Console.ReadKey(true);
            if (Util.previousOptions.Contains(key.Key) || (key.Key == ConsoleKey.Tab && key.Modifiers.HasFlag(ConsoleModifiers.Shift)))
            {
                selected = (selected - 1 + options.Length) % options.Length;
            }
            else if (Util.nextOptions.Contains(key.Key))
            {
                selected = (selected + 1) % options.Length;
            }
            else if (Util.affirmatives.Contains(key.Key))
            {
                if (options[selected] == T("Menu.Back"))
                {
                    return new object();
                }

                return GetGame(games[options[selected]]);
            }

            return null;
        }

        private static Dictionary<string, string> ImportGames()
        {
            Dictionary<string, string> games = [];

            string[] gameDirectories = Directory.GetDirectories(GAMES_FOLDER_PATH); // get all games in the games folder (all implemented games)

            for (int i = 0; i < gameDirectories.Length; i++)
            {
                gameDirectories[i] = gameDirectories[i].Replace(GAMES_FOLDER_PATH, ""); // get just the name of the game, not the path
                gameDirectories[i] = char.ToUpper(gameDirectories[i][0]) + gameDirectories[i][1..]; // capitalise first letter
                string gameClassName = "cardgames.game." + gameDirectories[i].ToLower() + "." + gameDirectories[i] + "Game";

                games.Add(gameDirectories[i], gameClassName); // add to the dictionary in the required format (name, namespace)
            }

            return games; // return the dictionary when finished
        }

        private static object? GetGame(string gameClassName)
        {
            object? game = null;
            if (Assembly.GetExecutingAssembly().GetType(gameClassName) == null)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine();
                Console.WriteLine(T("Err.GameNotFound"));
                Util.ResetColor();
            }
            else
            {
                game = Activator.CreateInstance(Assembly.GetExecutingAssembly().GetType(gameClassName)!);
            }

            return game;
        }

        private static void LeaderboardMenu() // takes into account balance & recharges
        {
            Console.Clear();
            UpdateLeaderboard();
            int ITEMS_TO_DISPLAY = Console.WindowHeight - 5;
            string[] leaderboard = File.ReadAllLines(LEADERBOARD_PATH);

            Console.ForegroundColor = ConsoleColor.Red;

            Console.WriteLine(new string('=', T("Leaderboard.Title").Length));
            Console.WriteLine(T("Leaderboard.Title"));
            Console.WriteLine(new string('=', T("Leaderboard.Title").Length));
            Console.WriteLine();
            Util.ResetColor();
            for (int i = 1; i < Math.Min(ITEMS_TO_DISPLAY + 1, leaderboard.Length); i++)
            {
                switch (i)
                {
                    case 1:
                        Console.ForegroundColor = ConsoleColor.DarkYellow;
                        break;
                    case 2:
                        Console.ForegroundColor = ConsoleColor.Gray;
                        break;
                    case 3:
                        Console.ForegroundColor = ConsoleColor.Red;
                        break;
                    default:
                        Util.ResetColor();
                        break;
                }
                Console.WriteLine(leaderboard[i]);
            }
            if (leaderboard.Length == 0) Console.WriteLine(T("Leaderboard.Empty"));
            Console.WriteLine();
            Util.PressAnyKey();

        }

        public static void UpdateLeaderboard()
        {
            if (!File.Exists(LEADERBOARD_PATH))
            {
                File.Create(LEADERBOARD_PATH);
            }

            List<string> data = [];

            if (data.Count == 0)
            {
                data.Add("# Leaderboard");
            }

            Dictionary<string, Money> leaderboardPlayerData = Player.GetLeaderboardPlayerData();
            int counter = 1;

            foreach (KeyValuePair<string, Money> kvp in leaderboardPlayerData)
            {
                string dataForThisLine = "";

                dataForThisLine += counter;
                dataForThisLine += ". ";
                dataForThisLine += kvp.Key;
                dataForThisLine += " -> cr";
                dataForThisLine += kvp.Value;
                counter++;

                data.Add(dataForThisLine);
            }

            File.WriteAllText(LEADERBOARD_PATH, string.Empty);

            File.WriteAllLines(LEADERBOARD_PATH, data);
        }
    }
}
