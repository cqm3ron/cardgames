using System.Collections.Specialized;
using System.ComponentModel.Design;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
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

            if (players == null)
            {
                players = [];
            }

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

                            Console.WriteLine(T("Util.PressKey"));
                            Console.ReadKey(true);
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
                else
                {
                    options = [T("Menu.AddPlayer"), T("Menu.CreateUser"), T("Menu.GameSelection"), T("Menu.ViewLeaderboard"), T("Menu.RechargeBalance"), T("Menu.LanguageSelect"), T("Menu.Exit")];
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
                                    Console.WriteLine(T("Err.AlreadyLoggedIn")); // TODO: add translation key to dictionary
                                    Console.WriteLine(T("Util.PressKey"));
                                    Console.ReadKey(true);
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

                            Console.WriteLine(T("Util.PressKey"));
                            Console.ReadKey(true);
                            break;

                        case 1:
                            player = new Player(true);
                            break;

                        case 2:
                            goToGames = true;
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
            Console.WriteLine("Bankrupt Players:"); // TODO: LANG
            foreach (Player player in players)
            {
                if (player.GetBalance() <= 0)
                {
                    options.Add(player.GetName() + " has balance cr" + player.GetBalance()); // TODO: LANG
                    playerOptions.Add(players.IndexOf(player));
                }
            }

            options.Add(T("Menu.Back"));

            int option = Util.GetChoice([.. options]);

            if (option == options.Count - 1) return;
            else players[playerOptions[option]].RechargeBalance();
        }

        public static GameBase<Player>? GameMenu()
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

                var choice = GetGameChoice(games, options, ref selected);

                var temp = new object();

                if (choice != null && choice.GetType() == temp.GetType())
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
                    return game;
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
            const int ITEMS_TO_DISPLAY = 10;
            string[] leaderboard = File.ReadAllLines(LEADERBOARD_PATH);

            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(T("Leaderboard.Title")); // TODO: LANG (--# LEADERBOARD #--)
            Util.ResetColor();
            for (int i = 1; i < Math.Min(ITEMS_TO_DISPLAY + 1, leaderboard.Length); i++) Console.WriteLine(leaderboard[i]);
            Console.WriteLine();
            Console.WriteLine(T("Util.PressKey"));
            Console.ReadKey(true);

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
