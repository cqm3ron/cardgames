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

        //public static void Solve(KlondikeState state, int moveNumber = 0)
        //{
        //    if (state.CheckSolveState()) return; // TODO: return the solution path instead of just returning if solved.
        //    if (visitedStates.Contains(state)) return;
        //    visitedStates.Add(state);

        //    List<KlondikeMove> possibleMoves = state.GetAllPossibleMoves();

        //    if (moveNumber >= possibleMoves.Count) return; // no more moves to try for this state; now must try the next move in the previous state.

        //    state.SelectNthMove(moveNumber);
        //    state.TryMakeSelectedMove();



        //    Solve(state);
        //}


        public static List<KlondikeMove>? Solve(KlondikeState state, HashSet<KlondikeState> visitedStates, List<KlondikeMove> solutionPath)
        {
            if (state.CheckSolveState()) return solutionPath; // solved; return solution
            if (visitedStates.Contains(state)) return null; // already explored; skip

            visitedStates.Add(state); // mark visited in a hashset for speeeeed

            List<KlondikeMove> possibleMoves = state.GetAllPossibleMoves();

            if (possibleMoves.Count == 0) return null; // no more moves to try for this state; backtrack to previous state

            foreach (KlondikeMove move in possibleMoves)
            {
                KlondikeState stateSnapshot = state.Clone(); // create a snapshot of the current state

                bool successfulMove = state.TryMakeMove(move); // make the move
                
                List<KlondikeMove>? resultingMovesList = [];
                
                if (successfulMove)
                {
                    solutionPath.Add(move);
                    resultingMovesList = Solve(state, visitedStates, solutionPath); // recursively solve the new state

                    if (resultingMovesList != null) return resultingMovesList; // if a solution was found, return it

                    solutionPath.RemoveAt(solutionPath.Count - 1);
                    state = stateSnapshot;
                }
            }

            return null;
        }
    }       
}
