using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lib.Models;

namespace Lib.Intelligence.Metrics
{
    public static class SimpleMetrics
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

        
        public static double EraseLines(Map before, Map after, PositionedUnit unit)
        {

            if (after.Scores.ClearedLinesCountAtThisMap == 0) return 0;
            if (after.Scores.ClearedLinesCountAtThisMap == 1) return 0.3;
            if (after.Scores.ClearedLinesCountAtThisMap == 2) return 0.8;
            return 1;
        }


        /// new metrics

        public static double GoDown(Map before, Map after, PositionedUnit unit)
        {
            return unit.Members.Average(z => z.Y) / after.Height;
        }
        public static double MapDown(Map before, Map after, PositionedUnit unit)
        {
            return after.AverageDepth() / after.Height;
        }


    }
}
