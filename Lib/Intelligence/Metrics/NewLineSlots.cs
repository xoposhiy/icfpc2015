﻿using Lib.Models;
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
                    if (map.Filled[x, y] == AtLine(map, new Point(x, y)))
                        return map.Height - 1 - y;
            }
            return map.Height - 1;
        }

        public static double BottomFillness(Map map, double bottomPercentage)
        {
            var h = (int)(map.Height * bottomPercentage);
            double count = 0;
            for (int y = map.Height - h; y < map.Height; y++)
                for (int x = 0; x < map.Width; x++)
                    count += map.Filled[x, y] ? 1 : 0;
            return count / (map.Width * h);    

        }

        public static double Check(Map before, Map after, PositionedUnit unit)
        {
            if (!before.NextUnits.Any(z => z.IsLine))
                return 0;
            if (BottomFillness(before,0.4) > 0.8) return 0;

            var any = unit.Members.Any(p => AtLine(before, p));
            var all = unit.Members.All(p => AtLine(before, p));

            if (!unit.Unit.IsLine)
            {
                if (any)
                    return -1;
                return 0;
            }
            else
            {
                if (all)
                {
                    if (CheckFillness(before) < unit.Members.Count())
                        return -1;
                    return 1;
                }
                else if (any) return -1;
                else return 0;
            }
        }
    }
}
