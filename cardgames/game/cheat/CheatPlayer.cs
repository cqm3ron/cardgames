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
            CheatPlayer cheatPlayer = new()
            {
                name = player.GetName(),
                uname = player.GetUsername(),
                balance = player.GetBalance()
            };

            return cheatPlayer;
        }

        public static List<CheatPlayer> ConvertTo(List<Player> players)
        {
            List<CheatPlayer> cheatPlayers = [];

            foreach (Player p in players.ToList())
            {
                cheatPlayers.Add(ConvertTo(p));
            }

            return cheatPlayers;
        }

        public static Player ConvertFrom(CheatPlayer cheatPlayer)
        {
            Player player = new(cheatPlayer.name, cheatPlayer.uname, cheatPlayer.balance.Value, cheatPlayer.rechargeCount);
            return player;
        }

        public static List<Player> ConvertFrom(List<CheatPlayer> cheatPlayers)
        {
            List<Player> players = [];
            foreach (CheatPlayer cheatPlayer in cheatPlayers)
            {
                players.Add(ConvertFrom(cheatPlayer));
            }
            return players;
        }

        #endregion
    }
}
