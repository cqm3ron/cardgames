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
        private Queue<Card>[] orderToAddToSuitStacks = [];

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
            if (index < 0) index = 0;
            else if (index > 7) index = 6;
            return cardStacks[index];
        }
        public Stack<Card>[] GetCardStacks()
        {
            return cardStacks;
        }

        public Card GetTopCardFromStack(int index) // zero-based
        {
            if (index > cardStacks.Length - 1) index = cardStacks.Length - 1;
            if (index < 0) index = 0;

            return cardStacks[index].Peek();
        }
        public Card GetNthCardFromStack(int index, int n) // zero-based
        {
            if (index > cardStacks.Length - 1) index = cardStacks.Length - 1;
            if (index < 0) index = 0;
            
            if (n >= cardStacks[index].Count) n = cardStacks[index].Count - 1;
            if (n < 0) n = 0;

            return cardStacks[index].ElementAt(cardStacks[index].Count - n - 1);
        }
        public void RemoveNthCardFromStack(int index, int n) // zero-based
        {
            if (index > cardStacks.Length - 1) index = cardStacks.Length - 1;
            if (index < 0) index = 0;

            cardStacks[index].RemoveFirstOccurrence(cardStacks[index].ElementAt(cardStacks[index].Count - n - 1));
        }
        public void HoverTopCardFromStack(int index) // zero-based
        {
            GetTopCardFromStack(index).Hover();
        }
        public void HoverCardFromStack(int stackIndex, int cardIndex)
        {
            GetNthCardFromStack(stackIndex, cardIndex).Hover();
        }
        public void UnhoverTopCardFromStack(int index) // zero-based
        {
            GetTopCardFromStack(index).Unhover();
        }
        public void UnhoverCardFromStack(int stackIndex, int cardIndex)
        {
            GetNthCardFromStack(stackIndex, cardIndex).Unhover();
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
        public void ToggleSelectionOfNthCardFromStack(int currentStack, int currentCardInStack) // zero-based
        {
            Card card = GetNthCardFromStack(currentStack, currentCardInStack);

            if (!card.IsFaceUp) return;

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

                /* TODO
                 * if the current card is selected
                 * and if any card below it is selected
                 * deselect all the below cards + the current card
                 * 
                 */


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
        public Ranks GetNextRankForGivenSuit(Suits suit)
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
        public void MoveCardToSuitStack(Card card, bool cameFromCardStacks)
        {
            Suits suit = card.Suit;
            switch (suit)
            {
                case Suits.Hearts:
                    suitStacks[0].Push(card);
                    orderToAddToSuitStacks[0].Dequeue();
                    break;
                case Suits.Diamonds: 
                    suitStacks[1].Push(card);
                    orderToAddToSuitStacks[1].Dequeue();
                    break;
                case Suits.Clubs:
                    suitStacks[2].Push(card);
                    orderToAddToSuitStacks[2].Dequeue();
                    break;
                case Suits.Spades:
                    suitStacks[3].Push(card);
                    orderToAddToSuitStacks[3].Dequeue();
                    break;
            }

            if (cameFromCardStacks)
            {
                foreach (Stack<Card> cardStack in cardStacks)
                {
                    cardStack.RemoveFirstOccurrence(card); // remove the card from the card stack it came from
                }
            }
            else
            {
                drawnCards.RemoveFirstOccurrence(card); // remove the card from the drawn cards stack 
            }
        }
    }
}
