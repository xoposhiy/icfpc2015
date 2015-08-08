using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using Lib.Finder;

namespace Lib.Models
{
    public class PositionedUnit
    {
        public static PositionedUnit Null = new PositionedUnit(new Unit(new List<Point>(), new Point(0, 0)), 0, new Point(int.MaxValue, int.MaxValue));
        public readonly Unit Unit;
        public readonly UnitPosition Position;

        public PositionedUnit(Unit unit, int rotationIndex, Point pivotLocation)
        {
            //TODO pe
            Unit = unit;
            Position = new UnitPosition(pivotLocation, (rotationIndex + Unit.Period) % Unit.Period);
        }

        public PositionedUnit(Unit unit, UnitPosition position)
        {
            Unit = unit;
            Position = position;
        }

        protected bool Equals(PositionedUnit other)
        {
            return Equals(Position, other.Position);
        }

        public PositionedUnit WithNewPosition(UnitPosition pos)
        {   
            return new PositionedUnit(Unit, pos);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((PositionedUnit)obj);
        }

        public override int GetHashCode()
        {
            return (Position != null ? Position.GetHashCode() : 0);
        }

        public IEnumerable<Point> Members
        {
            get
            {
                return Unit
                    .Displacements[Position.Angle]
                    .Select(p => p.Add(Position.Point.ToGeometry()).ToMap());
            }
        }

        public PositionedUnit Move(Directions direction)
        {
            switch (direction)
            {
                case Directions.CW:
                    return new PositionedUnit(Unit, Position.Angle+ 1, Position.Point);
                case Directions.CCW:
                    return new PositionedUnit(Unit, Position.Angle - 1, Position.Point);
                default:
                    return new PositionedUnit(Unit, Position.Angle, Position.Point.Move(direction));
            }
        }
    }
}