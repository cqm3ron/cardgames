using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cardgames.core
{
    internal static class Application
    {
        private static List<Player> players = [];

        public static void Load()
        {
            InitialiseSettings();


            players = Menu.StartupMenu();
            GameBase game = Menu.GameMenu();
            Console.WriteLine("Loading " + game.GetType());
            game.StartGame(players);


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

        private static void InitialiseSettings()
        {
            Translator.Load("en-GB");
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
