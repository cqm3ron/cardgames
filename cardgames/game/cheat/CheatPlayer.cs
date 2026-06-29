using cardgames.core;
using cardgames.game.blackjack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cardgames.game.cheat
{
    internal class CheatPlayer : Player
    {
        private bool won = false;
        public bool HasWon()
        {
            return won;
        }
        public void Win()
        {
            won = true;
        }

        #region CONVERSIONS

        public static CheatPlayer ConvertTo(Player player)
        {
            CheatPlayer blackjackPlayer = new()
            {
                name = player.GetName(),
                uname = player.GetUsername(),
                balance = player.GetBalance()
            };

            return blackjackPlayer;
        }

        public static List<CheatPlayer> ConvertTo(List<Player> players)
        {
            List<CheatPlayer> bjplayers = [];

            foreach (Player p in players.ToList())
            {
                bjplayers.Add(ConvertTo(p));
            }

            return bjplayers;
        }

        public static Player ConvertFrom(CheatPlayer blackjackPlayer)
        {
            Player player = new(blackjackPlayer.name, blackjackPlayer.uname, blackjackPlayer.balance.Value, blackjackPlayer.rechargeCount);
            return player;
        }

        public static List<Player> ConvertFrom(List<CheatPlayer> blackjackPlayers)
        {
            List<Player> players = [];
            foreach (CheatPlayer bjp in blackjackPlayers)
            {
                players.Add(ConvertFrom(bjp));
            }
            return players;
        }

        #endregion
    }
}
