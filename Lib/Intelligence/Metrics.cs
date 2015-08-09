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
            var score = 
                unit.Members.Max(m => m.Y) // чем ниже нижняя точка, тем лучше 
                + unit.Members.Average(m => m.Y) / 10.0 // чем ниже центр масс, тем лучше
                + unit.Members.Max(m => m.X)*0.2 / map.Width; // лучше двигать фигуры в одну сторону при прочих равных, а не кидать посередине
            // последний коэффициент при maxX подобран несколькими запусками ArensTests.
            // остальные выбраны наобум.
            if (map.Scores.ClearedLinesCountAtThisMap > 0)
                return score + 100;
            for (int i = -1; i < unit.Rectangle.Width + 1; i++)
            {
                for (int j = -1; j < unit.Rectangle.Height + 1; j++)
                {
                    var point = new Point(unit.Rectangle.X + i, unit.Rectangle.Y + j);
                    if(point.X.InRange(0, map.Width - 1)
                       && (point.Y.InRange(0, map.Height - 1) && map.IsSimpleHole(point)))
                        return score - 3; // за дырку штраф эквивалентный трем позициям по Y.
                }
            }
            return score;
        }

        
        public static double ShouldEraseLines(Map map, PositionedUnit unit)
        {
            var lines = 0;
            for (int y=0;y<map.Height;y++)
            {
                bool ok = true;
                for (int x=0;x<map.Height;x++)
                {
                    if (!map.Filled[x, y] && !unit.Members.Contains(new Point(x, y)))
                    {
                        ok = false;
                        break;
                    }
                }
                if (ok) lines++;
            }
            if (lines == 0) return 0;
            if (lines == 1) return 0.1;
            return 1;
        }


        public static double ShouldCountSpells()
        {

            return 0;
        }
    }
}
