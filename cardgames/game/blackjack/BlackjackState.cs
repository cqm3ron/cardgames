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

        public void SetupDeck(int deckCount)
        {
            gameDeck = new(); gameDeck.AddStandardDecks(deckCount); gameDeck.Shuffle(); // Create, populate & shuffle the play deck
        }

        public void Deal(int cardsToDraw)
        {
            for (int i = 0; i < cardsToDraw; i++)
            {
                foreach (BlackjackPlayer player in GetPlayerList())
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
