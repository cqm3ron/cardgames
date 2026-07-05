using cardgames.core;
using cardgames.core.extension;

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
        private Queue<Card>[] orderToAddToSuitStacks = [];
        private readonly Queue<Card>[] orderToAddToCardStacks = [];

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
        public Stack<Card> GetCardStack(int index) // zero-based
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

        public Card GetTopCardFromStack(int index) // zero-based
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
        public Card GetNthCardFromStack(int index, int n) // zero-based
        {
            if (index > cardStacks.Length - 1)
            {
                index = cardStacks.Length - 1;
            }

            if (index < 0)
            {
                index = 0;
            }

            // Return null if stack is empty to prevent IndexOutOfRange
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

        public Card GetHoveredCard()
        {
            foreach (Stack<Card> cardStack in cardStacks)
            {
                foreach (Card card in cardStack)
                {
                    if (card.IsHovered)
                    {
                        return card;
                    }
                }
            }
            if (drawnCards.Count > 0 && drawnCards.Peek().IsHovered)
            {
                return drawnCards.Peek();
            }

            return null;
        }

        public void HoverTopCardFromStack(int index) // zero-based
        {
            GetTopCardFromStack(index).Hover();
        }
        public void HoverCardFromStack(int stackIndex, int cardIndex)
        {
            Card card = GetNthCardFromStack(stackIndex, cardIndex);
            if (card != null) 
            {
                card.Hover();
            }
        }
        public void UnhoverTopCardFromStack(int index) // zero-based
        {
            GetTopCardFromStack(index).Unhover();
        }
        public void UnhoverCardFromStack(int stackIndex, int cardIndex)
        {
            Card card = GetNthCardFromStack(stackIndex, cardIndex);
            if (card != null)
            {
                card.Unhover();
            }
        }
        public void SelectTopCardFromStack(int index) // zero-based
        {
            GetTopCardFromStack(index).Select();
        }
        public void DeselectTopCardFromStack(int index) // zero-based
        {
            GetTopCardFromStack(index).Deselect();
        }
        public void ToggleSelectionOfTopCardFromStack(int index) // zero-based
        {
            GetTopCardFromStack(index).ToggleSelect();
        }

        public List<Card> GetNthCardAndAboveFromStack(int currentStack, int currentCardInStack)
        {
            List<Card> cards = [];
            Card card = GetNthCardFromStack(currentStack, currentCardInStack);
            if (card == null || !card.IsFaceUp)
                return [];

            for (int i = currentCardInStack; i <= GetCardStack(currentStack).Count - 1; i++)
            {
                Card checkingCard = GetNthCardFromStack(currentStack, i);
                cards.Add(checkingCard);  // Add, don't Push
            }

            return cards;
        }

        public void ToggleSelectionOfNthCardFromStack(int currentStack, int currentCardInStack) // zero-based
        {
            // TODO: UTILISE FUNCTIONALITY FROM GetNthcardAndAboveFromStack() TO SIMPLIFY FUNCTION & AVOID CODE DUPLICATION
            Card card = GetNthCardFromStack(currentStack, currentCardInStack);

            if (!card.IsFaceUp)
            {
                return;
            }

            if (card.IsSelected)
            {
                foreach (Card cardToCheck in GetCardStack(currentStack))
                {
                    if (cardToCheck.IsSelected)
                    {
                        cardToCheck.Deselect();
                    }
                }
            }
            else
            {
                for (int i = GetCardStack(currentStack).Count - 1; i >= currentCardInStack; i--)
                {
                    Card checkingCard = GetNthCardFromStack(currentStack, i);
                    if (!checkingCard.IsSelected)
                    {
                        checkingCard.Select();
                    }
                }
            }
        }
        public void AddCardToDrawnCards(Card card)
        {
            card.TurnFaceUp();
            drawnCards.Push(card);
        }
        public Stack<Card> GetDrawnCards() => drawnCards;
        private void DetermineOrderToAddToSuitStacks() // hearts, diamonds, clubs, spades, A-K
        {
            orderToAddToSuitStacks = new Queue<Card>[4];
            Suits[] suits = [Suits.Hearts, Suits.Diamonds, Suits.Clubs, Suits.Spades];
            Ranks[] ranks = [Ranks.Ace, Ranks.Two, Ranks.Three, Ranks.Four, Ranks.Five, Ranks.Six, Ranks.Seven, Ranks.Eight, Ranks.Nine, Ranks.Ten, Ranks.Jack, Ranks.Queen, Ranks.King];

            for (int i = 0; i < 4; i++)
            {
                orderToAddToSuitStacks[i] = [];
                for (int j = 0; j < ranks.Length; j++)
                {
                    orderToAddToSuitStacks[i].Enqueue(new Card(suits[i], ranks[j]));
                }
            }
        }

        public Ranks GetNextRankForSuitStack(Suits suit)
        {
            switch (suit)
            {
                case Suits.Hearts:
                    return orderToAddToSuitStacks[0].Peek().Rank;
                case Suits.Diamonds:
                    return orderToAddToSuitStacks[1].Peek().Rank;
                case Suits.Clubs:
                    return orderToAddToSuitStacks[2].Peek().Rank;
                case Suits.Spades:
                    return orderToAddToSuitStacks[3].Peek().Rank;
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
        public List<(MoveType, int)> GetPermissableMoves(Card card, bool isFromDrawPile, int stackIndex = -1, int cardIndex = -1)
        {
            // TODO: FIX BEING ABLE TO MOVE KING ON TOP OF ACE
            
            if (card == null || !card.IsFaceUp)
            {
                return [];
            }

            Suits[] suitStackOrder = [Suits.Hearts, Suits.Diamonds, Suits.Clubs, Suits.Spades];
            List<(MoveType, int)> permissableMoves = [];

            Suits suit = card.Suit;
            Ranks nextRankForSuitStack = GetNextRankForSuitStack(suit);

            for (int i = 0; i < cardStacks.Length; i++) // check each card stack to see if the card can be moved there
            {
                if (i == stackIndex)
                {
                    continue;
                }

                Stack<Card> cardStack = cardStacks[i];
                if (cardStack.Count == 0 && card.Rank == Ranks.King)
                {
                    permissableMoves.Add((MoveType.ToCardStack, i));
                }
                else if (cardStack.Count > 0)
                {
                    Card topCard = cardStack.Peek();
                    if (topCard.IsFaceUp && topCard.IsRed != card.IsRed && (int)topCard.Rank == (int)card.Rank + 1)
                    {
                        permissableMoves.Add((MoveType.ToCardStack, i));
                    }
                }
            }

            if (isFromDrawPile)
            {
                if (card.Rank == nextRankForSuitStack)
                {
                    permissableMoves.Add((MoveType.ToSuitStack, Array.IndexOf(suitStackOrder, suit)));
                }
            }
            else
            {
                if (stackIndex < 0 || stackIndex >= cardStacks.Length)
                {
                    return permissableMoves; // if the stack index is not in range, return
                }

                if (cardIndex == cardStacks[stackIndex].Count - 1 && card.Rank == nextRankForSuitStack)
                {
                    permissableMoves.Add((MoveType.ToSuitStack, Array.IndexOf(suitStackOrder, suit)));
                }
            }


            return permissableMoves;
        }

        public void MakeMove(MoveType type, Card card, int targetIndex, bool isFromDrawPile, int sourceStackIndex = -1, int sourceCardIndex = -1)
        {
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
                        return;
                    }
                    orderToAddToSuitStacks[suitIndex].Dequeue();
                    suitStacks[suitIndex].Push(card);
                }
            }
            else
            {
                if (!isFromDrawPile)
                {
                    List<Card> cardsToAdd = GetNthCardAndAboveFromStack(sourceStackIndex, sourceCardIndex);
                    if (cardsToAdd != null && cardsToAdd.Count > 0)
                    {
                        movedCards = cardsToAdd;
                        foreach (Card cardToAdd in cardsToAdd)
                        {
                            cardStacks[targetIndex].Push(cardToAdd);
                        }
                    }
                }
                if (isFromDrawPile)
                {
                    cardStacks[targetIndex].Push(card);
                }
            }

            if (isFromDrawPile)
            {
                drawnCards.RemoveFirstOccurrence(card);
            }
            else
            {
                if (movedCards.Count == 0) cardStacks[sourceStackIndex].RemoveFirstOccurrence(card);
                else
                {
                    foreach (Card currentCard in movedCards)
                    {
                        cardStacks[sourceStackIndex].RemoveFirstOccurrence(currentCard);
                    }
                }
            }
        }
    }
}
