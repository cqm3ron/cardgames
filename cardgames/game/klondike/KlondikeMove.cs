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

        public enum MoveType
        {
            ToSuitStack,
            ToCardStack,
            DrawCard,
            ResetDrawPile
        }

        public MoveType Type { get; private set; }
        public int TargetIndex { get; private set; }

        //public static List<KlondikeMove> GetPossibleMoves(KlondikeState state)
        //{

        //}
    }
}
