using cardgames.game.cheat;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using System.Text;

namespace cardgames.core
{
    internal static class Application
    {
        private static List<Player> players = [];
        public const int MAX_PLAYERS = 8;

        public static void Load()
        {
            LoadDefaultSettings(); // Load the default settings; some can be changed later.

            Menu.UpdateLeaderboard();

            GameBase<Player>? game = null;

            while (game == null)
            {
                while (game == null)
                {
                    if (players.Count == 0)
                    {
                        players = Menu.StartupMenu(); // display the pre-login menu
                    }
                    else
                    {
                        Menu.StartupMenu(players); // display the post-login menu
                    }

                    game = Menu.GameMenu(players);
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
            AppDomain.CurrentDomain.ProcessExit += new EventHandler(OnProcessExit); // save the game on exit; referenced https://stackoverflow.com/questions/40143822/applicationexit-event-not-being-raised
            Console.CancelKeyPress += OnCancelKeyPress; // save the game when ctrl+c is performed; referenced https://stackoverflow.com/questions/40143822/applicationexit-event-not-being-raised
            Util.MaximiseWindow();
            Language.Load("en-GB"); // load default language
            Util.ResetColor();
            Console.OutputEncoding = Encoding.UTF8;
        }

        private static void OnProcessExit(object sender, EventArgs e) // referenced https://stackoverflow.com/questions/40143822/applicationexit-event-not-being-raised
        {
            Player.SavePlayers(players);
        }

        private static void OnCancelKeyPress(object sender, ConsoleCancelEventArgs e) // referenced https://stackoverflow.com/questions/40143822/applicationexit-event-not-being-raised
        {
            Environment.Exit(0);
        }
    }
}
