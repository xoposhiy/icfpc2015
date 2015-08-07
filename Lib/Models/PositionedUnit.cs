using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Lib.Models
{
    public class PositionedUnit
    {
        public PositionedUnit(Unit unit, int rotationIndex, Point pivotLocation)
        {
            Unit = unit;
            RotationIndex = rotationIndex;
            PivotLocation = pivotLocation;
        }

        public IEnumerable<Point> Members
        {
            get
            {
                if (PivotLocation.Y % 2 == 0)
                    return Unit.Rotations[RotationIndex].Select(p => p.Add(PivotLocation));
                else
                    return Unit.Rotations[RotationIndex].Select(p => p.Add(PivotLocation).Add(new Point(p.Y % 2 == 0 ? 0 : 1, 0)));
            }
        }

        public PositionedUnit Move(Directions direction)
        {
            switch (direction)
            {
                case Directions.CW:
                    return new PositionedUnit(Unit, (RotationIndex + 1) % 6, PivotLocation);
                case Directions.CCW:
                    return new PositionedUnit(Unit, (RotationIndex + 5) % 6, PivotLocation);
                default:
                    return new PositionedUnit(Unit, RotationIndex, PivotLocation.Move(direction));
            }
        }

        public readonly Unit Unit;
        public readonly int RotationIndex;
        public readonly Point PivotLocation;
    }
}