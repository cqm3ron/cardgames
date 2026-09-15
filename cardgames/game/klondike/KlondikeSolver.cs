using cardgames.core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using static cardgames.game.klondike.KlondikeMove;
using static cardgames.game.klondike.KlondikeState;

namespace cardgames.game.klondike
{
    internal static class KlondikeSolver
    {
        public static List<KlondikeMove>? Solve(KlondikeState state, HashSet<KlondikeState> visitedStates, List<KlondikeMove> solutionPath)
        {
            if (state.CheckSolveState()) return solutionPath; // solved; return solution
            if (visitedStates.Contains(state)) return null;

            visitedStates.Add(state);

            List<KlondikeMove> possibleMoves = state.GetAllPossibleMoves();
            if (possibleMoves.Count == 0) return null; // no more moves to try for this state; backtrack to previous state

            foreach (KlondikeMove move in possibleMoves)
            {
                KlondikeState nextState = state.Clone();

                if (nextState.TryMakeMove(move))
                {
                    List<KlondikeMove> nextPath = new(solutionPath) { move };

                    List<KlondikeMove>? result = Solve(nextState, visitedStates, nextPath);
                    if (result != null) return result;
                }
            }

            return null;
        }
    }
}