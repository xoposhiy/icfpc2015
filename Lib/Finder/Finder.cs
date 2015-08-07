using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lib.Finder
{
    class PointInt
    {
        public int x { get; set; }
        public int y { get; set; }

        public PointInt(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public static PointInt operator + (PointInt left, PointInt right)
        {
            return new PointInt(left.x + right.x, left.y + right.y);
        }

        public static PointInt operator - (PointInt left, PointInt right)
        {
            return new PointInt(left.x - right.x, left.y - right.y);
        }

        public PointInt Rotate()
        {

        }

        public PointInt Rotate(int angle)
        {
            var point = this;
            for (int i = 0; i < angle; i++)
                point = point.Rotate();
            return point;
        }
    }

    class Unit
    {
        private PointInt[] points;
        public int period { get; set; }

        public Unit(PointInt[] points, PointInt pivot)
        {
            this.points = points;
            for (int i = 0; i < this.points.Length; i++)
                this.points[i] -= pivot;
            period = GetPeriod();
        }

        private Unit(PointInt[] points, PointInt pivot, int period)
        {
            this.points = points;
            for (int i = 0; i < this.points.Length; i++)
                this.points[i] -= pivot;
            this.period = period;
        }

        private int GetPeriod()
        {
            var rotated = new PointInt[points.Length];
            for (int i = 0; i < points.Length; i++)
                rotated[i] = points[i];

            for (int i = 1; ; i++)
            {
                for (int j = 0; j < points.Length; j++)
                    rotated[j] = rotated[j].Rotate();
                bool ok = true;
                for (int k = 0; k < points.Length && ok; k++)
                    if (points[k].x != rotated[k].x || points[k].y != rotated[k].y)
                        ok = false;
                if (ok)
                    return i;
            }
        }

        public PointInt[] FixAt(State state)
        {
            var result = new PointInt[points.Length];
            for (int i = 0; i < points.Length; i++)
                result[i] = state.position + points[i].Rotate(state.angle);
            return result;
        }

        public PointInt GetStartPosition(int width)
        {
            int minX = int.MaxValue, maxX = int.MinValue;
            int minY = int.MaxValue;
            for (int i = 0; i < points.Length; i++)
            {
                minX = Math.Min(minX, points[i].x);
                maxX = Math.Max(maxX, points[i].x);
                minY = Math.Min(minY, points[i].y);
            }
            int prefixX = ((width - maxX) + minX) / 2;
            return new PointInt(prefixX, -minY);
        }
    }

    class State
    {
        public PointInt position { get; set; }
        public int mask { get; set; }
        public int angle { get; set; }
    }

    class TransitionInfo
    {
        public State state { get; set; }
        public char operation { get; set; }
    }

    class Movement
    {
        public PointInt delta { get; set; }
        public char operation { get; set; }
    }

    class StateArray<T>
    {
        T[,,,] array;

        public StateArray(int width, int height, int period)
        {
            array = new T[width, height, 1 << period, period];
        }

        public T this[State state]
        {
            get
            {
                return array[state.position.x, state.position.y, state.mask, state.angle];
            }
            set
            {
                array[state.position.x, state.position.y, state.mask, state.angle] = value;
            }
        }
    }

    class Finder
    {
        private readonly Movement[][] movements = new Movement[][]
        {
            new Movement[]
            {
                new Movement { delta = new PointInt(-1, 0), operation = 'W' },
                new Movement { delta = new PointInt(1, 0), operation = 'E' },
                new Movement { delta = new PointInt(-1, 1), operation = 'S' },
                new Movement { delta = new PointInt(0, 1), operation = 'D' }
            },
            new Movement[]
            {
                new Movement { delta = new PointInt(-1, 0), operation = 'W' },
                new Movement { delta = new PointInt(1, 0), operation = 'E' },
                new Movement { delta = new PointInt(0, 1), operation = 'S' },
                new Movement { delta = new PointInt(1, 1), operation = 'D' }
            }
        };

        private bool CanBePlaced(bool[,] field, PointInt[] points)
        {
            int width = field.GetLength(0);
            int height = field.GetLength(1);
            foreach (var point in points)
            {
                if (point.x < 0 || point.x >= width || point.y < 0 || point.y >= height)
                    return false;
                if (field[point.x, point.y])
                    return false;
            }
            return true;
        }

        private void DFS(State state, bool[,] field, Unit figure, StateArray<TransitionInfo> parents)
        {
            foreach (var movement in movements[state.position.y % 2])
            {
                var to = new State { position = state.position + movement.delta, mask = state.mask, angle = state.angle };
                if (!CanBePlaced(field, figure.FixAt(state)) || parents[to] != null)
                    continue;
                parents[to] = new TransitionInfo { state = state, operation = movement.operation };
                DFS(state, field, figure, parents);
            }

            for (int delta = -1; delta <= 1; delta += 2)
            {
                int angle = (state.angle + delta + figure.period) % figure.period;
                if ((state.mask & (1 << angle)) != 0)
                    continue;

                var to = new State { position = state.position, mask = state.mask | angle, angle = angle };
                if (!CanBePlaced(field, figure.FixAt(state)) || parents[to] != null)
                    continue;
                parents[to] = new TransitionInfo { state = state, operation = delta == -1 ? 'L' : 'R' };
                DFS(state, field, figure, parents);
            }
        }

        private string RestorePath(State state, StateArray<TransitionInfo> parents)
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

        public string GetPath(bool[,] field, Unit figure, PointInt target)
        {
            var startState = new State { position = figure.GetStartPosition(field.GetLength(0)), mask = 1, angle = 0 };
            if (!CanBePlaced(field, figure.FixAt(startState)))
                return null;

            var parents = new StateArray<TransitionInfo>(field.GetLength(0), field.GetLength(1), figure.period);
            parents[startState] = new TransitionInfo { state = startState, operation = '0' };

            DFS(startState, field, figure, parents);

            for (int i = 0; i < figure.period; i++)
            {
                var path = RestorePath(new State { position = target, mask = 1 << i }, parents);
                if (path != null)
                    return path;
            }
            return null;
        }
    }
}
