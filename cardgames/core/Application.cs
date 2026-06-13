using cardgames.game.cheat;
using System.Reflection.Metadata;
using System.Text;

namespace cardgames.core
{
    internal static class Application
    {
        private static List<Player> players = [];

        public static void Load()
        {
            //LanGen.GenerateLocalisations(); // gen or update l10n files
            LoadDefaultSettings(); // Load the default settings; some can be changed later.

            players.Add(Player.LogIn("cam", "Potato123!")!); // remove after testing
            players.Add(Player.LogIn("zaineb", "Zaineb!1sthebest!")!); // remove after testing

            Menu.UpdateLeaderboard();

            GameBase<Player>? game = null;

            while (game == null)
            {
                while (game == null)
                {
                    if (players.Count == 0)
                    {
                        players = Menu.StartupMenu();
                    }
                    else
                    {
                        Menu.StartupMenu(players);
                    }

                    game = Menu.GameMenu();
                }
                game.LoadGame();
                players = game.PlayGame(players);
                Language.UnloadGame(game.GameName);
                Player.SavePlayers(players);
                game = null;
            }
        }

        private static void LoadDefaultSettings()
        {
            AppDomain.CurrentDomain.ProcessExit += new EventHandler(OnProcessExit);
            Console.CancelKeyPress += OnCancelKeyPress;
            Util.MaximiseWindow();
            Language.Load("en-GB");
            Util.ResetColor();
            Console.OutputEncoding = Encoding.UTF8;
        }

        static void OnProcessExit(object sender, EventArgs e)
        {
            Player.SavePlayers(players);
        }

        static void OnCancelKeyPress(object sender, ConsoleCancelEventArgs e)
        {
            Environment.Exit(0);
        }
    }
}
