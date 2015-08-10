using System;
using System.Collections.Immutable;
using System.Drawing;
using System.Linq;
using System.Text;
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
        public readonly bool Died;
        public readonly ImmutableHashSet<PositionedUnit> UsedPositions;

        public Map(int id, bool[,] filled, ImmutableStack<Unit> nextUnits, Scores scores)
            : this(id, filled,
                   PositionNewUnit(filled.GetLength(0), nextUnits),
                   nextUnits.TryPop(), scores)
        {
        }

        private Map(int id, bool[,] filled, PositionedUnit unit, ImmutableStack<Unit> nextUnits, Scores scores)
            : this(id, filled,
                   unit,
                   nextUnits, ImmutableHashSet<PositionedUnit>.Empty.Add(unit), scores)
        {
        }

        public Map(int id, bool[,] filled, PositionedUnit unit, ImmutableStack<Unit> nextUnits, ImmutableHashSet<PositionedUnit> usedPositions, Scores scores, bool died = false)
        {
            Id = id;
            NextUnits = nextUnits;
            Width = filled.GetLength(0);
            Height = filled.GetLength(1);
            Filled = filled;
            Unit = IsValidPosition(unit) ? unit : PositionedUnit.Null;
            UsedPositions = usedPositions.Add(Unit);
            Scores = scores;
            Died = died;
        }

        public double AverageDepth()
        {
            var filled =
                from x in Enumerable.Range(0, Width)
                from y in Enumerable.Range(0, Height)
                where Filled[x, y]
                select y;
            return filled.Concat(new[] { Height + 1 }).Average();
        }

        public bool[,] Filled { get; }

        public override string ToString()
        {
            var lines = Enumerable.Range(0, Height)
                      .Select(y => (y % 2 == 1 ? " " : "")
                                   + string.Join(" ",
                                                 Enumerable
                                                     .Range(0, Width)
                                                     .Select(x => Filled[x, y] ? "#" : ".")))
                                                     .Select(line => line.ToCharArray()).ToList();
            foreach (var cell in Unit.Members)
                lines[cell.Y][cell.X * 2 + cell.Y % 2] = 'X';
            var s = string.Join("\r\n", lines.Select(l => new string(l)));
            return $"Score: {Scores}\r\n{s}";
        }

        public PositionedUnit Unit { get; }

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
            var f = (bool[,])Filled.Clone();
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

        public Map TeleportUnit(UnitPosition position)
        {
            return new Map(Id, Filled, new PositionedUnit(Unit.Unit, position), NextUnits, UsedPositions, Scores);
        }

        private int RemoveFilledLines(bool[,] map)
        {
            var removedLines = 0;
            var width = map.GetLength(0);
            var height = map.GetLength(1);
            for (var y = height - 1; y >= 0; y--)
            {
                if (removedLines > 0)
                {
                    for (var x = 0; x < width; x++)
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
            var nextUnit = Unit.Move(direction);
            return !IsCatastrophicMove(nextUnit) && IsValidPosition(nextUnit);
        }

        public bool IsValidPosition(PositionedUnit unit)
        {
            return unit.Members.All(IsValid);
        }

        public bool IsCatastrophicMove(PositionedUnit unit)
        {
            if (IsOver) return true;
            return UsedPositions.Contains(unit);
        }

        private bool IsValid(Point p)
        {
            return p.X >= 0 && p.X < Width && p.Y >= 0 && p.Y < Height && !Filled[p.X, p.Y];
        }

        public bool IsInside(Point p)
        {
            return p.X >= 0 && p.X < Width && p.Y >= 0 && p.Y < Height;
        }

        public Map Move(Directions dir)
        {
            var newUnit = Unit.Move(dir);
            if (IsCatastrophicMove(newUnit)) return Die();
            return IsValidPosition(newUnit)
                       ? DoMove(newUnit)
                       : LockUnit();
        }

        public Map Move(char c)
        {
            return Move(c.ToDirection());
        }

        private Map DoMove(PositionedUnit nextUnit)
        {
            return new Map(Id, Filled, nextUnit, NextUnits, UsedPositions.Add(nextUnit), new Scores(Scores.TotalScores, 0));
        }

        private Map Die()
        {
            return new Map(Id, Filled, PositionedUnit.Null, NextUnits, ImmutableHashSet<PositionedUnit>.Empty, new Scores(0, 0), true);
        }
    }

    [TestFixture]
    public class PositionNewUnit
    {
        [Test]
        public void One()
        {
            var units = ImmutableStack<Unit>.Empty.Push(new Unit(new[] { new Point(-1, 0), new Point(0, 1) }, new Point(0, 0)));
            var pos = Map.PositionNewUnit(4, units);
            Assert.AreEqual(new Point(2, 0), pos.Position.Point);
        }

        [Test]
        public void Two()
        {
            var units = ImmutableStack<Unit>.Empty.Push(new Unit(new[] { new Point(-1, 0), new Point(0, 1) }, new Point(0, 0)));
            var pos = Map.PositionNewUnit(5, units);
            Assert.AreEqual(new Point(2, 0), pos.Position.Point);
        }
    }
}