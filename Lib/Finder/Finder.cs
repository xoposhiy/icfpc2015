using System.Drawing;
using System.Linq;
using System.Text;
using Lib.Models;

namespace Lib.Finder
{
    public static class Finder
    {
        private static readonly Movement[][] movements =
        {
            new[]
            {
                new Movement {delta = new Point(-1, 0), operation = 'W'},
                new Movement {delta = new Point(1, 0), operation = 'E'},
                new Movement {delta = new Point(-1, 1), operation = 'S'},
                new Movement {delta = new Point(0, 1), operation = 'D'}
            },
            new[]
            {
                new Movement {delta = new Point(-1, 0), operation = 'W'},
                new Movement {delta = new Point(1, 0), operation = 'E'},
                new Movement {delta = new Point(0, 1), operation = 'S'},
                new Movement {delta = new Point(1, 1), operation = 'D'}
            }
        };

        private static bool CanBePlaced(bool[,] field, Point[] points)
        {
            var width = field.GetLength(0);
            var height = field.GetLength(1);
            foreach (var point in points)
            {
                if (point.X < 0 || point.X >= width || point.Y < 0 || point.Y >= height)
                    return false;
                if (field[point.X, point.Y])
                    return false;
            }
            return true;
        }

        private static void DFS(State state, bool[,] field, FinderUnit figure, StateArray<TransitionInfo> parents)
        {
            foreach (var movement in movements[state.position.Y % 2])
            {
                var to = new State {position = state.position.Add(movement.delta), mask = state.mask, angle = state.angle};
                if (!CanBePlaced(field, figure.FixAt(state)) || parents[to] != null)
                    continue;
                parents[to] = new TransitionInfo {state = state, operation = movement.operation};
                DFS(state, field, figure, parents);
            }

            for (var delta = -1; delta <= 1; delta += 2)
            {
                var angle = (state.angle + delta + figure.period) % figure.period;
                if ((state.mask & (1 << angle)) != 0)
                    continue;

                var to = new State {position = state.position, mask = state.mask | angle, angle = angle};
                if (!CanBePlaced(field, figure.FixAt(state)) || parents[to] != null)
                    continue;
                parents[to] = new TransitionInfo {state = state, operation = delta == -1 ? 'L' : 'R'};
                DFS(state, field, figure, parents);
            }
        }

        private static string RestorePath(State state, StateArray<TransitionInfo> parents)
        {
            var path = new StringBuilder();
            while (true)
            {
                var par = parents[state].state;
                var op = parents[state].operation;
                if (par == state)
                    break;
                path.Append(op);
                state = par;
            }
            return string.Join("", path.ToString().Reverse());
        }

        public static string GetPath(bool[,] field, FinderUnit figure, Point target)
        {
            var startState = new State {position = figure.GetStartPosition(field.GetLength(0)), mask = 1, angle = 0};
            if (!CanBePlaced(field, figure.FixAt(startState)))
                return null;

            var parents = new StateArray<TransitionInfo>(field.GetLength(0), field.GetLength(1), figure.period);
            parents[startState] = new TransitionInfo {state = startState, operation = '0'};

            DFS(startState, field, figure, parents);

            for (var i = 0; i < figure.period; i++)
            {
                var path = RestorePath(new State {position = target, mask = 1 << i}, parents);
                if (path != null)
                    return path;
            }
            return null;
        }
    }
}