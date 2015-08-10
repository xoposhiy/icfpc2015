using Lib.Models;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Lib.Models;

namespace Lib.Intelligence.Metrics
{
    public class ClosureIndex
    {
        struct ClosureIndexChange
        {
            public int before;
            public int after;
        }


        public static double Minimize(Map before, Map after, PositionedUnit unit)
        {
            var xmin = Math.Max(0, unit.Rectangle.X - 2);
            var xmax = Math.Min(before.Width, unit.Rectangle.Right + 2);
            var ymin = Math.Max(0, unit.Rectangle.Y - 2);
            var ymax = Math.Min(before.Height, unit.Rectangle.Bottom + 2);

            var dict = new Dictionary<int, int>();
            for (int i = -4; i <= 4; i++)
                dict[i] = 0;

            for(int x= xmin;x< xmax;x++)
                for (int y= ymin;y<ymax;y++)
                {
                    var delta = FindClosureIndex(x, y, after) - FindClosureIndex(x, y, before);
                    dict[delta]++;
                }
            double factor = 1;
            double result = 0;
            for (int i = 4; i > 0; i--)
            {
                result -= dict[i] * factor;
                result += dict[-i] * factor;
                factor /= 2;
            }

            return result;
        }

        public static double Index(Map _, Map map, PositionedUnit unit)
        {
            var bads =
                from x in Enumerable.Range(0, map.Width)
                from y in Enumerable.Range(0, map.Height)
                where !map.Filled[x, y]
                let cap = GetSmallCap(x, y).Where(map.IsInside)
                let capSize = cap.Count()
                let filledCapSize = cap.Count(p => map.Filled[p.X, p.Y])
                select CapPenalty(filledCapSize, capSize, x == 0 || x == map.Width-1);
            double max = map.Width * map.Height;
            return (max  - bads.Sum()) / max;
        }

        private static double CapPenalty(int filledCapSize, int capSize, bool edge)
        {
            if (edge && filledCapSize > 0) return 10;
            return filledCapSize == capSize ? 1 : filledCapSize > 0 ? 0.5 : 0;
        }

        //public static bool IsConnected(int x1, int y1, int x2, int y2)
        //{
        //    if (y1 == y2 && Math.Abs(x1 - x2) == 1) return true;
        //    if (y1 % 2 == 1 && Math.Abs(y1-y2)==1 && (x2 == x1 || x2 == x1 + 1)) return true;
        //    if (y1 % 2 == 0 && Math.Abs(y1 - y2) == 1 && (x2 == x1 || x2 == x1 - 1)) return true;
        //    return false;
        //}

        //public static IEnumerable<Point> GetAllConnectedMaybeOutsideMap(int x, int y)
        //{
        //    for (int xx = x - 1; xx <= x + 1; xx++)
        //        for (int yy = y - 1; yy <= y + 1; yy++)
        //            if (IsConnected(x, y, xx, yy))
        //                yield return new Point(xx, yy); 
        //}

        private static double FindClosureIndex(Map map, int xmin, int xmax, int ymin, int ymax)
        {
            var pointsCount = 0;
            double closure = 0;
            for (var x = xmin; x <= xmax; x++)
            {
                for (var y = ymin; y <= ymax; y++)
                {
                    if (!map.IsInside(new Point(x, y))) continue;
                    pointsCount++;
                    if (map.Filled[x, y]) continue;
                    closure += FindClosureIndex(x, y, map);
                }
            }
            return closure / pointsCount;
        }
        [TestCase(5,3,4,3,5,2,6,2,6,3)]
        [TestCase(7,2,6,2,6,1,7,1,8,2)]
        public static void TestGetCap(int x, int y, params int[] coos)
        {
            var given = GetCap(x, y).SelectMany(p => new[] { p.X, p.Y }).ToArray();
            Assert.AreEqual(coos, given);
        }
        
        static int FindClosureIndex(int x, int y, Map map)
        {
            if (map.Filled[x, y]) return 0;
            var cap = GetCap(x, y);
            int problems = cap.Where(p => !map.IsInside(p) || map.Filled[p.X, p.Y]).Count();
            return problems;
        }

        private static Point[] GetCap(int x, int y)
        {
            if (y % 2 == 0)
            {
                return new[]
                {
                    new Point(x - 1, y), new Point(x - 1, y - 1), new Point(x, y - 1), new Point(x + 1, y)
                };
            }
            return new[]
            {
                new Point(x - 1, y), new Point(x, y - 1), new Point(x + 1, y - 1), new Point(x + 1, y)
            };
        }
        private static Point[] GetSmallCap(int x, int y)
        {
            if (y % 2 == 0)
            {
                return new[]
                {
                    new Point(x - 1, y - 1), new Point(x, y - 1)
                };
            }
            return new[]
            {
                new Point(x, y - 1), new Point(x + 1, y - 1)
            };
        }
    }
}