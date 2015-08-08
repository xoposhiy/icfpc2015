using System;
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
                new Movement {delta = new Point(-1, 0), operation = 'p'},
                new Movement {delta = new Point(1, 0), operation = 'b'},
                new Movement {delta = new Point(-1, 1), operation = 'a'},
                new Movement {delta = new Point(0, 1), operation = 'l'}
            },
            new[]
            {
                new Movement {delta = new Point(-1, 0), operation = 'p'},
                new Movement {delta = new Point(1, 0), operation = 'b'},
                new Movement {delta = new Point(0, 1), operation = 'a'},
                new Movement {delta = new Point(1, 1), operation = 'l'}
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

        private static void DFS(UnitState state, bool[,] field, Unit figure, StateArray<TransitionInfo> parents)
        {
            foreach (var movement in movements[state.position.Y % 2])
            {
                var to = new UnitState {position = state.position.Add(movement.delta), angle = state.angle};
                if (!CanBePlaced(field, figure.FixAt(to)) || parents[to] != null)
                    continue;
                parents[to] = new TransitionInfo {state = state, operation = movement.operation};
                DFS(to, field, figure, parents);
            }

            for (var delta = -1; delta <= 1; delta += 2)
            {
                var angle = (state.angle + delta + figure.Period) % figure.Period;
                var to = new UnitState {position = state.position, angle = angle};
                if (!CanBePlaced(field, figure.FixAt(to)) || parents[to] != null)
                    continue;
                parents[to] = new TransitionInfo {state = state, operation = delta == 1 ? 'd' : 'k'};
                DFS(to, field, figure, parents);
            }
        }

        private static string RestorePath(UnitState state, StateArray<TransitionInfo> parents)
        {
            if (parents[state] == null)
                return null;
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

        public static Directions CharToDirection(char c)
        {
            switch (char.ToUpperInvariant(c))
            {
                case 'p':
                    return Directions.W;
                case 'b':
                    return Directions.E;
                case 'a':
                    return Directions.SW;
                case 'l':
                    return Directions.SE;
                case 'd':
                    return Directions.CCW;
                case 'k':
                    return Directions.CW;
                default:
                    throw new ArgumentException(c.ToString());
            }
        }

        public static char DirectionToChar(Directions d)
        {
            switch (d)
            {
                case Directions.W:
                    return 'p';
                case Directions.E:
                    return 'b';
                case Directions.SW:
                    return 'a';
                case Directions.SE:
                    return 'l';
                case Directions.CCW:
                    return 'd';
                case Directions.CW:
                    return 'k';
                default:
                    throw new ArgumentException();
            }
        }

        public static Directions[] StringToDirections(string path)
        {
            return path.Select(CharToDirection).ToArray();
        }

        public static string GetPath(bool[,] field, Unit figure, UnitState target)
        {
            target.angle %= figure.Period;

            var startState = new UnitState {position = figure.GetStartPosition(field.GetLength(0)), angle = 0};
            if (!CanBePlaced(field, figure.FixAt(startState)))
                return null;

            var parents = new StateArray<TransitionInfo>(field.GetLength(0), field.GetLength(1), figure.Period);
            parents[startState] = new TransitionInfo {state = startState, operation = '0'};

            DFS(startState, field, figure, parents);

            return RestorePath(target, parents);
        }
    }
}