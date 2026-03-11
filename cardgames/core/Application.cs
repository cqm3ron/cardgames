using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static cardgames.core.Language;

namespace cardgames.core
{
    internal static class Application
    {
        private static List<Player> players = [];

        public static void Load()
        { 
            LoadDefaultSettings();

            //players.Add(Player.LogIn("cam", "Potato123!")); // remove after testing
            //players.Add(Player.LogIn("zaineb", "Zaineb!1sthebest!")); // remove after testing

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
                players = game.PlayGame(players);
                Player.SavePlayers(players);
                game = null;
            }


            //AppDomain.CurrentDomain.ProcessExit += new EventHandler (OnProcessExit);
            //Console.CancelKeyPress += OnCancelKeyPress;

            ////player = new Player();

            //Console.WriteLine("Enter username: ");
            //string username = Console.ReadLine();
            //Console.WriteLine("Enter password: ");
            //string password = Console.ReadLine();

            //player = Player.LogIn(username, password);

            //Console.ReadKey();
        }

        private static void LoadDefaultSettings()
        {
            Language.Load("en-GB");
            Util.ResetColor();
            Console.OutputEncoding = Encoding.UTF8;
        }

        //static void OnProcessExit(object sender, EventArgs e)
        //{
        //    player.SaveUserData();
        //}
        //static void OnCancelKeyPress(object sender, ConsoleCancelEventArgs e)
        //{
        //    Environment.Exit(0);
        //}
    }
}
