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
        public static double ShouldNotCreateSimpleHoles(Map map, PositionedUnit unit)
        {
            if (map.Scores.ClearedLinesCountAtThisMap > 0)
                return 100500;
            for (int i = -1; i < unit.Rectangle.Width + 1; i++)
            {
                for (int j = -1; j < unit.Rectangle.Height + 1; j++)
                {
                    var point = new Point(unit.Rectangle.X + i, unit.Rectangle.Y + j);
                    if(point.X.InRange(0, map.Width - 1)
                       && (point.Y.InRange(0, map.Height - 1) && map.IsSimpleHole(point)))
                        return 0;
                }
            }
//            return unit.Position.Point.Y;
            return unit.Members.Max(m => m.Y) + unit.Members.Average(m => m.Y) / 10.0;
        }
    }
}
