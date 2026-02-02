using cardgames.core;

namespace cardgames.games.blackjack
{
    internal static class BlackjackDisplay
    {
        public static void DisplayPlayerInfo(List<BlackjackPlayer> players)
        {
            foreach (BlackjackPlayer player in players)
            {
                if (player.standing && !player.doubled)
                {
                    Utilities.WriteLineBackwards($"{player.Name} stood on {player.GetHandValue()} with a bet of $" + player.GetBet().ToString());
                }
                else if (player.standing && player.doubled)
                {
                    Utilities.WriteLineBackwards($"{player.Name} stood on {player.GetHandValue()} with a doubled bet of $" + player.GetBet().ToString());
                }
                else if (player.bust && !player.doubled)
                {
                    Utilities.WriteLineBackwards($"{player.Name} went bust with a bet of $" + player.GetBet().ToString());
                }
                else if (player.bust && player.doubled)
                {
                    Utilities.WriteLineBackwards($"{player.Name} went bust with a doubled bet of $" + player.GetBet().ToString());
                }
                else
                {
                    Utilities.WriteLineBackwards($"{player.Name} bet $" + player.GetBet().ToString());
                }

            }
            Console.SetCursorPosition(0, 0);

        }
    }
}
