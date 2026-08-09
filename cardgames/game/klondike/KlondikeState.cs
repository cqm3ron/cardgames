using cardgames.core;
using cardgames.core.extension;
using System.Reflection.Metadata.Ecma335;

namespace cardgames.game.klondike
{
    internal class KlondikeState(List<KlondikePlayer> _players) : GameState<KlondikePlayer>(_players)
    {
        private readonly Stack<Card>[] cardStacks = new Stack<Card>[7];
        private readonly Stack<Card>[] suitStacks = new Stack<Card>[4]; // hearts, diamonds, clubs, spades
        private readonly Stack<Card> drawnCards = [];
        
        public enum MoveType
        {
            ToSuitStack,
            ToCardStack
        }

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

        private List<(MoveType, int)> Moves = [];
        public int SelectedMoveIndex
        {
            get;
            private set;
        }

        private Stack<Card>[] orderToAddToSuitStacks = [];

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
            SetupDeck(1);
            SetupCards();
        }

        public bool IsInCardStacks() => CurrentLocation == Location.CardStacks;
        public bool IsInDrawPile() => CurrentLocation == Location.DrawPiles;
        public bool IsInSuitStacks() => CurrentLocation == Location.SuitStacks;
        public bool IsInLeftmostSuitStack() => CurrentLocation == Location.SuitStacks && SelectedSuitStack == 0;
        public bool IsInFaceUpDrawPile() => CurrentLocation == Location.DrawPiles && CurrentDrawPileRegion == DrawPileRegion.FaceUp;
        public bool IsInFaceDownDrawPile() => CurrentLocation == Location.DrawPiles && CurrentDrawPileRegion == DrawPileRegion.FaceDown;
        
        public List<(MoveType, int)> GetMoves() => Moves;

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
        

        public void SetupCards()
        {
            DetermineOrderToAddToSuitStacks(); // determine the order to add cards to the suit stacks (hearts, diamonds, clubs, spades, A-K)
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
        public Stack<Card>[] GetSuitStacks()
        {
            return suitStacks;
        }

        public Card? GetCurrentCard()
        {
            if (IsInCardStacks()) return GetNthCardFromStack(SelectedCardStack, SelectedCardInStack);
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
        public Card? GetNthCardFromStack(int index, int n) // zero-based
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

            if (n >= cardStacks[index].Count)
            {
                n = cardStacks[index].Count - 1;
            }

            if (n < 0)
            {
                n = 0;
            }

            return cardStacks[index].ElementAt(cardStacks[index].Count - n - 1);
        }
        public void RemoveNthCardFromStack(int index, int n) // zero-based
        {
            if (index > cardStacks.Length - 1)
            {
                index = cardStacks.Length - 1;
            }

            if (index < 0)
            {
                index = 0;
            }

            cardStacks[index].RemoveFirstOccurrence(cardStacks[index].ElementAt(cardStacks[index].Count - n - 1));
        }

        public void HoverCardFromStack(int stackIndex, int cardIndex)
        {
            Card? card = GetNthCardFromStack(stackIndex, cardIndex);
            if (card != null) 
            {
                card.Hover();
            }
        }

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

        public void UnhoverCardFromStack(int stackIndex, int cardIndex)
        {
            Card? card = GetNthCardFromStack(stackIndex, cardIndex);
            if (card != null)
            {
                card.Unhover();
            }
        }
        public List<Card> GetNthCardAndAboveFromStack(int currentStack, int currentCardInStack)
        {
            List<Card> cards = [];
            Card? card = GetNthCardFromStack(currentStack, currentCardInStack);
            if (card == null || !card.IsFaceUp) return [];

            for (int i = currentCardInStack; i <= GetCardStack(currentStack).Count - 1; i++)
            {
                Card? checkingCard = GetNthCardFromStack(currentStack, i);
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
        private void DetermineOrderToAddToSuitStacks() // hearts, diamonds, clubs, spades, A-K
        {
            orderToAddToSuitStacks = new Stack<Card>[4];
            Suits[] suits = [Suits.Hearts, Suits.Diamonds, Suits.Clubs, Suits.Spades];
            Ranks[] ranks = [Ranks.Ace, Ranks.Two, Ranks.Three, Ranks.Four, Ranks.Five, Ranks.Six, Ranks.Seven, Ranks.Eight, Ranks.Nine, Ranks.Ten, Ranks.Jack, Ranks.Queen, Ranks.King];

            for (int i = 0; i < 4; i++)
            {
                orderToAddToSuitStacks[i] = [];
                for (int j = ranks.Length - 1; j >= 0; j--)
                {
                    orderToAddToSuitStacks[i].Push(new Card(suits[i], ranks[j]));
                }
            }
        }

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
            List<(MoveType, int)> moves = [];
            Card? card = null;

            if (IsInCardStacks())
            {
                card = GetNthCardFromStack(SelectedCardStack, SelectedCardInStack);
            }
            else if (IsInFaceUpDrawPile() && drawnCards.Count > 0)
            {
                card = drawnCards.Peek();
            }
            else if (IsInSuitStacks() && suitStacks[SelectedSuitStack].Count > 0)
            {
                card = suitStacks[SelectedSuitStack].Peek();
            }
            else
            {
                ResetMoves();
            }

            if (card == null || !card.IsFaceUp) // if card isn't real or is face down, return no moves (no cheating going on here thank you very much)
            {
                ResetMoves();
                return;
            }

            Suits[] suitStackOrder = [Suits.Hearts, Suits.Diamonds, Suits.Clubs, Suits.Spades];

            Suits suit = card.Suit;

            Ranks? nextRankForSuitStack = GetNextRankForSuitStack(suit);

            if (IsInFaceUpDrawPile()) // if the card is from the draw pile, check if it can be moved to a suit stack
            {
                if (card.Rank == nextRankForSuitStack)
                {
                    moves.Add((MoveType.ToSuitStack, Array.IndexOf(suitStackOrder, suit)));
                }
            }
            else if (IsInCardStacks()) // if the card is from a card stack, check if it can be moved to a suit stack
            {
                if (SelectedCardStack < 0 || SelectedCardStack >= cardStacks.Length)
                {
                    Moves = moves; // if the stack index is not in range, return
                    return;
                }

                if (SelectedCardInStack == cardStacks[SelectedCardStack].Count - 1 && card.Rank == nextRankForSuitStack) // if it can be moved to a suit stack then prioritise that
                {
                     moves.Add((MoveType.ToSuitStack, Array.IndexOf(suitStackOrder, suit)));
                }
            }

            moves = moves.OrderBy(move => move.Item1).ToList(); 

            for (int i = 0; i < cardStacks.Length; i++) // check each card stack to see if the card can be moved there
            {
                if (IsInCardStacks() && i == SelectedCardStack) continue;

                Stack<Card> cardStack = cardStacks[i];

                if (cardStack.Count == 0 && card.Rank == Ranks.King) // if king, prioritise moving to empty stack
                {
                    moves.Add((MoveType.ToCardStack, i));
                }
                else if (cardStack.Count > 0) // otherwise, check if the card can be moved to any other stack
                {
                    Card topCard = cardStack.Peek();

                    if (topCard.Rank != Ranks.Ace && topCard.IsFaceUp && topCard.IsRed != card.IsRed && (int)topCard.Rank == (int)card.Rank + 1) // Aces cannot have cards moved on top of them
                    { 
                        moves.Add((MoveType.ToCardStack, i));
                    }
                }
            }


            if (moves.Count == 0) SelectedMoveIndex = 0;
            else if (SelectedMoveIndex >= moves.Count) SelectedMoveIndex = moves.Count - 1;

            Moves = moves;
        }

        public bool TryMakeSelectedMove() // TODO: UPDATE & OPTIIMSE
        {
            if (Moves.Count == 0) return false;

            MoveType type = Moves[SelectedMoveIndex].Item1;
            int targetIndex = Moves[SelectedMoveIndex].Item2;
            Card? card = GetCurrentCard();

            if (card == null) return false;

            Suits[] suitStackOrder = [Suits.Hearts, Suits.Diamonds, Suits.Clubs, Suits.Spades];
            List<Card> movedCards = [];

            if (type == MoveType.ToSuitStack)
            {
                int suitIndex = Array.IndexOf(suitStackOrder, card.Suit);

                if (orderToAddToSuitStacks[suitIndex].Count > 0)
                {
                    Card expectedCard = orderToAddToSuitStacks[suitIndex].Peek();
                    if (card.Suit != expectedCard.Suit || card.Rank != expectedCard.Rank)
                    {
                        return false;
                    }
                    orderToAddToSuitStacks[suitIndex].Pop();
                    UnhoverCurrentCard();
                    suitStacks[suitIndex].Push(card);
                }
            }
            else
            {
                if (IsInCardStacks())
                {
                    List<Card> cardsToAdd = GetNthCardAndAboveFromStack(SelectedCardStack, SelectedCardInStack);
                    if (cardsToAdd != null && cardsToAdd.Count > 0)
                    {
                        movedCards = cardsToAdd;
                        foreach (Card cardToAdd in cardsToAdd)
                        {
                            UnhoverCurrentCard();
                            cardStacks[targetIndex].Push(cardToAdd);
                        }
                    }
                }
                else if (IsInDrawPile())
                {
                    UnhoverCurrentCard();
                    cardStacks[targetIndex].Push(card);
                }
                else if (IsInSuitStacks())
                {
                    UnhoverCurrentCard();
                    AddRankBackToNextSuitStackList(card.Suit, card.Rank);
                    cardStacks[targetIndex].Push(card);
                    suitStacks[SelectedSuitStack].Pop();
                }
            }

            if (IsInDrawPile())
            {
                drawnCards.Pop();
            }
            else if (IsInCardStacks())
            {
                Stack<Card> sourceStack = cardStacks[SelectedCardStack];
                List<Card> stackContents = sourceStack.ToList();

                if (movedCards.Count == 0)
                {
                    stackContents.Remove(card);
                }
                else
                {
                    foreach (Card currentCard in movedCards)
                    {
                        stackContents.Remove(currentCard);
                    }
                }

                sourceStack.Clear();
                for (int i = stackContents.Count - 1; i >= 0; i--)
                {
                    sourceStack.Push(stackContents[i]);
                }
            }

            MoveCardSelectionToTop();
            ResetMoves();
            return true;
        }

        public int Hash()
        {
            return GetHashCode();
        }
    }
}
