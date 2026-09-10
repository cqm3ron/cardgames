using cardgames.core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cardgames.game.klondike
{
    internal class KlondikeMove
    {
        public KlondikeMove(MoveType type, int targetIndex, Card? card)
        {
            Type = type;
            TargetIndex = targetIndex;
            Card = card;
        }
        public KlondikeMove(MoveType type, int targetIndex, KlondikeState.Location? startingLocation, int startingStackIndex, int startingCardIndex, Card? card)
        {
            Type = type;
            TargetIndex = targetIndex;
            StartingLocation = startingLocation;
            StartingStackIndex = startingStackIndex;
            StartingCardIndex = startingCardIndex;
            Card = card;
        }
        public KlondikeMove(MoveType type, int targetIndex, KlondikeState.Location? startingLocation, Card? card)
        {
            Type = type;
            TargetIndex = targetIndex;
            StartingLocation = startingLocation;
            Card = card;
        }
        public KlondikeMove(MoveType type, Card? card)
        {
            Type = type;
            Card = card;
        }
        public enum MoveType
        {
            ToSuitStack,
            ToCardStack,
            DrawCard,
            ResetDrawPile,
            TurnCard
        }

        public KlondikeState.Location? StartingLocation { get; private set; }
        public MoveType Type { get; private set; }
        public int StartingStackIndex { get; private set; }
        public int StartingCardIndex { get; private set; }
        public int TargetIndex { get; private set; }
        public Card? Card { get; private set; }

        public override string ToString()
        {
            return $"Card {Card}, {Type} from stack {StartingStackIndex} card {StartingCardIndex} to stack index {TargetIndex}";
        }
    }
}
