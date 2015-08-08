using Lib.Finder;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;

namespace Lib.Models
{
    public class PositionedUnit
    {
        public static PositionedUnit Null = new PositionedUnit(new Unit(new List<Point>(), new Point(0, 0)), 0, new Point(int.MaxValue, int.MaxValue));

        public PositionedUnit(Unit unit, int rotationIndex, Point pivotLocation)
        {
            Unit = unit;
            RotationIndex = (rotationIndex + Unit.Period) % Unit.Period;
            Debug.Assert(RotationIndex.InRange(0, Unit.Displacements.Length-1));
            PivotLocation = pivotLocation;
        }

        public PositionedUnit(Unit unit, UnitState state) : this(unit, state.angle, state.position)
        { }

        public PositionedUnit TranslateToState(UnitState state)
        {
            return new PositionedUnit(Unit, state);
        }
        

        protected bool Equals(PositionedUnit other)
        {
            return RotationIndex == other.RotationIndex && PivotLocation.Equals(other.PivotLocation);
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
            unchecked
            {
                return (RotationIndex * 397) ^ PivotLocation.GetHashCode();
            }
        }

        public IEnumerable<Point> Members
        {
            get
            {
                if (!RotationIndex.InRange(0, Unit.Displacements.Length - 1))
                    throw new Exception(RotationIndex.ToString());
                return
                    Unit
                    .Displacements[RotationIndex]
                    .Select(p => p.Add(PivotLocation.ToGeometry()).ToMap());
            }
        }

        public PositionedUnit Move(Directions direction)
        {
            switch (direction)
            {
                case Directions.CW:
                    return new PositionedUnit(Unit, RotationIndex + 1, PivotLocation);
                case Directions.CCW:
                    return new PositionedUnit(Unit, RotationIndex - 1, PivotLocation);
                default:
                    return new PositionedUnit(Unit, RotationIndex, PivotLocation.Move(direction));
            }
        }

        public readonly Unit Unit;
        public readonly int RotationIndex;
        public readonly Point PivotLocation;
    }
}