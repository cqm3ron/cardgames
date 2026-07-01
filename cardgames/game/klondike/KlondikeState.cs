using cardgames.core;
using System.Reflection.Metadata.Ecma335;

namespace cardgames.game.klondike
{
    internal class KlondikeState(List<KlondikePlayer> _players) : GameState<KlondikePlayer>(_players)
    {
        private Stack<Card>[] cardStacks = new Stack<Card>[7];
        private Stack<Card> drawnCards = [];

        public void SetupCardPilesFromDeck()
        {
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

            return cardStacks[index].ElementAt(cardStacks[index].Count - n - 1);
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
            //if (card.IsFaceUp) // this should NOT be commented; only for testing purposes
                GetNthCardFromStack(currentStack, currentCardInStack).ToggleSelect();

            if (GetNthCardFromStack(currentStack, currentCardInStack) != GetCardStack(currentStack).ElementAt(GetCardStack(currentStack).Count - 1)) // if card is not the top card in the stack (i.e there are cards above it)
            {
                // select those cards too
                for (int i = GetCardStack(currentStack).Count - 1; i > currentCardInStack; i--)
                {
                    GetNthCardFromStack(currentStack, i).ToggleSelect();
                }
            }

            /* TODO
             * if the current card is selected
             * and if any card below it is selected
             * deselect all the below cards + the current card
             */


        }
        public void AddCardToDrawnCards(Card card)
        {
            card.TurnFaceUp();
            drawnCards.Push(card);
        }
        public Stack<Card> GetDrawnCards() => drawnCards;
    }
}
