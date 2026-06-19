using cardgames.core;
using cardgames.games.cheat;
using static cardgames.core.Language;

namespace cardgames.game.cheat
{
    internal class CheatGame : GameBase<Player>
    {
        private bool CANCEL_COUNTDOWN = false;
        public const int DECKCOUNT = 1;
        private readonly decimal[] BETTING_AMOUNTS = [0.01m, 0.05m, 0.1m, 0.2m, 0.33m, 0.5m, 0.75m, 0.9m, 1m];
        public CheatState State { get; set; }
        public CheatGame() : base() { }
        public override List<Player> PlayGame(List<Player> players)
        {
            const int CHEAT_DISPLAY_DURATION_SECONDS = 5;

        List<CheatPlayer> cheatPlayers = CheatPlayer.ConvertTo(players);

            State = new(cheatPlayers);

            // 1 deck default, 1 deck for every 4 players over 4.
            State.SetupDeck(DECKCOUNT + (int)Math.Ceiling((players.Count - 4.0) / 4.0f)); // floats specified to ensure float division not integer division
            State.Deal(-1); // deal ALL cards out, not worrying about it being even between players


            State.ChooseRandomStartingPlayer();

            while (true) // game loop 
                // (true) is temporary; end condition needed. 
            {
                Player current = State.GetCurrentPlayer();
                Console.WriteLine(T("Cheat.Player.Current", ("name", current.GetName()))); // Current Player
                Console.WriteLine(T("Cheat.Player.LookAway")); // Prompt for other players to look away
                Console.WriteLine(T("Util.PressKey"));
                Console.ReadKey(true);
                current.SortHandByRank();
                current.DeselectAllCards();
                ScrollMenu(current.GetHand());
                List<Card> playedCards = current.PlayCards(Card.GetSelectedCards(current.GetHand())); // play selected cards; return them to be added to discard deck
                State.Discard(playedCards); // add the played cards onto the top of the discard deck

                CheatParser nlp = new();
                nlp.ImportMaps("..\\..\\..\\game\\cheat\\map\\rankmap.json", "..\\..\\..\\game\\cheat\\map\\suitmap.json");
                Ranks? rank = null;
                bool invalidClaim = false, moveOn = false;

                while (moveOn == false)
                {
                    CANCEL_COUNTDOWN = false;
                    while (rank == null)
                    {
                        Console.Clear();
                        Console.Write(T("Cheat.Player.PlayedCards") + ": ");
                        for (int i = 0; i < playedCards.Count; i++)
                        {
                            Console.Write(playedCards[i]);
                            if (i < playedCards.Count - 1) Console.Write(", ");
                        }
                        Console.WriteLine();
                        Console.WriteLine(T("Cheat.Player.RankClaimPrompt"));

                        if (invalidClaim) Console.WriteLine(T("Cheat.Player.InvalidClaim"));
                        invalidClaim = false;
                        Console.CursorVisible = true;
                        string claim = Console.ReadLine()!;
                        nlp.TryParseRank(claim, out rank);
                        if (rank == null) invalidClaim = true;
                    }

                    Console.WriteLine(T("Cheat.Player.RankConfirmation", ("rank", rank.ToString()!)));
                    if (Util.UserAgrees())
                    {
                        moveOn = true;
                    }
                    else
                    {
                        rank = null;
                        moveOn = false;
                    }
                }

                Console.WriteLine(T("Util.PressKey"));
                Console.ReadKey(true);
                
                Console.Clear();

                Console.WriteLine(T("Cheat.Player.OpenEyes"));
                CheatState.Beep();
                Console.WriteLine();

                if (playedCards.Count == 1) Console.WriteLine(T("Cheat.Player.ClaimDisplaySingular", ("player", current.GetName()), ("count", playedCards.Count.ToString()), ("rank", rank.ToString()!)));
                else Console.WriteLine(T("Cheat.Player.ClaimDisplayMultiple", ("player", current.GetName()), ("count", playedCards.Count.ToString()), ("rank", rank.ToString()!)));

                Console.WriteLine(T("Cheat.Player.CheatPrompt", ("player", current.GetName())));
                Console.WriteLine(); // Otherwise, allow the timer to finish.



                Thread? awaitingCheatCall = new(() => DisplayCallCheatMenu(CHEAT_DISPLAY_DURATION_SECONDS))
                {
                    IsBackground = true
                };

                awaitingCheatCall.Start();
                while (awaitingCheatCall.ThreadState == ThreadState.Background)
                {
                    Console.ReadKey(true);
                    CANCEL_COUNTDOWN = true;
                    awaitingCheatCall.Join();
                    List<string> options = [];
                    List<Player?> playerOptions = [];
                    for (int i = 0; i < players.Count; i++) 
                    {
                        if (players[i].GetUsername() == current.GetUsername()) continue;
                        options.Add(players[i].GetName());
                        playerOptions.Add(players[i]);
                    }
                    options.Add(T("Util.Cancel"));
                    playerOptions.Add(null);
                    Console.Clear();
                    Console.WriteLine(T("Cheat.Player.CallCheat"));
                    Player? playerCalledCheat = playerOptions[Util.GetChoice([.. options])];
                    if (playerCalledCheat != null) CheatCalled(current, playerCalledCheat, playedCards, rank);
                }
                Console.Clear();

                // Cheat window closed. Moving on to the next player's turn.



                Console.WriteLine(T("Util.PressKey"));
                Console.ReadKey(true);


                State.NextPlayer();
            }

            /*
             * PLAN
             * 1. give out hands [x]
             * 2. determine play order [x]
             * 4. first player look; everyone else look away [x]
             * 5. first player do they thang [x]
             * 5a. nlp [x]
             * 6. first player finish turn CLEAR SCREEN [x]
             * 7. option to call cheat at any point? [x] ==> Choosing option B; more similar to official rules.
             *     a. each player gets their own key to press to call cheat perhaps?
             *     b. or just a slower-paced game; display a timed window in which any player can call cheat
             *  8. handle cheat [ ] 
             *  9. next turn [ ]
             *  10. some kinda base case idk what [ ]
             */

            EndGame();

            Console.WriteLine(T("Util.PressKey"));
            Console.ReadKey(true);

            players = CheatPlayer.ConvertFrom(State.GetPlayerList());
            return players;
        }

        private void DisplayCallCheatMenu(int displayDurationSeconds)
        {
            const int msStepDuration = 100;
            for (int t = displayDurationSeconds * 1000; t > 0; t -= msStepDuration) // convert time in seconds to ms; step by -10ms 
            {
                if (CANCEL_COUNTDOWN) break;
                if (t % 1000 == 0) Console.WriteLine(t / 1000);
                Thread.Sleep(msStepDuration);
            }

        }

        private void CheatCalled(Player accused, Player accuser, List<Card> playedCards, Ranks? rankClaimed)
        {
            Console.Clear();
            Console.WriteLine(T("Cheat.CheatCalled.LookAway", ("accused", accused.GetName()), ("accuser", accuser.GetName())));
            
            Console.WriteLine(T("Util.PressKey"));
            Console.ReadKey(true);
            Console.Clear();

            foreach (Card card in playedCards)
            {
                CheatDisplay.BigCard(card);
            }

            if (DidCheat(playedCards, rankClaimed))
            {

            }
            

            Console.WriteLine(T("Util.PressKey"));
            Console.ReadKey(true);

            Console.Clear();
            Console.WriteLine(T("Cheat.Player.OpenEyes"));
            CheatState.Beep();
        }

        private static bool DidCheat(List<Card> playedCards, Ranks? rankClaimed)
        {
            foreach (Card card in playedCards)
            {
                if (card.Rank != rankClaimed) return true;
            }
            return false;
        }

        private protected override void PlayTurn()
        {

        }
        private protected override void EndGame()
        {

        }

        private static void ScrollMenu(List<Card> cards)
        {
            Console.CursorVisible = false;
            int current = 0;
            ConsoleKeyInfo inputKey;
            Card selectedCard;
            bool changesMade = true;

            while (true)
            {
                if (changesMade) CheatDisplay.DisplayScrollMenu(cards, current); changesMade = false;

                inputKey = Console.ReadKey(true);

                if (Util.scrollRight.Contains(inputKey.Key) && current < cards.Count - 1)
                {
                    current++;
                    changesMade = true;
                }
                else if (Util.scrollLeft.Contains(inputKey.Key) && current > 0)
                {
                    current--;
                    changesMade = true;
                }
                else if (inputKey.Key != ConsoleKey.Enter && Util.affirmatives.Contains(inputKey.Key))
                {
                    selectedCard = cards[current];
                    if (Card.GetSelectedCardCount(cards) < 4 &!selectedCard.IsSelected)
                    {
                        selectedCard.Select();
                        changesMade = true;
                    }
                    else if (selectedCard.IsSelected)
                    {
                        selectedCard.Deselect();
                        changesMade = true;
                    }
                }
                else if (Card.GetSelectedCardCount(cards) > 0 && inputKey.Key == ConsoleKey.Enter)
                {
                    break;
                }
                else
                {
                    changesMade = false;
                }
            }

        }
    }
}
