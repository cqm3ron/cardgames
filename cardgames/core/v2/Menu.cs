//using static cardgames.core.Language;

//namespace cardgames.core.v2
//{
//    internal static class Menu // reworked menu class because the old one was too far gone
//    {
//        private enum MenuState
//        {
//            PreLogin,
//            PostLogin,
//            Games,
//            Exit
//        }

//        private const string GAMES_FOLDER_PATH = "..\\..\\..\\game\\";
//        private const string LEADERBOARD_PATH = "..\\..\\..\\data\\leaderboard.md"; // In a real production environment, this data would be stored in %appdata% or similar location, in order to prevent people tampering with the user data or breaking things accidentally. However, for the purposes of this project, the data will be stored in a folder within the project directory, to make it easier to access and manage during development and testing.
//        private static readonly string TITLE = $"### {T("Menu.Welcome")} ###";

//        private static readonly Dictionary<string, string> preLoginOptions = new() // kvp of option name & translation key
//        {
//            { "login", "Menu.Login" },
//            { "createuser", "Menu.CreateUser" },
//            { "languageselect", "Menu.LanguageSelect" },
//            { "exit", "Menu.Exit" }
//        };
//        private static readonly Dictionary<string, string> postLoginOptions = new()
//        {
//            { "addplayer", "Menu.AddPlayer"  },
//            { "createuser", "Menu.CreateUser" },
//            { "gameselect", "Menu.GameSelection" },
//            { "leaderboard", "Menu.ViewLeaderboard" },
//            { "recharge", "Menu.RechargeBalance" },
//            { "languageselect", "Menu.LanguageSelect" },
//            { "exit", "Menu.Exit" }
//        };

//        private static readonly string[] gameOptions = [];

//        public static List<Player> DisplayMenu(List<Player>? players = null)
//        {
//            MenuState? state = null;
//            players ??= []; // if shorthand; "if players is null, assign an empty list"

//            while (state != MenuState.Exit)
//            {
//                switch (players.Count) // determine which menu to show
//                {
//                    case 0:
//                        state = MenuState.PreLogin; break;
//                    case > 0:
//                        state = MenuState.PostLogin; break;
//                }

//                switch (state) // show the menu & get user input
//                {
//                    case MenuState.PreLogin:
//                        PreLogin(players);
//                        break;
//                    case MenuState.PostLogin:
//                        state = PostLogin(players);
//                        break;
//                }
//            }

//        }

//        // Handle each menu separately:

//        private static void PreLogin(List<Player> players)
//        {

//        }
//        private static MenuState PostLogin(List<Player> players)
//        {
//            int choice = Util.GetChoice([.. postLoginOptions.Values]); // get the translation keys for the options, and pass them to GetChoice to display the menu
//            switch (postLoginOptions.ElementAt(choice).Key)
//            {
//                case "addplayer":

//            }
//        }
//        private static void Games() { }


//       // Handle sub-menus:

//        private static void AddPlayer()
//        {
//            Console.WriteLine();
//            Console.WriteLine(T("Menu.Input.Username"));
//        }

//    }
//}
