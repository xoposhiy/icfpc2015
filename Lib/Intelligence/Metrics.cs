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
            for (int i = -1; i < unit.Rectangle.Width + 2; i++)
            {
                for (int j = -1; j < unit.Rectangle.Height + 2; j++)
                {
                    var point = new Point(unit.Rectangle.X + i, unit.Rectangle.Y + j);
                    if((point.X).InRange(0, map.Width - 1)
                       && (point.Y.InRange(0, map.Height - 1) && map.IsSimpleHole(point)))
                        return 0;
                }
            }
            return unit.Position.Point.Y;
        }
    }
}
