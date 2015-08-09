using Lib.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lib.Intelligence.Metrics
{
    public class NewLineSlots
    {
        public static bool AtLine(Map map, Point p)
        {
            var end = new Point(map.Width/2, map.Height - 1);
            var dp = end.Sub(p);
            if ( end.Y % 2 == 0)
                return dp.X == (dp.Y + 1) / 2;
            else 
                return dp.X == dp.Y / 2;
        }

        public static int CheckFillness(Map map)
        {
            for (int y=map.Height-1;y>=0;y--)
            {
                for (int x = 0; x < map.Width; x++)
                    if (map.Filled[x, y] != AtLine(map, new Point(x, y)))
                        return map.Height - 1 - y;
            }
            return map.Height - 1;
        }

        public static double Check(Map before, Map after, PositionedUnit unit)
        {
            if (!before.NextUnits.Any(z => z.IsLine))
                return 0;
            if (!unit.Unit.IsLine)
            {
                if (unit.Members.Any(p => AtLine(before, p)))
                    return -1;
                return 0;
            }
            else
            {
                if (CheckFillness(before) < unit.Members.Count())
                    return -1;
                if (unit.Members.All(p => AtLine(before, p)))
                    return 1;
                return 0;
            }
        }
    }
}
