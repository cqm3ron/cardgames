using cardgames.core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cardgames.games.blackjack
{
    internal class BlackjackPlayer : Player
    {
        private bool bust;
        private bool standing;
        private bool doubled;

        public static BlackjackPlayer ConvertTo(Player player)
        {
            BlackjackPlayer blackjackPlayer = new BlackjackPlayer();
            blackjackPlayer.name = player.GetName();
            blackjackPlayer.uname = player.GetUsername();
            blackjackPlayer.balance = player.GetBalance();
            return blackjackPlayer;
        }
        public static Player ConvertFrom(BlackjackPlayer blackjackPlayer)
        {
            Player player = new Player(blackjackPlayer.name, blackjackPlayer.uname, blackjackPlayer.balance);
            return player;
        }
    }
}
