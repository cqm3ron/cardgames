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
        // TODO: utilise this const. Prevent logins when MAX_PLAYERS already logged in (remove option from menu) 
        // TODO: logout option
        // TODO: resend display when windowsize changes (thread?)
        // TODO: improve authentication input validation to handle empty inputs and CTRL+C / unexpected inputs
        // TODO: user data file deleted itself somehow
        // TODO: complex algorithms - find card game additions to support this?
        // TODO: refactor menu
        // TODO: add guest users
        // TODO: add solitaire
        // TODO: solitaire should be greyed out in menu if more than one player is logged in.
        // TODO: minimum and maximum player counts; only allow loading into a game if between min-max player counts. Defined in base class and constructor?
        // TODO: remove all instances of "var"; use strongly-typed variables instead.
        // TODO: add a timer to Solitaire; bets based on timer?
        // TODO: "player to place bet:" doesn't specify player in Cheat
        public static void Load()
        {
            //LanGen.GenerateLocalisations(); // gen or update l10n files
            LoadDefaultSettings(); // Load the default settings; some can be changed later.

            players.Add(Player.LogIn("cam", "Potato123!")!); // remove after testing
            //players.Add(Player.LogIn("zaineb", "Zaineb!1sthebest!")!); // remove after testing

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
            AppDomain.CurrentDomain.ProcessExit += new EventHandler(OnProcessExit); // save the game on exit
            Console.CancelKeyPress += OnCancelKeyPress; // save the game when ctrl+c is performed
            Util.MaximiseWindow();
            Language.Load("en-GB"); // load default language
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
