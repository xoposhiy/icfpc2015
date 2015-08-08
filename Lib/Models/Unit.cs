using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Drawing;
using System.Linq;
using Lib.Finder;

namespace Lib.Models
{
    public class Unit
    {
        private static readonly PointsComparer pointsComparer = new PointsComparer();

        public readonly int Period;
        public readonly Point[] Members;
        public readonly List<Point>[] Rotations;
        private readonly int pivotOddity;

        public Unit(IEnumerable<Point> members, Point pivot)
        {
            Members = members.Select(c => c.Sub(pivot)).ToArray();
            Array.Sort(Members, pointsComparer);
            Period = GetPeriod();
            pivotOddity = pivot.Y % 2;

            double cv = Math.PI / 3;
            Rotations = new List<Point>[6];
            Rotations[0] = Members.ToList();
            for (int i = 1; i < 6; i++)
                Rotations[i] = Rotations[i - 1].Select(z => z.Rotate(new Point(0, 0), cv)).ToList();
        }

        public Point GetFixingVector(int vectorOddity, int newPivotOddity)
        {
            if (pivotOddity == newPivotOddity || vectorOddity == 0)
                return new Point(0, 0);
            if (pivotOddity == 0 && newPivotOddity == 1)
                return new Point(1, 0);
            return new Point(-1, 0);
        }

        public bool IsSafePath(IEnumerable<Directions> path)
        {
            throw new NotImplementedException();
        }

        private int GetPeriod()
        {
            var rotated = new Point[Members.Length];
            for (int i = 0; i < Members.Length; i++)
                rotated[i] = Members[i];

            for (int i = 1; ; i++)
            {
                for (int j = 0; j < Members.Length; j++)
                    rotated[j] = rotated[j].Rotate();
                Array.Sort(rotated, pointsComparer);
                bool ok = true;
                for (int k = 0; k < Members.Length && ok; k++)
                    if (Members[k] != rotated[k])
                        ok = false;
                if (ok)
                    return i;
            }
        }

        public Point[] FixAt(UnitState unitState)
        {
            var result = new Point[Members.Length];
            for (int i = 0; i < Members.Length; i++)
            {
                Point vector = Members[i].Rotate(unitState.angle);
                result[i] = unitState.position.Add(vector).Add(GetFixingVector(vector.Y % 2, unitState.position.Y % 2));
            }
            return result;
        }

        public Point GetStartPosition(int width)
        {
            var x = Map.PositionNewUnit(width, ImmutableStack.Create(this));
            return x.PivotLocation;
        }
    }
}