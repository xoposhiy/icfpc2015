using Lib.Finder;
using Lib.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lib.Intelligence
{
    public class OracleServices
    {
        public static IEnumerable<UnitPosition> GetAllUnitPositions(Map map)
        {
            for (int x = 0; x < map.Width; x++)
                for (int y = 0; y < map.Height; y++)
                    for (int rot = 0; rot < 6; rot++)
                        yield return new UnitPosition { Angle = rot, Point = new System.Drawing.Point(x, y) };
        }

        public static IEnumerable<Directions> GetAllDirections()
        {
            yield return Directions.E;
            yield return Directions.SE;
            yield return Directions.SW;
            yield return Directions.W;
            yield return Directions.CW;
            yield return Directions.CCW;
        }

    }
}
