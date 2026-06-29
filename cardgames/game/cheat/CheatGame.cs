using cardgames.core;
using cardgames.core.extension;
using cardgames.games.cheat;
using System.Numerics;
using System.Runtime.CompilerServices;
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
            List<CheatPlayer> cheatPlayers = CheatPlayer.ConvertTo(players);

            State = new(cheatPlayers);

            // 1 deck default, 1 deck for every 4 players over 4.
            // TODO: link to number of selectable cards. Should this be higher with more decks? not sure.
            State.SetupDeck(DECKCOUNT + (int)Math.Ceiling((cheatPlayers.Count - 4.0) / 4.0f)); // floats specified to ensure float division not integer division
            State.Deal(-1); // deal ALL cards out, not worrying about it being even between players

            Betting.BettingMenu(State.GetPlayerList());

            State.ChooseRandomStartingPlayer();

            while (!State.IsGameOver()) // game loop 
            {
                PlayTurn();

                CheckWinners();

                if (!State.IsGameOver()) State.NextPlayer();
            }

            EndGame();

            Console.WriteLine(T("Util.PressKey"));
            Console.ReadKey(true);

            players = CheatPlayer.ConvertFrom(State.GetPlayerList());
            return players;
        }

        private void CheckWinners()
        {
            foreach (CheatPlayer player in State.GetPlayerList())
            {
                if (player.GetHand().Count == 0)
                {
                    player.Win();
                    State.GameOver();
                    return;
                }
            }
        }

        private void DisplayCallCheatMenu(int displayDurationSeconds)
        {
            const int msStepDuration = 100;
            for (int t = displayDurationSeconds * 1000; t > 0; t -= msStepDuration) // convert time in seconds to ms; step by -100ms 
            {
                if (CANCEL_COUNTDOWN) return;
                if (t % 1000 == 0) Console.WriteLine(t / 1000);
                Thread.Sleep(msStepDuration);
            }

        }

        private void CheatCalled(CheatPlayer accused, CheatPlayer accuser, List<Card> playedCards, Ranks? rankClaimed)
        {
            Console.Clear();
            Console.WriteLine(T("Cheat.CheatCalled.LookAway", ("accused", accused.GetName()), ("accuser", accuser.GetName())));
            
            Console.WriteLine(T("Util.PressKey"));
            Console.ReadKey(true);
            Console.Clear();


            Console.WriteLine(T("Cheat.Player.PlayedCards", ("player", accused.GetName())));
            foreach (Card card in playedCards)
            {
                CheatDisplay.BigCard(card);
            }
            Console.SetCursorPosition(0, CheatDisplay.CARD_HEIGHT + Console.GetCursorPosition().Top);
            if (DidCheat(playedCards, rankClaimed))
            {
                Console.WriteLine(T("Cheat.Player.Cheated", ("player", accused.GetName())));
                State.PlayerPicksUpPile(accused);
            }
            else
            {
                Console.WriteLine(T("Cheat.Player.Truthful", ("player", accused.GetName())));
                State.PlayerPicksUpPile(accuser); 
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
            Console.Clear();
            const int CHEAT_DISPLAY_DURATION_SECONDS = 5;

            State.PrepareDeck();
            CheatPlayer current = State.GetCurrentPlayer();
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
                    Console.Write(T("Cheat.Player.PlayedCards", ("player", current.GetName())) + ": ");
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

            Card.DeselectCards(playedCards);

            Thread? awaitingCheatCall = new(() => DisplayCallCheatMenu(CHEAT_DISPLAY_DURATION_SECONDS))
            {
                IsBackground = true
            };

            awaitingCheatCall.Start();

            while (awaitingCheatCall.IsAlive)
            {
                if (!Console.KeyAvailable)
                {
                    Thread.Sleep(50);
                    continue;
                }

                Console.ReadKey(true);
                CANCEL_COUNTDOWN = true;
                awaitingCheatCall.Join();

                List<string> options = [];
                List<CheatPlayer?> playerOptions = [];
                for (int i = 0; i < State.GetPlayerList().Count; i++)
                {
                    if (State.GetPlayerList()[i].GetUsername() == current.GetUsername()) continue;
                    options.Add(State.GetPlayerList()[i].GetName());
                    playerOptions.Add(State.GetPlayerList()[i]);
                }
                options.Add(T("Util.Cancel"));
                playerOptions.Add(null);

                Console.Clear();
                Console.WriteLine(T("Cheat.Player.CallCheat"));
                CheatPlayer? playerCalledCheat = playerOptions[Util.GetChoice([.. options])];
                if (playerCalledCheat != null) CheatCalled(current, playerCalledCheat, playedCards, rank);
                Console.Clear();

                break;
            }

            Console.WriteLine(T("Util.PressKey"));
            Console.ReadKey(true);
        }

        private protected override void EndGame()
        {
            Money pool = 0;
            Player? won = null;
            foreach (CheatPlayer player in State.GetPlayerList())
            {
                pool += player.DeductBetFromBalance();
            }
            foreach (CheatPlayer player in State.GetPlayerList())
            {
                if (player.HasWon())
                {
                    won = player;
                    player.AddToBalance(pool);
                }
            }
            Console.Clear();
            Console.WriteLine(T("Cheat.Player.Won", ("player", won?.GetName() ?? "Unknown"), ("amount", pool.ToString())));
            
            Console.WriteLine(T("Cheat.Player.FinalHands"));
            foreach (CheatPlayer player in State.GetPlayerList())
            {
                Console.Write(player.GetName() + ": ");
                if (!player.HasWon()) Console.WriteLine(string.Join(", ", player.GetHand().Select(card => $"{card.GetRankSymbol()}{card.GetSuitSymbol()}")));
                else Console.WriteLine(T("Cheat.Player.EmptyHand"));
            }
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
