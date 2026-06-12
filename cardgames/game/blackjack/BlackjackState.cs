using cardgames.core;

namespace cardgames.game.blackjack
{
    internal class BlackjackState : GameState<BlackjackPlayer>
    {
        public BlackjackDealer dealer;

        public BlackjackState(List<BlackjackPlayer> _players) : base(_players)
        {
            dealer = new();
        }

        public override void Deal(int cardsToDrawEach)
        {
            for (int i = 0; i < cardsToDrawEach; i++)
            {
                foreach (Player player in GetPlayerList())
                {
                    player.AddToHand(DrawCard());
                }
                dealer.AddCardToHand(DrawCard());
            }
        }

        public bool PlayerTurnsFinished()
        {
            foreach (BlackjackPlayer player in GetPlayerList())
            {
                if (!player.HasPlayed)
                {
                    return false;
                }
            }
            return true;
        }

    }
}
