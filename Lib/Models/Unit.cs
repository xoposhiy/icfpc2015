using System;
using System.Collections.Generic;
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
        public Unit(IEnumerable<Point> members, Point pivot)
        {
            Members = members.Select(c => c.Sub(pivot)).ToArray();
            Array.Sort(Members, pointsComparer);
            Period = GetPeriod();

            double cv = Math.PI / 3;
            Rotations = new List<Point>[6];
            Rotations[0] = Members.Select(z => z.Rotate(new Point(0, 0), cv)).ToList();
            for (int i = 1; i < 6; i++)
                Rotations[i] = Rotations[i - 1].Select(z => z.Rotate(new Point(0, 0), cv)).ToList();
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
                    if (Members[k].Sub(Members[0]) != rotated[k].Sub(rotated[0]))
                        ok = false;
                if (ok)
                    return i;
            }
        }

        public Point[] FixAt(UnitState unitState)
        {
            var result = new Point[Members.Length];
            for (int i = 0; i < Members.Length; i++)
                result[i] = unitState.position.Add(Members[i].Rotate(unitState.angle));
            return result;
        }

        public Point GetStartPosition(int width)
        {
            int minX = int.MaxValue, maxX = int.MinValue;
            int minY = int.MaxValue;
            for (int i = 0; i < Members.Length; i++)
            {
                minX = Math.Min(minX, Members[i].X);
                maxX = Math.Max(maxX, Members[i].X);
                minY = Math.Min(minY, Members[i].Y);
            }
            int prefixX = (width - (maxX - minX + 1)) / 2;
            return new Point(prefixX, -minY);
        }
    }
}