using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Lib.Finder;
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

        private static OracleSuggestion TryGetSugession(Map map, PositionedUnit unit)
        {
            return GetAllDirections()
                .Where(dir => !map.IsValidPosition(unit.Move(dir)))
                .Select(dir => new OracleSuggestion(unit.Position, dir, map.LockUnit()))
                .FirstOrDefault();
        }

        public static IEnumerable<OracleSuggestion> GetReachableSugessions(IFinder finder, Map map)
        {
            return
                finder.GetReachablePositions(map)
                      .Select(m => TryGetSugession(m, m.Unit))
                      .Where(s => s != null);

        }
    }
}