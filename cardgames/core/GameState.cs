namespace cardgames.core
{
    enum Direction
    {
        clockwise,
        anticlockwise
    }
    internal class GameState
    {
        private protected List<Player> players;
        private protected int currentPlayer;
        private protected string gamePhase;
        private protected Direction direction;

        public GameState(List<Player> _players)
        {
            players = _players;
            currentPlayer = 0;
        }

        public Player GetCurrentPlayer()
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
        public void SetDirection(Direction _direction)
        {
            direction = _direction;
        }
    }
}
