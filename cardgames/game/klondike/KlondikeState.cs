using cardgames.core;
using cardgames.core.extension;
using System.ComponentModel.Design;
using static cardgames.game.klondike.KlondikeMove;

namespace cardgames.game.klondike
{
    internal class KlondikeState(List<KlondikePlayer> _players) : GameState<KlondikePlayer>(_players)
    {
        private static Suits[] suitStackOrder = [Suits.Hearts, Suits.Diamonds, Suits.Clubs, Suits.Spades];
        private readonly Stack<Card>[] cardStacks = new Stack<Card>[7];
        private readonly Stack<Card>[] suitStacks = new Stack<Card>[4]; // hearts, diamonds, clubs, spades
        private Stack<Card> drawnCards = [];
        
        public enum Location
        {
            CardStacks,
            SuitStacks,
            DrawPiles
        }

        public enum DrawPileRegion
        {
            FaceDown,
            FaceUp
        }

        public int SelectedCardStack
        {
            get; private set;
        }

        public int SelectedSuitStack
        {
            get; private set;
        }

        public int SelectedCardInStack
        {
            get; private set;
        }

        public bool HasBeenSolved
        {
            get; private set;
        }

        public Location CurrentLocation
        {
            get; private set;
        }

        public DrawPileRegion CurrentDrawPileRegion
        {
            get; private set;
        }

        private List<KlondikeMove> Moves = [];
        public int SelectedMoveIndex
        {
            get;
            private set;
        }

        public int TimesDrawPileRestocked
        {
            get; private set;
        }

        public int Score
        {
            get; private set;
        }

        private Stack<Card>[] orderToAddToSuitStacks = new Stack<Card>[4];



        #region LOCATION BOOLEANS

        public bool IsInCardStacks() => CurrentLocation == Location.CardStacks;
        public bool IsInDrawPile() => CurrentLocation == Location.DrawPiles;
        public bool IsInSuitStacks() => CurrentLocation == Location.SuitStacks;
        public bool IsInLeftmostSuitStack() => CurrentLocation == Location.SuitStacks && SelectedSuitStack == 0;
        public bool IsInFaceUpDrawPile() => CurrentLocation == Location.DrawPiles && CurrentDrawPileRegion == DrawPileRegion.FaceUp;
        public bool IsInFaceDownDrawPile() => CurrentLocation == Location.DrawPiles && CurrentDrawPileRegion == DrawPileRegion.FaceDown;

        #endregion

        public List<KlondikeMove> GetMoves() => Moves;

        #region MOVE STACK & CARD SELECTIONS

        public bool MoveStackSelectionLeft()
        {
            if (SelectedCardStack > 0)
            {
                UnhoverCurrentCard();
                SelectedCardStack--;
                SelectedCardInStack = cardStacks[SelectedCardStack].Count - 1;
                HoverCurrentCard();
                ResetMoves();
                return true;
            }
            return false;
        }
        public bool MoveStackSelectionRight()
        {
            if (SelectedCardStack < cardStacks.Length - 1)
            {
                UnhoverCurrentCard();
                SelectedCardStack++;
                SelectedCardInStack = cardStacks[SelectedCardStack].Count - 1;
                HoverCurrentCard();
                ResetMoves();
                return true;
            }
            return false;
        }
        public bool MoveCardSelectionUp()
        {
            if (SelectedCardInStack > 0)
            {
                UnhoverCurrentCard();
                SelectedCardInStack--;
                HoverCurrentCard();
                ResetMoves();
                return true;
            }
            return false;
        }
        public bool MoveCardSelectionDown()
        {
            if (SelectedCardInStack < cardStacks[SelectedCardStack].Count - 1)
            {
                UnhoverCurrentCard();
                SelectedCardInStack++;
                HoverCurrentCard();
                ResetMoves();
                return true;
            }
            return false;
        }
        public bool MoveCardSelectionToTop()
        {
            if (SelectedCardInStack != cardStacks[SelectedCardStack].Count - 1)
            {
                UnhoverCurrentCard();
                SelectedCardInStack = cardStacks[SelectedCardStack].Count - 1;
                HoverCurrentCard();
                ResetMoves();
                return true;
            }
            return false;
        }

        #endregion

        #region MOVE TO LOCATIONS

        public bool MoveToDrawPile()
        {
            if (CurrentLocation != Location.DrawPiles)
            {
                UnhoverCurrentCard();
                CurrentLocation = Location.DrawPiles;
                ResetMoves();
                return true;
            }
            return false;
        }
        public bool MoveToFaceDownDrawPile()
        {
            if (!(CurrentLocation == Location.DrawPiles && CurrentDrawPileRegion == DrawPileRegion.FaceDown))
            {
                UnhoverCurrentCard();
                CurrentLocation = Location.DrawPiles;
                CurrentDrawPileRegion = DrawPileRegion.FaceDown;
                ResetMoves();
                return true;
            }
            return false;
        }
        public bool MoveToFaceUpDrawPile()
        {
            if (!(CurrentLocation == Location.DrawPiles && CurrentDrawPileRegion == DrawPileRegion.FaceUp))
            {
                UnhoverCurrentCard();
                CurrentLocation = Location.DrawPiles;
                CurrentDrawPileRegion = DrawPileRegion.FaceUp;
                ResetMoves();
                return true;
            }
            return false;
        }
        public bool MoveToCardStacks()
        {
            if (CurrentLocation != Location.CardStacks)
            {
                UnhoverCurrentCard();
                CurrentLocation = Location.CardStacks;
                HoverCurrentCard();
                ResetMoves();
                return true;
            }
            return false;
        }
        public bool MoveToSuitStacks()
        {
            if (CurrentLocation != Location.SuitStacks)
            {
                SelectedSuitStack = 0;
                UnhoverCurrentCard();
                CurrentLocation = Location.SuitStacks;
                HoverCurrentCard();
                ResetMoves();
                return true;
            }
            return false;
        }
        public bool MoveSuitStackRight()
        {
            if (SelectedSuitStack < suitStacks.Length - 1)
            {
                UnhoverCurrentCard();
                SelectedSuitStack++;
                HoverCurrentCard();
                ResetMoves();
                return true;
            }
            return false;
        }
        public bool MoveSuitStackLeft()
        {
            if (SelectedSuitStack > 0)
            {
                UnhoverCurrentCard();
                SelectedSuitStack--;
                HoverCurrentCard();
                ResetMoves();
                return true;
            }
            return false;
        }

        #endregion

        #region SELECT & MANAGE MOVES

        public bool SelectNthMove(int n)
        {
            if (n >= 0 && n < Moves.Count)
            {
                SelectedMoveIndex = n;
                return true;
            }
            return false;
        }
        public bool SelectNextMove()
        {
            if (Moves.Count > 0)
            {
                SelectedMoveIndex++;
                if (SelectedMoveIndex >= Moves.Count)
                {
                    SelectedMoveIndex = 0;
                }
                return true;
            }
            else return false;
        }
        public bool SelectPreviousMove()
        {
            if (Moves.Count > 0)
            {
                SelectedMoveIndex--;
                if (SelectedMoveIndex < 0)
                {
                    SelectedMoveIndex = Moves.Count - 1;
                }
                return true;
            }
            else return false;
        }
        public bool ResetMoves()
        {
            if (Moves.Count > 0)
            {
                SelectedMoveIndex = 0;
                Moves = [];
                return true;
            }
            return false;
        }

        #endregion

        #region SCORING

        public void ScoreDrawnToCardStacks()
        {
            Score += 5;
        }
        public void ScoreDrawnToSuitStacks()
        {
            Score += 10;
        }
        public void ScoreCardStacksToSuitStacks()
        {
            Score += 10;
        }
        public void ScoreTurnOverCardStacksCard()
        {
            Score += 5;
        }
        public void ScoreSuitStacksToCardStacks()
        {
            Score -= 15;
        }
        public void ScoreRestockDrawPile()
        {
            Score -= 100;
        }

        public void MarkDrawPileAsRestocked()
        {
            TimesDrawPileRestocked++;
            ScoreRestockDrawPile();
        }

        #endregion

        #region GAME MANAGEMENT

        public void SetupCards()
        {
            InitialiseSuitStackOrder(); // determine the order to add cards to the suit stacks (hearts, diamonds, clubs, spades, A-K)
            for (int i = 0; i < cardStacks.Length; i++)
            {
                cardStacks[i] = [];
            }

            for (int i = 0; i < 7; i++)
            {
                if (i < 1)
                {
                    cardStacks[0].Push(gameDeck.Draw());
                }
                if (i < 2)
                {
                    cardStacks[1].Push(gameDeck.Draw());
                }
                if (i < 3)
                {
                    cardStacks[2].Push(gameDeck.Draw());
                }
                if (i < 4)
                {
                    cardStacks[3].Push(gameDeck.Draw());
                }
                if (i < 5)
                {
                    cardStacks[4].Push(gameDeck.Draw());
                }
                if (i < 6)
                {
                    cardStacks[5].Push(gameDeck.Draw());
                }
                if (i < 7)
                {
                    cardStacks[6].Push(gameDeck.Draw());
                }
            }

            foreach (Card card in gameDeck.GetCards())
            {
                card.TurnFaceDown();
            }

            foreach (Stack<Card> cardStack in cardStacks)
            {
                cardStack.Peek().TurnFaceUp();
            }

            cardStacks[0].Peek().Hover(); // hover the first card by default

            for (int i = 0; i < suitStacks.Length; i++)
            {
                suitStacks[i] = [];
            }

        }
        public void ResetGame() // ctrl + alt + f7 to trigger (debug)
        {
            foreach (Stack<Card> cardStack in cardStacks)
            {
                cardStack.Clear();
            }
            foreach (Stack<Card> suitStack in suitStacks)
            {
                suitStack.Clear();
            }
            drawnCards.Clear();
            CurrentLocation = Location.CardStacks;
            CurrentDrawPileRegion = DrawPileRegion.FaceDown;
            SelectedCardStack = 0;
            SelectedSuitStack = 0;
            SelectedCardInStack = 0;
            ResetMoves();
            SelectedMoveIndex = 0;
            Score = 0;
            TimesDrawPileRestocked = 0;
            HasBeenSolved = false;
            SetupDeck(1);
            SetupCards();
        }
        public KlondikeState Clone()
        {
            KlondikeState cloned = new KlondikeState(_players);

            for (int i = 0; i < cardStacks.Length; i++)
            {
                cloned.cardStacks[i] = new Stack<Card>(cardStacks[i].Reverse());
            }

            for (int i = 0; i < suitStacks.Length; i++)
            {
                cloned.suitStacks[i] = new Stack<Card>(suitStacks[i].Reverse());
            }

            cloned.drawnCards = new Stack<Card>(drawnCards.Reverse());

            // Copy all state properties
            cloned.SelectedCardStack = SelectedCardStack;
            cloned.SelectedSuitStack = SelectedSuitStack;
            cloned.SelectedCardInStack = SelectedCardInStack;
            cloned.HasBeenSolved = HasBeenSolved;
            cloned.CurrentLocation = CurrentLocation;
            cloned.CurrentDrawPileRegion = CurrentDrawPileRegion;
            cloned.Moves = new List<KlondikeMove>(Moves);
            cloned.SelectedMoveIndex = SelectedMoveIndex;
            cloned.TimesDrawPileRestocked = TimesDrawPileRestocked;
            cloned.Score = Score;
            cloned.gameDeck = gameDeck.Clone();
            cloned.orderToAddToSuitStacks = orderToAddToSuitStacks;

            return cloned;
        }
        private void InitialiseSuitStackOrder() // hearts, diamonds, clubs, spades, A-K
        {
            orderToAddToSuitStacks = new Stack<Card>[4];
            Ranks[] ranks = [Ranks.Ace, Ranks.Two, Ranks.Three, Ranks.Four, Ranks.Five, Ranks.Six, Ranks.Seven, Ranks.Eight, Ranks.Nine, Ranks.Ten, Ranks.Jack, Ranks.Queen, Ranks.King];

            for (int i = 0; i < 4; i++)
            {
                orderToAddToSuitStacks[i] = [];
                for (int j = ranks.Length - 1; j >= 0; j--)
                {
                    orderToAddToSuitStacks[i].Push(new Card(suitStackOrder[i], ranks[j]));
                }
            }
        }



        #endregion

        private Stack<Card> GetCardStack(int index) // zero-based
        {
            if (index < 0)
            {
                index = 0;
            }
            else if (index > 7)
            {
                index = 6;
            }

            return cardStacks[index];
        }
        public Stack<Card>[] GetCardStacks()
        {
            return cardStacks;
        }

        public Card? GetCurrentCard()
        {
            if (IsInCardStacks()) return GetNthCardFromCardStack(SelectedCardStack, SelectedCardInStack);
            else if (IsInFaceUpDrawPile() && drawnCards.Count > 0) return drawnCards.Peek();
            else if (IsInSuitStacks() && suitStacks[SelectedSuitStack].Count > 0) return suitStacks[SelectedSuitStack].Peek();
            else return null;
        }

        public Card? GetTopCardFromStack(int index) // zero-based
        { 
            if (index > cardStacks.Length - 1)
            {
                index = cardStacks.Length - 1;
            }

            if (index < 0)
            {
                index = 0;
            }

            if (cardStacks[index].Count == 0)
            {
                return null;
            }

            return cardStacks[index].Peek();
        }
        public Card? GetNthCardFromCardStack(int cardStackIndex, int n) // zero-based
        {
            if (cardStackIndex > cardStacks.Length - 1)
            {
                cardStackIndex = cardStacks.Length - 1;
            }

            if (cardStackIndex < 0)
            {
                cardStackIndex = 0;
            }

            if (cardStacks[cardStackIndex].Count == 0)
            {
                return null;
            }

            if (n >= cardStacks[cardStackIndex].Count)
            {
                n = cardStacks[cardStackIndex].Count - 1;
            }

            if (n < 0)
            {
                n = 0;
            }

            return cardStacks[cardStackIndex].ElementAt(cardStacks[cardStackIndex].Count - n - 1);
        }
        public Card? GetCardAtLocation(Location location, int cardIndex = -1, int stackIndex = -1, DrawPileRegion? drawPileRegion = null)
        {
            if (location == Location.CardStacks)
            {
                if (cardIndex < 0) return null;
                if (stackIndex < 0) return null;
                if (stackIndex > cardStacks.Length - 1) return null;
                if (cardIndex > cardStacks[stackIndex].Count - 1) return null;

                return cardStacks[stackIndex].ElementAt(cardStacks[stackIndex].Count - cardIndex - 1);
            }
            else if (location == Location.SuitStacks)
            {
                if (cardIndex < 0) return null;
                if (stackIndex < 0) return null;
                if (stackIndex > suitStacks.Length - 1) return null;
                if (cardIndex > suitStacks[stackIndex].Count - 1) return null;

                return suitStacks[stackIndex].ElementAt(suitStacks[stackIndex].Count - cardIndex - 1);
            }
            else if (location == Location.DrawPiles) // can only ever get the top card from either draw pile
            {
                if (drawPileRegion == null) return null;

                if (drawPileRegion == DrawPileRegion.FaceUp)
                {
                    drawnCards.TryPeek(out Card? card);
                    return card;
                }
                else
                {
                    return gameDeck.GetTopCard();
                }
            }
            else return null;
        }


        #region CARD HOVERING

        public void HoverCurrentCard()
        {
            Card? card = GetCurrentCard();
            if (card != null) card.Hover();
        }
        public void UnhoverCurrentCard()
        {
            Card? card = GetCurrentCard();
            if (card != null) card.Unhover();
        }

        #endregion

        public List<Card> GetNthCardAndAboveFromStack(int currentStack, int currentCardInStack)
        {
            List<Card> cards = [];
            Card? card = GetNthCardFromCardStack(currentStack, currentCardInStack);
            if (card == null || !card.IsFaceUp) return [];

            for (int i = currentCardInStack; i <= GetCardStack(currentStack).Count - 1; i++)
            {
                Card? checkingCard = GetNthCardFromCardStack(currentStack, i);
                if (checkingCard != null) cards.Add(checkingCard);
            }

            return cards;
        }
        public void AddCardToDrawnCards(Card card)
        {
            card.TurnFaceUp();
            drawnCards.Push(card);
        }
        
        public Stack<Card> GetDrawnCards() => drawnCards;
        

        public Ranks? GetNextRankForSuitStack(Suits suit)
        {
            Ranks? rankToReturn = null;
            switch (suit)
            {
                case Suits.Hearts:
                    if (orderToAddToSuitStacks[0].Count != 0) rankToReturn = orderToAddToSuitStacks[0].Peek().Rank;
                    break;
                case Suits.Diamonds:
                    if (orderToAddToSuitStacks[1].Count != 0) rankToReturn = orderToAddToSuitStacks[1].Peek().Rank;
                    break;
                case Suits.Clubs:
                    if (orderToAddToSuitStacks[2].Count != 0) rankToReturn = orderToAddToSuitStacks[2].Peek().Rank;
                    break;
                case Suits.Spades:
                    if (orderToAddToSuitStacks[3].Count != 0) rankToReturn = orderToAddToSuitStacks[3].Peek().Rank;
                    break;
                default:
                    throw new ArgumentOutOfRangeException($"Invalid suit: {suit}"); // this should never happen but the compiler demands a default case so I added it
            }

            return rankToReturn;
        }

        public void AddRankBackToNextSuitStackList(Suits suit, Ranks rank)
        {
            switch (suit) { 
                case Suits.Hearts:
                    orderToAddToSuitStacks[0].Push(new Card(suit, rank));
                    break;
                case Suits.Diamonds:
                    orderToAddToSuitStacks[1].Push(new Card(suit, rank));
                    break;
                case Suits.Clubs:
                    orderToAddToSuitStacks[2].Push(new Card(suit, rank));
                    break;
                case Suits.Spades:
                    orderToAddToSuitStacks[3].Push(new Card(suit, rank));
                    break;
                default:
                    throw new ArgumentOutOfRangeException($"Invalid suit: {suit}"); // this should never happen but the compiler demands a default case so I added it
            }
        }

        public Card GetTopCardFromSuitStack(Suits suit)
        {
            switch (suit)
            {
                case Suits.Hearts:
                    return suitStacks[0].Peek();
                case Suits.Diamonds:
                    return suitStacks[1].Peek();
                case Suits.Clubs:
                    return suitStacks[2].Peek();
                case Suits.Spades:
                    return suitStacks[3].Peek();
                default:
                    throw new ArgumentOutOfRangeException($"Invalid suit: {suit}"); // this should never happen but the compiler demands a default case so I added it
            }
        }


        /* Menu Traversal:
         *  if a card cannot be moved anywhere, selection does nothing
         *  if a card can be moved, it is selected and available moves are highlighted in a colour (magenta?)
         *  if a card is selected, the user can select a destination and the card will be moved there incl. any cards on top of it (same logic as KlondikeState.SelectNthCardFromStack() func)
         */
        public void UpdateMoves()
        {
            List<KlondikeMove> moves = GetPossibleMovesForCurrentCard();
            if (moves.Count > 0)
            {
                Moves = moves;
            }
            else
            {
                ResetMoves();
            }
        }
        
        public List<KlondikeMove> GetPossibleMovesForCurrentCard()
        {
            if (IsInCardStacks()) return GetMovesByLocation(CurrentLocation, SelectedCardInStack, SelectedCardStack);
            else if (IsInSuitStacks() && suitStacks[SelectedSuitStack].Count > 0) return GetMovesByLocation(CurrentLocation, SelectedCardInStack, SelectedSuitStack);
            else if (IsInFaceDownDrawPile()) return GetMovesByLocation(CurrentLocation, drawRegion: DrawPileRegion.FaceDown);
            else if (IsInFaceUpDrawPile() && drawnCards.Count > 0) return GetMovesByLocation(CurrentLocation, drawRegion:DrawPileRegion.FaceUp);
            else return [];
        }

        public List<KlondikeMove> GetMovesByLocation(Location location, int cardIndex = 0, int cardStackIndex = -1, DrawPileRegion? drawRegion = null)
        {
            Card? card = GetCardAtLocation(location, cardIndex, cardStackIndex, drawRegion);

            List<KlondikeMove> moves = [];

            if (location == Location.DrawPiles)
            {
                if (location == Location.DrawPiles && drawRegion == DrawPileRegion.FaceDown) // if its from teh face down draw pile, decide what to do with it
                {
                    if (gameDeck.Count == 0 && drawnCards.Count > 0) // if the draw pile is empty and the drawn cards pile isn't, restock the draw pile from the drawn pile
                    {
                        moves.Add(new KlondikeMove(MoveType.ResetDrawPile, card)); // tell the move handler to reset the draw pile if it is empty & cards are available to restock it with
                        return moves;
                    }
                    else // if the draw pile is not yet empty, draw a card from it.
                    {
                        moves.Add(new KlondikeMove(MoveType.DrawCard, card)); // same as above but draw a card instead of resetting the draw pile
                        return moves;
                    }
                }
            }


            if (card == null) return []; // if the card is null, return no moves


            if (location == Location.CardStacks && !card.IsFaceUp && card == GetTopCardFromStack(cardStackIndex)) // if card can be turned over, always do that.
            {
                moves.Add(new KlondikeMove(MoveType.TurnCard, cardStackIndex, card));
                return moves;
            }
            Suits suit = card.Suit;

            Ranks? nextRankForSuitStack = GetNextRankForSuitStack(suit);

            if (location == Location.CardStacks) // if the card is from a card stack, check if it can be moved to a suit stack
            {
                if (!card.IsFaceUp) return []; // at this point if the card is face down then it shouldn't be able to be moved. all checks involving face-down cards have already hpapened

                if (cardStackIndex < 0 || cardStackIndex >= cardStacks.Length)
                {
                    return moves; // if the stack index is somehow not in range, return
                }

                if (cardIndex == cardStacks[cardStackIndex].Count - 1 && card.Rank == nextRankForSuitStack) // if it can be moved to a suit stack then prioritise that
                {
                    moves.Add(new KlondikeMove(MoveType.ToSuitStack, Array.IndexOf(suitStackOrder, suit), location, cardStackIndex, cardIndex, card));
                }
            }
            else if (location == Location.DrawPiles && drawRegion == DrawPileRegion.FaceUp && card.IsFaceUp && card.Rank == nextRankForSuitStack) // if the card is from the face up draw pile, check if it can be moved to a suit stack
            {
                moves.Add(new KlondikeMove(MoveType.ToSuitStack, Array.IndexOf(suitStackOrder, suit), card));
            }

            moves = moves.OrderBy(move => move.Type).ToList();

            if (location == Location.CardStacks || location == Location.SuitStacks || (location == Location.DrawPiles && drawRegion == DrawPileRegion.FaceUp)) // if in the face up draw pile, suit stacks or card stacks, then check for movement to card stacks. Movement to card stacks should not be checked if the card is in the face-down draw pile
            {
                for (int i = 0; i < cardStacks.Length; i++) // check each card stack to see if the card can be moved there
                {
                    if (location == Location.SuitStacks && i == cardStackIndex) continue;

                    Stack<Card> cardStack = cardStacks[i];

                    if (cardStack.Count == 0 && card.Rank == Ranks.King) // if king, prioritise moving to empty stack
                    {
                        moves.Add(new KlondikeMove(MoveType.ToCardStack, i, location, cardStackIndex, cardIndex, card));
                    }
                    else if (cardStack.Count > 0) // otherwise, check if the card can be moved to any other stack
                    {
                        Card topCard = cardStack.Peek();

                        if (topCard.Rank != Ranks.Ace && topCard.IsFaceUp && topCard.IsRed != card.IsRed && (int)topCard.Rank == (int)card.Rank + 1) // Aces cannot have cards moved on top of them
                        {
                            moves.Add(new KlondikeMove(MoveType.ToCardStack, i, location, cardStackIndex, cardIndex, card));
                        }
                    }
                }
            }


            if (moves.Count == 0) SelectedMoveIndex = 0;
            else if (SelectedMoveIndex >= moves.Count) SelectedMoveIndex = moves.Count - 1;

            return moves;
        }

        public List<KlondikeMove> GetAllPossibleMoves() // for solver
        {
            List<KlondikeMove> moves = [];

            for (int s = 0; s < cardStacks.Length; s++)
            {
                Stack<Card> cardStack = cardStacks[s];

                for (int c = 0; c < cardStack.Count; c++)
                {
                    moves.AddRange(GetMovesByLocation(Location.CardStacks, c, s));
                }
            }

            for (int c = 0; c < drawnCards.Count; c++)
            {
                moves.AddRange(GetMovesByLocation(Location.DrawPiles, c));
            }

            for (int s = 0; s < suitStacks.Length; s++)
            {
                Stack<Card> suitStack = suitStacks[s];

                for (int c = 0; c < suitStack.Count; c++)
                {
                    moves.AddRange(GetMovesByLocation(Location.SuitStacks, c, s));
                }
            }

            moves.AddRange(GetMovesByLocation(Location.DrawPiles, drawRegion: DrawPileRegion.FaceDown));
            moves.AddRange(GetMovesByLocation(Location.DrawPiles, drawRegion: DrawPileRegion.FaceUp));

            return moves;
        }


        public bool TryMakeSelectedMove()
        {
            if (Moves.Count == 0) return false;

            KlondikeMove move = Moves[SelectedMoveIndex];
            return TryMakeMove(move);
        }

        public bool TryMakeMove(KlondikeMove move) // TODO: some way of selecting which card is moved from the solver?
        {
            MoveType type = move.Type;
            int targetIndex = move.TargetIndex;
            Card? card = move.Card;


            List<Card> movedCards = [];


            if (type == MoveType.TurnCard)
            {
                card.TurnFaceUp();
                card.Hover();
                ScoreTurnOverCardStacksCard();
                return true;
            }
            else if (type == MoveType.ResetDrawPile)
            {
                while (drawnCards.Count > 0)
                {
                    Card _card = drawnCards.Pop();
                    _card.TurnFaceDown();
                    gameDeck.AddCard(_card);
                }
                MarkDrawPileAsRestocked();
                return true;
            }
            else if (type == MoveType.DrawCard)
            {
                Card drawn = DrawCard();
                if (drawn != null) AddCardToDrawnCards(drawn);
                return true;
            }

            if (card == null) return false;

            if (type == MoveType.ToSuitStack)
            {
                int suitIndex = Array.IndexOf(suitStackOrder, card.Suit); // get the index of the suit stack to add to (0-3)

                if (orderToAddToSuitStacks[suitIndex].Count > 0)
                {
                    Card expectedCard = orderToAddToSuitStacks[suitIndex].Peek(); // get the next card that can be added to the stack
                    if (card.Suit != expectedCard.Suit || card.Rank != expectedCard.Rank) // if it doesnt match, return
                    {
                        return false;
                    }
                    orderToAddToSuitStacks[suitIndex].Pop(); // otherwise, remove the card from the list of expected cards,
                    card.Unhover(); // unhover it,
                    suitStacks[suitIndex].Push(card); // and push it to the correct suit stack

                    if (move.StartingLocation == Location.DrawPiles) ScoreDrawnToSuitStacks(); // score depending on where the card originated from
                    else if (move.StartingLocation == Location.CardStacks) ScoreCardStacksToSuitStacks();
                }
            }
            else if (type == MoveType.ToCardStack)
            {
                if (move.StartingLocation == Location.CardStacks) // if the card is moving from card stack to card stack, move all cards above the card too.
                {
                    List<Card> cardsToAdd = GetNthCardAndAboveFromStack(move.StartingStackIndex, move.StartingCardIndex); // get the card and cards above it
                    if (cardsToAdd != null && cardsToAdd.Count > 0)
                    {
                        movedCards = cardsToAdd;
                        foreach (Card cardToAdd in cardsToAdd) // push each card to the new stack
                        {
                            cardToAdd.Unhover(); //UnhoverCurrentCard(); // THIS USED TO BE UnhoverCurrentCard() but my midnight logic says that doesnt make sense? TODO: check if this still works
                            cardStacks[targetIndex].Push(cardToAdd);
                        }
                    }
                }
                else if (move.StartingLocation == Location.DrawPiles)
                {
                    card.Unhover(); // unhover it
                    cardStacks[targetIndex].Push(card); // push it to the stack
                    ScoreDrawnToCardStacks(); // score it
                }
                else if (move.StartingLocation == Location.SuitStacks) // if its starting off in the suit stack
                {
                    card.Unhover(); // unhover it
                    AddRankBackToNextSuitStackList(card.Suit, card.Rank); // add it back to the list of expected cards for teh suit stack
                    cardStacks[targetIndex].Push(card); // push to card stack
                    suitStacks[move.StartingStackIndex].Pop(); // remove from suit stack
                    ScoreSuitStacksToCardStacks(); // decrease their score cause boooo
                }
            }

            if (move.StartingLocation == Location.DrawPiles) // if its starting off from the draw pile, pop the card from the drawn cards stack
            {
                drawnCards.Pop();
            }

            else if (move.StartingLocation == Location.CardStacks) // otherwise if its starting from the card stacks, ...
            {
                // sort out variables
                Stack<Card> sourceStack = cardStacks[SelectedCardStack];
                List<Card> stackContents = sourceStack.ToList();

                if (movedCards.Count == 0) // if no extra cards were moved, just remove that card
                {
                    stackContents.Remove(card);
                }
                else
                {
                    foreach (Card currentCard in movedCards)
                    {
                        stackContents.Remove(currentCard); // if there were more, remove them all too.
                    }
                }

                sourceStack.Clear(); // self-explanatory
                for (int i = stackContents.Count - 1; i >= 0; i--) // for each card in the stack, ppush it back in reverse order so the top card is still on top !
                {
                    sourceStack.Push(stackContents[i]);
                }
            }

            MoveCardSelectionToTop(); // select the top card in teh stack by default
            ResetMoves(); // reset the moves so that the list doesn't store now invalid moves
            return true; // success!
        }

        public bool CheckSolveState()
        {
            foreach (Stack<Card> suitStack in suitStacks)
            {
                if (suitStack.Count < Enum.GetValues<Ranks>().Length)
                {
                    HasBeenSolved = false;
                    return false;
                }
            }

            HasBeenSolved = true;
            return true;
        }



        //public (Location?, int?, int?) GetLocationOfCard(Card card) // returns tuple of location, index of card, index of stack (sometimes null)
        //{
        //    Location? location = null;
        //    int? cardStackIndex = null, cardIndex = null;

        //    for (int s = 0; s < cardStacks.Length; s++)
        //    {
        //        Stack<Card> cardStack = cardStacks[s];

        //        for (int c = 0; c < cardStack.Count; c++)
        //        {
        //            Card cardInStack = cardStack.ElementAt(c);

        //            if (cardInStack.Rank == card.Rank && cardInStack.Suit == card.Suit)
        //            {
        //                location = Location.CardStacks;
        //                cardStackIndex = s;
        //                cardIndex = c;
        //                return (location, cardIndex, cardStackIndex);
        //            }
        //        }
        //    }

        //    for (int s = 0; s < suitStacks.Length; s++)
        //    {
        //        Stack<Card> suitStack = suitStacks[s];

        //        for (int c = 0; c < suitStack.Count; c++)
        //        {
        //            Card cardInStack = suitStack.ElementAt(c);

        //            if (cardInStack.Rank == card.Rank && cardInStack.Suit == card.Suit)
        //            {
        //                location = Location.SuitStacks;
        //                cardStackIndex = s;
        //                cardIndex = c;
        //                return (location, cardIndex, cardStackIndex);
        //            }
        //        }
        //    }

        //    for (int c = 0; c < drawnCards.Count; c++)
        //    {
        //        Card cardInStack = drawnCards.ElementAt(c);

        //        if (cardInStack.Rank == card.Rank && cardInStack.Suit == card.Suit)
        //        {
        //            location = Location.SuitStacks;
        //            cardIndex = c;
        //            return (location, cardIndex, null);
        //        }
        //    }

        //    return (null, null, null); // not found
        //}

        public override bool Equals(object? obj)
        {
            if (obj is KlondikeState) // first makes sure the object is a klondikestate, then checks if it is equal using the wonderful other function below
            {
                return Equals((KlondikeState)obj);
            }
            else return false;
        }

        public bool Equals(KlondikeState? other)
        {
            if (other == null) return false; // if the provided state is null, return that they are not equal
            if (ReferenceEquals(this, other)) return true; // if the provided state IS the current state, return that they are equal

            if (TimesDrawPileRestocked != other.TimesDrawPileRestocked) return false; // if the draw pile has been restocked a different number of times, return false; they cannot be the same
            if (gameDeck.Count != other.gameDeck.Count) return false; // if the draw pile has a different number of cards, return false; they cannot be the same
            if (drawnCards.Count != other.drawnCards.Count) return false; // if the drawn cards pile has a different number of cards, return false; they cannot be the same

            for (int i = 0; i < suitStacks.Length; i++) // check through each suit stack and see if they are equal. If any suit stack is different, return false, otherwise continue.
            {
                if (suitStacks[i].Count != other.suitStacks[i].Count) return false;
                if (suitStacks[i].Count > 0 && !suitStacks[i].Peek().Equals(other.suitStacks[i].Peek())) return false;
            }

            if (drawnCards.Count > 0 && !drawnCards.Peek().Equals(other.drawnCards.Peek())) return false; // if the drawn cards pile has a different top card, return false; they cannot be the same

            for (int i = 0; i < cardStacks.Length; i++)
            {
                var stackA = cardStacks[i].ToArray();
                var stackB = other.cardStacks[i].ToArray();

                if (stackA.Length != stackB.Length) return false;

                for (int j = 0; j < stackA.Length; j++)
                {
                    // If face-up status differs
                    if (stackA[j].IsFaceUp != stackB[j].IsFaceUp) return false;

                    // Card identity must match for both face-up AND face-down cards
                    if (!stackA[j].Equals(stackB[j])) return false;
                }
            }

            return true;
        }

        public override int GetHashCode()
        {
            HashCode hash = new();

            // Suit Stacks: Count + Top Card
            for (int i = 0; i < suitStacks.Length; i++)
            {
                hash.Add(suitStacks[i].Count);
                if (suitStacks[i].Count > 0) hash.Add(suitStacks[i].Peek());
            }

            // Card Stacks: Face-down counts + Face-up card values
            for (int i = 0; i < cardStacks.Length; i++)
            {
                int faceDownCount = 0;
                foreach (Card card in cardStacks[i])
                {
                    if (!card.IsFaceUp)
                    {
                        faceDownCount++;
                    }
                    else
                    {
                        hash.Add(card);
                    }
                }
                hash.Add(faceDownCount);
                hash.Add(-1); // Delimiter between stacks
            }

            // Drawn Cards: Count + Top Card
            hash.Add(drawnCards.Count);
            if (drawnCards.Count > 0)
            {
                hash.Add(drawnCards.Peek());
            }

            // Deck & Restock Meta
            hash.Add(gameDeck.Count);
            hash.Add(TimesDrawPileRestocked);

            return hash.ToHashCode();
        }

    }
}
