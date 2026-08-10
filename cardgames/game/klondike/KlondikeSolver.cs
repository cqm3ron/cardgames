using cardgames.core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static cardgames.game.klondike.KlondikeState;

namespace cardgames.game.klondike
{
    internal static class KlondikeSolver
    {
        // TODO: Implement true solver, given state.
        // TODO: Implement heuristic solver, given state.
        // TODO: avoid loops in solvers
        // TODO: GetHint() function that returns the next best move for the player
        // TODO: use state.GetHash() function to compare states

        public static void Solve(KlondikeState state)
        {
            if (state.CheckSolveState()) return; // TODO: return the solution path instead of just returning if solved.

        }
    }       
}
