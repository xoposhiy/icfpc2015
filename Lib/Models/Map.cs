using System;
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

        public Map(int id, bool[,] filled, ImmutableStack<Unit> nextUnits)
            : this(id, filled, PositionNewUnit(filled.GetLength(0), nextUnits), nextUnits)
        {
        }

        private static PositionedUnit PositionNewUnit(int width, ImmutableStack<Unit> nextUnits)
        {
            if (nextUnits.IsEmpty) return null;
            var u = nextUnits.Peek();
            var topmostY = u.Members.Min(m => m.Y);
            var pivotPos = (width - 2 - u.Members.Max(m => m.X) - u.Members.Min(m => m.X)) / 2;
            return new PositionedUnit(u, 0, new Point(pivotPos, -topmostY));
        }

        public Map(int id, bool[,] filled, PositionedUnit unit, ImmutableStack<Unit> nextUnits)
        {
            Id = id;
            NextUnits = nextUnits;
            Width = filled.GetLength(0);
            Height = filled.GetLength(1);
            Filled = filled;
            Unit = unit.Members.All(IsValid) ? unit : PositionedUnit.Null;
        }

        public bool IsOver => Unit == PositionedUnit.Null;

        public bool[,] Filled { get; private set; }

        public PositionedUnit Unit { get; private set; }

        public Map LockUnit()
        {
            if (IsOver) return this;
            bool[,] f = (bool[,])Filled.Clone();
            foreach (var cell in Unit.Members)
                f[cell.X, cell.Y] = true;
            return new Map(Id, f, NextUnits.Pop());
        }

        public bool IsSafeMovement(Directions direction)
        {
            var nextUnit = Unit.Move(direction);
            return nextUnit.Members.All(IsValid);
        }

        private bool IsValid(Point p)
        {
            return p.X.InRange(0, Width - 1)
                && p.Y.InRange(0, Height - 1)
                && !Filled[p.X, p.Y];
        }

        public Map Move(Directions dir)
        {
            if (IsOver) return this;
            return IsSafeMovement(dir)
                ? new Map(Id, Filled, Unit.Move(dir), NextUnits)
                : LockUnit();
        }
    }
}