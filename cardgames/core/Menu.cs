using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using static cardgames.core.Translator;

namespace cardgames.core
{
    internal static class Menu
    {
        const string GAMES_PATH = "..\\..\\..\\games\\";

        public static List<Player> StartupMenu()
        {
            bool goToGames = false;
            List<Player> players = [];
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
                            Console.WriteLine();
                            SelectLanguage();
                            break;

                        case 3:
                            Environment.Exit(0);
                            break;

                        default:
                            throw new NotImplementedException();
                    }
                }
                else
                {
                    options = [T("Menu.AddPlayer"), T("Menu.CreateUser"), T("Menu.GameSelection"), T("Menu.LanguageSelect"), T("Menu.Exit")];
                    int choice = Util.GetChoice(options);
                    switch (choice)
                    {
                        case 0:
                            Console.WriteLine();
                            Console.WriteLine(T("Menu.Input.Username"));
                            string username = Console.ReadLine();
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
                            Console.WriteLine();
                            SelectLanguage();
                            break;

                        case 4:
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
            Console.Clear();
        }

        

        public static GameBase GameMenu()
        {
            Dictionary<string, string> games = ImportGames();

            (int, int) cursorPos = (Console.CursorLeft, Console.CursorTop);
            string[] options = [.. games.Keys];
            int selected = 0;

            while (!Console.KeyAvailable)
            {
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

                GameBase? game = (GameBase?)GetGameChoice(games, options, ref selected);
                if (game != null)
                {
                    Console.Clear();
                    return game;
                }
            }
            return null;
        }

        private static object? GetGameChoice(Dictionary<string, string> games, string[] options, ref int selected)
        {

            ConsoleKeyInfo key = Console.ReadKey(true);
            if (key.Key == ConsoleKey.UpArrow || key.Key == ConsoleKey.W)
            {
                selected = (selected - 1 + options.Length) % options.Length;
            }
            else if (key.Key == ConsoleKey.DownArrow || key.Key == ConsoleKey.S)
            {
                selected = (selected + 1) % options.Length;
            }
            else if (key.Key == ConsoleKey.Enter)
            {
                return GetGame(games[options[selected]]);
            }

            return null;
        }

        private static Dictionary<string, string> ImportGames()
        {
            Dictionary<string, string> games = [];

            string[] gameDirectories = Directory.GetDirectories(GAMES_PATH); // get all games in the games folder (all implemented games)

            for (int i = 0; i < gameDirectories.Length; i++)
            {
                gameDirectories[i] = gameDirectories[i].Replace(GAMES_PATH, ""); // get just the name of the game, not the path
                gameDirectories[i] = char.ToUpper(gameDirectories[i][0]) + gameDirectories[i][1..]; // capitalise first letter
                string gameClassName = "cardgames.games." + gameDirectories[i].ToLower() + "." + gameDirectories[i] + "Game";

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
                game = Activator.CreateInstance(Assembly.GetExecutingAssembly().GetType(gameClassName));
            }

            return game;
        }

    }
}
