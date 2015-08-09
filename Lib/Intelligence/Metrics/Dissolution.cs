using Lib.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lib.Intelligence.Metrics
{
    public class Dissolution
    {
        public static double Perform(Map before, Map after, PositionedUnit unit)
        {
            var counts = Enumerable
                .Range(0, before.Height)
                .Select(y => new { Y = y, Before = GetCountOfFilledCellsInLine(before, y) })
                .Select(z => new { Y = z.Y, Before = z.Before, Delta = GetCountOfFilledCellsInLine(after, z.Y) - z.Before })
                .OrderByDescending(z => z.Before)
                .Take(3)
                .ToList();

            double factor = 1;
            double scores = 0;
            foreach (var e in counts)
            {
                scores += e.Delta * factor;
                factor *= 0.5;
            }
      
            return scores/unit.Members.Count();
        }
        

        public static int FindFirstOccupiedLine(Map map)
        {
            for (int y=0;y<map.Height;y++)
            {
                if (Enumerable.Range(0, map.Width).Any(x => map.Filled[x, y])) return y;
            }
            return map.Height;
        }

        static double GetCountOfFilledCellsInLine(Map map, int y)
        {
            return Enumerable.Range(0, map.Width)
                .Where(x => map.Filled[x, y])
                .Count();
        }
    }
}
