using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Lib.Finder;

namespace Lib.Models
{
    public class Unit
    {
        private const double angle = Math.PI / 3;

        private static readonly PointsComparer pointsComparer = new PointsComparer();
        public readonly int Period;
        public readonly PointF[][] Displacements;
        public readonly bool IsLine;

        public Unit(IEnumerable<Point> members, Point pivot)
        {
            Displacements = new PointF[6][];
            var pivotF = pivot.ToGeometry();
            Displacements[0] = members.Select(z => z.ToGeometry().Sub(pivotF)).ToArray();
            Array.Sort(Displacements[0], pointsComparer);
            Period = 6;

            for (int i = 1; i < 6; i++)
            {
                Displacements[i] = Displacements[i - 1]
                    .Select(z => z.Rotate(new Point(0, 0), angle))
                    .ToArray();
                Array.Sort(Displacements[i], pointsComparer);
                if (Displacements[i].Zip(Displacements[0], (p1, p2) => Math.Abs(p1.X - p2.X) + Math.Abs(p1.Y - p2.Y) < 1e-3).All(a => a))
                {
                    Period = i;
                    break;
                }
            }

            IsLine = Displacements
                .Where(z => z != null)
                .Any(z => z.All(x => x.Y== z[0].Y));
            IsLine = IsLine && members.Count() > 1;
        }
    }
}