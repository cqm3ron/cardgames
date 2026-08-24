using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cardgames.game.klondike
{
    internal class KlondikeMove
    {
        public KlondikeMove(MoveType type, int targetIndex)
        {
            Type = type;
            TargetIndex = targetIndex;
        }
        public KlondikeMove(MoveType type)
        {
            Type = type;
        }
        public enum MoveType
        {
            ToSuitStack,
            ToCardStack,
            DrawCard,
            ResetDrawPile,
            TurnCard
        }

        public MoveType Type { get; private set; }
        public int TargetIndex { get; private set; }

        public override string ToString()
        {
            return $"{Type} to index {TargetIndex}";
        }
    }
}
