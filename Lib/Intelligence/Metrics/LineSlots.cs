using Lib.Models;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lib.Intelligence.Metrics
{
    [TestFixture]
   public  class LineSlots
    {
        const int maxLineSlotLength = 3;



        static IEnumerable<Point> GetPath(int startX, int startY, double dx, double dy)
        {
            var geo = new Point(startX, startY).ToGeometry();
            var deltaPoint = new PointF((float)dx, (float)dy);
            while (true)
            {
                geo = geo.Add(deltaPoint);
                yield return geo.ToMap();
            }
        }

        static IEnumerable<Point> GetPath(int startX, int startY, Directions dir)
        {
            switch (dir)
            {
                case Directions.E: return GetPath(startX, startY, Geometry.Width, 0);
                case Directions.SE: return GetPath(startX, startY, Geometry.Width/2, Geometry.YOffset);
                case Directions.SW: return GetPath(startX, startY, -Geometry.Width/2, Geometry.YOffset);
                default: throw new Exception();
            }
        }

        [TestCase(6, 4, Directions.E, 7, 4, 8, 4, 9, 4)]
        [TestCase(6, 3, Directions.E, 7, 3, 8, 3, 9, 3)]
        [TestCase(7, 4, Directions.SE, 7, 5, 8, 6, 8, 7)]
        [TestCase(7, 3, Directions.SE, 8, 4, 8, 5, 9, 6)]
        [TestCase(7, 4, Directions.SW, 6, 5, 6, 6, 5, 7)]
        [TestCase(6, 3, Directions.SW, 6, 4, 5, 5, 5, 6)]
        public void TestGetPath(int startX, int startY, Directions dir, int x1, int y1, int x2, int y2, int x3, int y3)
        {
            var expected = new[] { x1, y1, x2, y2, x3, y3 };
            var given = GetPath(startX, startY, dir).Take(3).SelectMany(p => new[] { p.X, p.Y }).ToArray();
            Assert.AreEqual(expected, given);
        }

        static int GetPathLength(Map map, int startX, int startY, Directions dir)
        {
            int length = 0;
            foreach (var p in GetPath(startX, startY, dir))
            {
                if (!map.IsInside(p)) break;
                if (map.Filled[p.X, p.Y]) break;
                length++;
                if (length > maxLineSlotLength) return -1;
            }
            return length;
        }

        static int ResultingLength(params int[] lengthes)
        {
            if (lengthes.All(z => z < 0)) return -1;
            return lengthes.Where(z => z >= 0).Max();
        }

        static double FindLinearityIndex(Map map)
        {
            int count = 0;
            double sum = 0;
            for (int y = 0; y < map.Height; y++)
                for (int x = 0; x < map.Width; x++)
                {
                    if (map.Filled[x, y]) continue;
                    var length = ResultingLength(
                        GetPathLength(map, x, y, Directions.E),
                        GetPathLength(map, x, y, Directions.SE),
                        GetPathLength(map, x, y, Directions.SW));
                    if (length < 0) continue;
                    sum += length;
                    count++;
                }
            return sum / count;
        }

        public static double Maximize(Map before, Map after, PositionedUnit unit)
        {
            var beforeIndex = FindLinearityIndex(before);
            var afterIndex = FindLinearityIndex(after);
            if (afterIndex < beforeIndex) return 0;
            if (beforeIndex < 0.01) return 1;
            return Math.Min(afterIndex / beforeIndex, 1);
        }

    }
}
