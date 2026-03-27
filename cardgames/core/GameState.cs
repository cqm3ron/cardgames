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
            gameDeck = new();
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
    }
}
