using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lib.Models;

namespace Lib.Intelligence
{
    public static class Metrics
    {
        public static double ShouldNotCreateSimpleHoles(Map before, Map after, PositionedUnit unit)
        {
            var score = 
                unit.Members.Max(m => m.Y) // чем ниже нижняя точка, тем лучше 
                + unit.Members.Average(m => m.Y) / 10.0 // чем ниже центр масс, тем лучше
                + unit.Members.Max(m => m.X)*0.2 / after.Width; // лучше двигать фигуры в одну сторону при прочих равных, а не кидать посередине
            // последний коэффициент при maxX подобран несколькими запусками ArensTests.
            // остальные выбраны наобум.
            if (after.Scores.ClearedLinesCountAtThisMap > 0)
                return score + 100;
            for (int i = -1; i < unit.Rectangle.Width + 1; i++)
            {
                for (int j = -1; j < unit.Rectangle.Height + 1; j++)
                {
                    var point = new Point(unit.Rectangle.X + i, unit.Rectangle.Y + j);
                    if(point.X.InRange(0, after.Width - 1)
                       && (point.Y.InRange(0, after.Height - 1) && after.IsSimpleHole(point)))
                        return score - 3; // за дырку штраф эквивалентный трем позициям по Y.
                }
            }
            return score;
        }
        
        public static double ShouldEraseLines(Map before, Map after, PositionedUnit unit)
        {

            if (after.Scores.ClearedLinesCountAtThisMap == 0) return 0;
            if (after.Scores.ClearedLinesCountAtThisMap == 1) return 0.3;
            if (after.Scores.ClearedLinesCountAtThisMap == 2) return 0.8;
            return 1;
        }


        /// new metrics
        
        public static double GoDown(Map before, Map after, PositionedUnit unit)
        {
            var result= (double)unit.Members.Average(z => z.Y) / after.Height;
            return result;
        }

        public static double MinimizeClosureIndex(Map before, Map after, PositionedUnit unit)
        {
            var beforeIndex = ClosureIndex(before, unit.Rectangle.X - 2, unit.Rectangle.Right + 2, unit.Rectangle.Y - 2, unit.Rectangle.Bottom + 2);
            var afterIndex = ClosureIndex(after, unit.Rectangle.X - 2, unit.Rectangle.Right + 2, unit.Rectangle.Y - 2, unit.Rectangle.Bottom + 2);

            if (afterIndex > beforeIndex) return 0;
            if (beforeIndex < 0.001) return 1;
            var result = 1 - afterIndex/beforeIndex;
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

        public static double ClosureIndex(Map map, int xmin, int xmax, int ymin, int ymax)
        {
            int pointsCount = 0;
            double closure = 0;
            for (int x = xmin; x <= xmax; x++)
                for (int y = ymin; y <= ymax; y++)
                {
                    if (!map.IsInside(new Point(x, y))) continue;
                    pointsCount++;
                    if (map.Filled[x, y]) continue;
                    closure += ClosureIndex(x, y, map);
                }
            return closure / pointsCount;
        }

        public static double ClosureIndex(int x, int y, Map map)
        {
            if (map.Filled[x, y]) return 0;
            Point[] cap = null;
            if (y % 2 == 0) cap = new[] { new Point(x - 1, y), new Point(x - 1, y - 1), new Point(x, y - 1), new Point(x + 1, y) };
            else cap = new[] { new Point(x - 1, y), new Point(x , y - 1), new Point(x +1, y - 1), new Point(x + 1, y) };
            int problems = cap.Where(p => !map.IsInside(p) || map.Filled[p.X, p.Y]).Count();
            return problems / 4.0;
        }


    }
}
