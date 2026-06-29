using static cardgames.core.Language;

namespace cardgames.core
{
    internal static class Betting
    {
        public static readonly decimal[] DEFAULT_BETTING_AMOUNTS = [0.01m, 0.05m, 0.1m, 0.2m, 0.33m, 0.5m, 0.75m, 0.9m, 1m];

        public static void BettingMenu<TPlayer>(List<TPlayer> players, decimal[]? betAmounts = null) where TPlayer : Player
        {
            betAmounts ??= DEFAULT_BETTING_AMOUNTS;
            foreach (Player player in players)
            {
                Console.Clear();
                List<Money> options = [];
                Money balance = player.GetBalance();
                Console.WriteLine(T("User.Betting.Info", ("name", player.GetName())));
                Console.WriteLine(T("User.Balance") + ": cr" + balance);
                Console.WriteLine(T("User.Ask.Betting"));

                foreach (Money amount in betAmounts)
                {
                    options.Add(balance * amount);
                }

                List<string> optionsToDisplay = [];

                for (int opt = 0; opt < options.Count; opt++)
                {
                    optionsToDisplay.Add($"{(int)(betAmounts[opt] * 100)}%: {options[opt]}");
                }

                int choice = Util.GetChoice([.. optionsToDisplay.ToArray()]);

                player.PlaceBet(options[choice]);
            }
        }
    }
}
