using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Drawing;
using System.Linq;

namespace Lib.Models
{
    public class Map
    {
        public readonly ImmutableStack<Unit> NextUnits;
        public readonly int Height;
        public readonly int Width;
        public readonly int Id;
        public readonly ImmutableHashSet<PositionedUnit> UsedPositions;
        public readonly Scores Scores;

        public Map(int id, bool[,] filled, ImmutableStack<Unit> nextUnits, Scores scores)
            : this(id, filled, PositionNewUnit(filled.GetLength(0), nextUnits), nextUnits, ImmutableHashSet<PositionedUnit>.Empty, scores)
        {
        }

        public static PositionedUnit PositionNewUnit(int width, ImmutableStack<Unit> nextUnits)
        {
            if (nextUnits.IsEmpty) return null;
            var u = nextUnits.Peek();
            var topmostY = u.Members.Min(m => m.Y);
            var pivotPos = (width - 2 - u.Members.Max(m => m.X + (m.Y + topmostY) % 2) -
                            u.Members.Min(m => m.X - (m.Y + topmostY) % 2)) / 2;
            return new PositionedUnit(u, 0, new Point(pivotPos, -topmostY));
        }

        public Map(int id, bool[,] filled, PositionedUnit unit, ImmutableStack<Unit> nextUnits, ImmutableHashSet<PositionedUnit> usedPositions, Scores scores)
        {
            Id = id;
            NextUnits = nextUnits;
            Width = filled.GetLength(0);
            Height = filled.GetLength(1);
            Filled = filled;
            Unit = unit.Members.All(IsValid) ? unit : PositionedUnit.Null;
            UsedPositions = usedPositions.Add(Unit);
            Scores = scores;
        }

        public bool IsOver => ReferenceEquals(Unit, PositionedUnit.Null);

        public bool[,] Filled { get; }

        public PositionedUnit Unit { get; }

        public Map LockUnit()
        {
            if (IsOver) return this;
            bool[,] f = (bool[,])Filled.Clone();
            foreach (var cell in Unit.Members)
                f[cell.X, cell.Y] = true;

            var ls_old = Scores.ClearedLinesCountAtThisMap;
            var ls =RemoveFilledLines(f);
            var size = Unit.Members.Count();

            var points = size + 100 * (1 + ls) * ls / 2;
            var line_bonus = 0;
            if (ls_old > 1)
                line_bonus = (int)Math.Floor((double)((ls_old - 1) * points / 10));
            

            var newScores = new Scores(Scores.TotalScores + points + line_bonus, ls);

            return new Map(Id, f, NextUnits.Pop(),newScores);
        }

        int RemoveFilledLines(bool[,] map)
        {
            var removedLines = 0;
            var width = map.GetLength(0);
            var height = map.GetLength(1);
            for (int y=height-1;y>=0;y--)
            {
                if (removedLines > 0)
                    for (int x = 0; x < width; x++)
                        map[x, y] = y >= removedLines && map[x, y - removedLines];
                if (Enumerable.Range(0, width).All(x => map[x, y]))
                {
                    removedLines++;
                    y++;
                }
            }
            return removedLines;
        }

        public bool IsSafeMovement(Directions direction)
        {
            var nextUnit = Unit.Move(direction);
            if (IsCatastrophicState(nextUnit))
                return false;
            return nextUnit.Members.All(IsValid);
        }

        public bool IsLockingState(PositionedUnit unit)
        {
            return unit.Members.Any(z=>!IsValid(z));
        }


        public bool IsCatastrophicState(PositionedUnit unit)
        {
            return UsedPositions.Contains(unit);
        }

        public bool IsCatastrophicMove(Directions d)
        {
            if (IsOver) return true;
            var nextUnit = Unit.Move(d);
            return IsCatastrophicState(nextUnit);
        }

        private bool IsValid(Point p)
        {
            return p.X.InRange(0, Width - 1)
                && p.Y.InRange(0, Height - 1)
                && !Filled[p.X, p.Y];
        }

        public Map Move(Directions dir)
        {
            if (IsCatastrophicMove(dir)) return Die();
            return IsSafeMovement(dir)
                ? DoMove(dir)
                : LockUnit();
        }

        private Map DoMove(Directions dir)
        {
            var nextUnit = Unit.Move(dir);
            return new Map(Id, Filled, nextUnit, NextUnits, UsedPositions.Add(nextUnit), Scores);
        }

        private Map Die()
        {
            return new Map(Id, Filled, PositionedUnit.Null, NextUnits, ImmutableHashSet<PositionedUnit>.Empty, Scores);
        }
    }

    public static class MapExtensions
    {
        public static IEnumerable<Directions> ToDirections(this string s)
        {
            return s.ToLowerInvariant()
                .Where(c => !"\t\n\r".Contains(c))
                .Select(ToDirection);
        }
        public static Directions ToDirection(this char c)
        {
            if ("p'!.03".Contains(c)) return Directions.W;
            if ("bcefy2".Contains(c)) return Directions.E;
            if ("aghij4".Contains(c)) return Directions.SW;
            if ("lmno 5".Contains(c)) return Directions.SE;
            if ("dqrvz1".Contains(c)) return Directions.CW;
            if ("kstuwx".Contains(c)) return Directions.CCW;
            throw new Exception(c.ToString());
        }
        public static Map Move(this Map map, IEnumerable<Directions> ds)
        {
            return ds.Aggregate(map, (m, d) => m.Move(d));
        }
    }
}