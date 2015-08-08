using System;
using System.Collections.Immutable;
using System.Drawing;
using System.Linq;
using NUnit.Framework;

namespace Lib.Models
{
    public class Map
    {
        public readonly ImmutableStack<Unit> NextUnits;
        public readonly int Height;
        public readonly int Width;
        public readonly int Id;
        public readonly Scores Scores;
        public bool[,] Filled { get; }

        public PositionedUnit Unit { get; }

        public Map(int id, bool[,] filled, ImmutableStack<Unit> nextUnits, Scores scores)
            : this(id, filled,
                  PositionNewUnit(filled.GetLength(0), nextUnits),
                  nextUnits.TryPop(), scores)
        {
        }

        public Map(int id, bool[,] filled, PositionedUnit unit, ImmutableStack<Unit> nextUnits, Scores scores)
        {
            Id = id;
            NextUnits = nextUnits;
            Width = filled.GetLength(0);
            Height = filled.GetLength(1);
            Filled = filled;
            Unit = unit.Members.All(IsValid) ? unit : PositionedUnit.Null;
            Scores = scores;
        }

        public static PositionedUnit PositionNewUnit(int width, ImmutableStack<Unit> nextUnits)
        {
            if (nextUnits.IsEmpty) return PositionedUnit.Null;
            var u = nextUnits.Peek();
            var topmostY = u.Displacements[0].Min(m => m.ToMap().Y);
            var pos = new PositionedUnit(u, 0, new Point(0, -topmostY));
            var leftMargin = pos.Members.Min(m => m.X);
            var rightMargin = width - 1 - pos.Members.Max(m => m.X);
            var newX = (rightMargin - leftMargin) / 2;
            return new PositionedUnit(u, 0, new Point(newX, -topmostY));
        }

        public bool IsOver => ReferenceEquals(Unit, PositionedUnit.Null);


        public Map LockUnit()
        {
            if (IsOver) return this;
            bool[,] f = (bool[,])Filled.Clone();
            foreach (var cell in Unit.Members)
                f[cell.X, cell.Y] = true;

            var ls_old = Scores.ClearedLinesCountAtThisMap;
            var ls = RemoveFilledLines(f);
            var size = Unit.Members.Count();

            var points = size + 100 * (1 + ls) * ls / 2;
            var line_bonus = 0;
            if (ls_old > 1)
                line_bonus = (int)Math.Floor((ls_old - 1) * points / 10f);

            var newScores = new Scores(Scores.TotalScores + points + line_bonus, ls);

            return new Map(Id, f, NextUnits, newScores);
        }

        private int RemoveFilledLines(bool[,] map)
        {
            var removedLines = 0;
            var width = map.GetLength(0);
            var height = map.GetLength(1);
            for (int y = height - 1; y >= 0; y--)
            {
                if (removedLines > 0)
                {
                    for (int x = 0; x < width; x++)
                        map[x, y] = y >= removedLines && map[x, y - removedLines];
                }
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
            return IsValidPosition(Unit.Move(direction));
        }

        public bool IsValidPosition(PositionedUnit unit)
        {
            return unit.Members.All(IsValid);
        }

        private bool IsValid(Point p)
        {
            return p.X.InRange(0, Width - 1)
                   && p.Y.InRange(0, Height - 1)
                   && !Filled[p.X, p.Y];
        }

        public Map Move(Directions dir)
        {
            return IsSafeMovement(dir)
                       ? DoMove(dir)
                       : LockUnit();
        }

        public Map Move(char c)
        {
            return Move(c.ToDirection());
        }

        private Map DoMove(Directions dir)
        {
            var nextUnit = Unit.Move(dir);
            return new Map(Id, Filled, nextUnit, NextUnits, new Scores(Scores.TotalScores, 0));
        }
    }

    [TestFixture]
    public class PositionNewUnit
    {
        [Test]
        public void One()
        {
            var units = ImmutableStack<Unit>.Empty.Push(new Unit(new[] {new Point(-1, 0), new Point(0, 1)}, new Point(0, 0)));
            var pos = Map.PositionNewUnit(4, units);
            Assert.AreEqual(new Point(2, 0), pos.Position.Point);
        }
        
        [Test]
        public void Two()
        {
            var units = ImmutableStack<Unit>.Empty.Push(new Unit(new[] {new Point(-1, 0), new Point(0, 1)}, new Point(0, 0)));
            var pos = Map.PositionNewUnit(5, units);
            Assert.AreEqual(new Point(2, 0), pos.Position.Point);
        }
    }
}