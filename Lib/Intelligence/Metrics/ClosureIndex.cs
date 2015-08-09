using Lib.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lib.Intelligence.Metrics
{
    public class ClosureIndex
    {
        public static double Minimize(Map before, Map after, PositionedUnit unit)
        {
            var beforeIndex = FindClosureIndex(before, unit.Rectangle.X - 2, unit.Rectangle.Right + 2, unit.Rectangle.Y - 2, unit.Rectangle.Bottom + 2);
            var afterIndex = FindClosureIndex(after, unit.Rectangle.X - 2, unit.Rectangle.Right + 2, unit.Rectangle.Y - 2, unit.Rectangle.Bottom + 2);

            if (afterIndex > beforeIndex) return 0;
            if (beforeIndex < 0.001) return 1;
            var result = 1 - afterIndex / beforeIndex;
            return result;
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

        static double FindClosureIndex(Map map, int xmin, int xmax, int ymin, int ymax)
        {
            int pointsCount = 0;
            double closure = 0;
            for (int x = xmin; x <= xmax; x++)
                for (int y = ymin; y <= ymax; y++)
                {
                    if (!map.IsInside(new Point(x, y))) continue;
                    pointsCount++;
                    if (map.Filled[x, y]) continue;
                    closure += FindClosureIndex(x, y, map);
                }
            return closure / pointsCount;
        }

        static double FindClosureIndex(int x, int y, Map map)
        {
            if (map.Filled[x, y]) return 0;
            Point[] cap = null;
            if (y % 2 == 0) cap = new[] { new Point(x - 1, y), new Point(x - 1, y - 1), new Point(x, y - 1), new Point(x + 1, y) };
            else cap = new[] { new Point(x - 1, y), new Point(x, y - 1), new Point(x + 1, y - 1), new Point(x + 1, y) };
            int problems = cap.Where(p => !map.IsInside(p) || map.Filled[p.X, p.Y]).Count();
            return problems / 4.0;
        }

    }
}
