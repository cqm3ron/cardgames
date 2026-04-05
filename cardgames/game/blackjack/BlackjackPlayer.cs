using cardgames.core;
using static cardgames.core.Language;

namespace cardgames.game.blackjack
{
    enum WinStates
    {
        Lost,
        Drew,
        Won
    }

    internal class BlackjackPlayer : Player
    {
        #region PROPERTIES
        public bool Bust { get; private set; }
        public bool Standing { get; private set; }
        public bool Doubled { get; private set; }
        public bool HasPlayed { get; private set; }

        public WinStates WinState { get; private set; }

        public int HandValue => CalculateHandValue();
        public int CardsInHand => hand.Count;
        #endregion

        #region CONVERSIONS

        public static BlackjackPlayer ConvertTo(Player player)
        {
            BlackjackPlayer blackjackPlayer = new()
            {
                name = player.GetName(),
                uname = player.GetUsername(),
                balance = player.GetBalance()
            };

            return blackjackPlayer;
        }

        public static List<BlackjackPlayer> ConvertTo(List<Player> players)
        {
            List<BlackjackPlayer> bjplayers = [];

            foreach (Player p in players.ToList())
            {
                bjplayers.Add(ConvertTo(p));
            }

            return bjplayers;
        }

        public static Player ConvertFrom(BlackjackPlayer blackjackPlayer)
        {
            Player player = new(blackjackPlayer.name, blackjackPlayer.uname, blackjackPlayer.balance.Value, blackjackPlayer.rechargeCount);
            return player;
        }

        public static List<Player> ConvertFrom(List<BlackjackPlayer> blackjackPlayers)
        {
            List<Player> players = [];
            foreach (BlackjackPlayer bjp in blackjackPlayers)
            {
                players.Add(ConvertFrom(bjp));
            }
            return players;
        }

        #endregion

        public void AddToHand(Card card)
        {
            hand.Add(card);
        }

        public void GoBust()
        {
            Bust = true;

        }

        public void Stand()
        {
            Standing = true;
        }

        public void Double()
        {
            Doubled = true;
            DoubleBet();
            Standing = true;
        }

        private int CalculateHandValue()
        {
            int total = 0;
            int aces = 0;

            foreach (Card card in hand)
            {
                if (card.Rank == Ranks.Ace)
                {
                    aces++;
                    total += 11;
                }
                else
                {
                    total += (card.IsFaceCard) switch
                    {
                        true => 10,
                        false => (int)card.Rank
                    };

                }


                while (total > 21 && aces > 0)
                {
                    total -= 10;
                    aces--;
                }

            }

            return total;
        }

        public double GetBustChance(BlackjackState state)           // COUNTED AS A COMPLEX CALCULATION (NEA)
        {
            int decksInShoe = BlackjackGame.DECKCOUNT;              // normally 6
            Deck possibleDeck = new();
            possibleDeck.AddStandardDecks(decksInShoe);
            List<Card> cardsUserKnowsAreRemoved = hand.ToList();    // convert to list to "copy" otherwise it adds the dealer's card to the user's hand for some reason
            cardsUserKnowsAreRemoved.Add(state.dealer.PublicCard);  // dealer's card has been seen; cannot be drawn

            foreach (Card card in cardsUserKnowsAreRemoved)
            {
                possibleDeck.RemoveCard(card);                      // remove cards user knows cannot be drawn from the possible cards
            }

            int maxSafeCardValue = 21 - HandValue;                  // maximum value of a card that can be drawn safely

            List<Ranks> ranksThatWouldBust = [];

            foreach (Ranks rank in Enum.GetValues<Ranks>())
            {
                int value;

                if (rank is Ranks.King or Ranks.Queen or Ranks.Jack)
                {
                    value = 10;
                }
                else if (rank == Ranks.Ace)
                {
                    value = 1;
                }
                else
                {
                    value = (int)rank;
                }

                if (value > maxSafeCardValue)
                {
                    ranksThatWouldBust.Add(rank);
                }
            }

            int numberOfCardsThatWouldBust = 0;                     // how many possible cards would bust the player?

            foreach (Card card in possibleDeck.GetCards().ToList())
            {
                if (ranksThatWouldBust.Contains(card.Rank))
                {
                    numberOfCardsThatWouldBust++;
                }
            }

            double bustProbability = numberOfCardsThatWouldBust / (double)possibleDeck.GetCards().Count;

            bustProbability *= 100;

            bustProbability = Math.Round(bustProbability, 3);

            return bustProbability;
        }
        public void EndTurn()
        {
            HasPlayed = true;

            Console.WriteLine(T("Util.PressKey"));
            Console.ReadKey();

        }

        public void Win()
        {
            WinState = WinStates.Won;
        }
        public void Draw()
        {
            WinState = WinStates.Drew;
        }
        public void Lose()
        {
            WinState = WinStates.Lost;
        }
    }
}
