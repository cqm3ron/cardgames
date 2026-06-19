namespace cardgames.core
{
    enum Direction
    {
        clockwise,
        anticlockwise
    }
    internal class GameState<TPlayer> where TPlayer : Player
    {
        private protected Deck gameDeck;
        private protected List<TPlayer> players;
        private protected int currentPlayer;
        private protected string gamePhase;
        private protected Direction direction;

        public GameState(IEnumerable<TPlayer> _players)
        {
            players = [.. _players];
            currentPlayer = 0;
            gameDeck = new(); // generates an empty deck
            gamePhase = "";
        }

        public TPlayer GetCurrentPlayer()
        {
            return players[currentPlayer];
        }

        public void NextPlayer()
        {
            if (direction == Direction.clockwise)
            {
                currentPlayer = (currentPlayer + 1) % players.Count;
            }
            else
            {
                currentPlayer = (currentPlayer - 1 + players.Count) % players.Count;
            }
        }

        public List<TPlayer> GetPlayerList()
        {
            return players;
        }

        public void SetDirection(Direction _direction)
        {
            direction = _direction;
        }

        public Deck GetDeck()
        {
            return gameDeck;
        }

        public Card DrawCard()
        {
            return gameDeck.Draw();
        }

        public virtual void Deal(int cardsToDrawEach)
        {
            if (cardsToDrawEach == -1) // deal all cards; some players may have different number of cards
            {
                int index = 0;

                while (gameDeck.Count > 0)
                {
                    players[index].AddToHand(DrawCard());
                    index = (index + 1) % players.Count;
                }
            }
            else if (cardsToDrawEach == -2) // deal all cards evenly so all players have same amount
            {              
                while (gameDeck.Count >= players.Count)
                {
                    foreach (Player player in players)
                    {
                        player.AddToHand(DrawCard());
                    }
                }
            }
            else
            {
                for (int i = 0; i < cardsToDrawEach; i++)
                {
                    foreach (Player player in GetPlayerList())
                    {
                        player.AddToHand(DrawCard());
                    }
                }
            }
        }
        
        public virtual void SetupDeck(int deckCount)
        {
            gameDeck = new(); gameDeck.AddStandardDecks(deckCount); gameDeck.Shuffle(); // Create, populate & shuffle the play deck
        }

        public int ChooseRandomStartingPlayer()
        {
            Random rnd = new();
            currentPlayer = rnd.Next(0, players.Count);
            return currentPlayer;
        }
    }
}
