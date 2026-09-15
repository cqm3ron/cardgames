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
                    // KlondikeDisplay.DisplayKlondikeMenu(nextState);

                    List<KlondikeMove> nextPath = new(solutionPath) { move };

                    List<KlondikeMove>? result = Solve(nextState, visitedStates, nextPath);
                    if (result != null) return result;
                }
            }

            return null;
        }


        // ============================================================================================================================
        // The below code was written using Generative AI, in order to test various solve routines. I do not take credit for this work.
        // ============================================================================================================================
        public static List<KlondikeMove>? SolveIteratively(KlondikeState initialState)
        {
            HashSet<KlondikeState> visitedStates = [];
            Stack<(KlondikeState state, List<KlondikeMove> moves, int moveIndex, List<KlondikeMove> path)> statesToVisit = [];

            if (initialState.HasBeenSolved) return []; // already solved
            if (!visitedStates.Contains(initialState))
            {
                visitedStates.Add(initialState);
                List<KlondikeMove> moves = initialState.GetAllPossibleMoves();
                if (moves.Count > 0) statesToVisit.Push((initialState, moves, 0, []));
            }

            while (statesToVisit.Count > 0)
            {
                (KlondikeState, List<KlondikeMove>, int, List<KlondikeMove>) current = statesToVisit.Pop();
                KlondikeState state = current.Item1;
                List<KlondikeMove> moves = current.Item2;
                int moveIndex = current.Item3;
                List<KlondikeMove> path = current.Item4;
                KlondikeMove move = moves[moveIndex];

                if (moveIndex >= moves.Count) continue;
                if (moveIndex < moves.Count)
                {
                    if (moveIndex + 1 < moves.Count) statesToVisit.Push((state, moves, moveIndex + 1, path)); // push the next move for this state onto the stack if there are more moves to try
                }

                KlondikeState nextState = state.Clone();
                if (nextState.TryMakeMove(move) && nextState.CheckSolveState()) return path; // if this state is solved once the move has been made, return the path that was taken to get here
                if (!visitedStates.Contains(nextState))
                {
                    visitedStates.Add(nextState);

                    List<KlondikeMove> nextPath = [.. path];
                    nextPath.Add(move);
                    var nextMoves = nextState.GetAllPossibleMoves();

                    if (nextMoves.Count > 0) statesToVisit.Push((nextState, nextMoves, 0, nextPath)); // push the next state onto the stack if there are moves to try
                }
            }

            return null;
        }

        public static List<KlondikeMove>? SolveWithDepthLimit(KlondikeState initialState, int maxDepth = 250)
        {
            if (initialState.HasBeenSolved) return [];

            var visitedStates = new HashSet<KlondikeState>();
            return DepthFirstSearch(initialState, maxDepth, 0, visitedStates);
        }

        private static List<KlondikeMove>? DepthFirstSearch(KlondikeState state, int maxDepth, int currentDepth, HashSet<KlondikeState> visitedStates)
        {
            if (state.CheckSolveState()) return [];
            if (currentDepth >= maxDepth || visitedStates.Contains(state)) return null;

            visitedStates.Add(state);

            foreach (KlondikeMove move in state.GetAllPossibleMoves())
            {
                KlondikeState nextState = state.Clone();
                if (nextState.TryMakeMove(move))
                {
                    var result = DepthFirstSearch(nextState, maxDepth, currentDepth + 1, visitedStates);
                    if (result != null)
                    {
                        result.Insert(0, move);
                        return result;
                    }
                }
            }

            return null;
        }

        public static List<KlondikeMove>? SolveWithMemoryLimit(KlondikeState initialState, int maxVisitedStates = 100000, int timeoutMs = 5000)
        {
            if (initialState.HasBeenSolved) return [];

            var visitedStates = new HashSet<KlondikeState>();
            var queue = new Queue<(KlondikeState state, List<KlondikeMove> path)>();

            queue.Enqueue((initialState, []));
            visitedStates.Add(initialState);

            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            int iterationCount = 0;

            while (queue.Count > 0 && visitedStates.Count < maxVisitedStates)
            {
                // Check timeout every 100 iterations (more efficient than checking every iteration)
                if (++iterationCount % 100 == 0 && stopwatch.ElapsedMilliseconds >= timeoutMs)
                    return null; // Timeout exceeded

                var (state, path) = queue.Dequeue();

                if (state.CheckSolveState()) return path;

                foreach (KlondikeMove move in state.GetAllPossibleMoves().OrderByDescending(m => MoveHeuristic(m)))
                {
                    // Check timeout before expensive clone operation
                    if (iterationCount % 100 == 0 && stopwatch.ElapsedMilliseconds >= timeoutMs)
                        return null;

                    KlondikeState nextState = state.Clone();
                    if (nextState.TryMakeMove(move))
                    {
                        if (!visitedStates.Contains(nextState))
                        {
                            visitedStates.Add(nextState);
                            queue.Enqueue((nextState, [..path, move]));
                        }
                    }
                }
            }

            return null;
        }

        private static int MoveHeuristic(KlondikeMove move) => move.Type switch
        {
            MoveType.ToSuitStack => 1000,
            MoveType.ToCardStack => 100,
            MoveType.TurnCard => 50,
            MoveType.DrawCard => -100,
            _ => 0
        };
    }
}