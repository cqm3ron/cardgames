using cardgames.core;

namespace cardgames.game.klondike
{
    internal class KlondikePlayer : Player
    {
        #region CONVERSIONS

        public static KlondikePlayer ConvertTo(Player player)
        {
            KlondikePlayer kdp = new()
            {
                name = player.GetName(),
                uname = player.GetUsername(),
                balance = player.GetBalance()
            };

            return kdp;
        }

        public static List<KlondikePlayer> ConvertTo(List<Player> players)
        {
            List<KlondikePlayer> klondikePlayers = [];

            foreach (Player p in players.ToList())
            {
                klondikePlayers.Add(ConvertTo(p));
            }

            return klondikePlayers;
        }

        public static Player ConvertFrom(KlondikePlayer kdp)
        {
            Player player = new(kdp.name, kdp.uname, kdp.balance.Value, kdp.rechargeCount);
            return player;
        }

        public static List<Player> ConvertFrom(List<KlondikePlayer> klondikePlayers)
        {
            List<Player> players = [];
            foreach (KlondikePlayer klondikePlayer in klondikePlayers)
            {
                players.Add(ConvertFrom(klondikePlayer));
            }
            return players;
        }

        #endregion
    }
}
