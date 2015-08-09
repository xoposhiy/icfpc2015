using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Lib.Models;

namespace Lib.Intelligence
{
    public class OracleServices
    {
        public static IEnumerable<UnitPosition> GetAllUnitPositions(Map map)
        {
            for (var x = 0; x < map.Width; x++)
            {
                for (var y = 0; y < map.Height; y++)
                {
                    for (var rot = 0; rot < map.Unit.Unit.Period; rot++)
                        yield return new UnitPosition {Angle = rot, Point = new Point(x, y)};
                }
            }
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

        public static IEnumerable<OracleSuggestion> GetAllFinalPositions(Map map)
        {
            foreach (var position in GetAllUnitPositions(map))
            {
                var positionedUnit = map.Unit.WithNewPosition(position);
                if (!map.IsValidPosition(positionedUnit)) continue;

                var lockableDirections = GetAllDirections()
                    .Where(dir => !map.IsValidPosition(positionedUnit.Move(dir)));
                foreach (var dir in lockableDirections)
                {
                    yield return new OracleSuggestion(position, dir);
                    break;
                }
            }
        }
    }
}