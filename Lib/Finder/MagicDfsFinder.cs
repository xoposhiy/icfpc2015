using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Lib.Models;

namespace Lib.Finder
{
    public class MagicDfsFinder : IFinder
    {
        private readonly IFinder dfsFinder = new DfsFinder();

        private static UnitPosition GetMidPositionByPhrase(UnitPosition target, IEnumerable<Directions> directions, int period)
        {
            int x = target.Point.X;
            int y = target.Point.Y;
            int a = target.Angle;

            foreach (var dir in directions.Reverse())
            {
                switch (dir)
                {
                    case Directions.W:
                        x++;
                        break;
                    case Directions.SW:
                        if (y % 2 == 1)
                            x++;
                        y--;
                        break;
                    case Directions.E:
                        x--;
                        break;
                    case Directions.SE:
                        if (y % 2 == 0)
                            x--;
                        y--;
                        break;
                    case Directions.CCW:
                        a = (a + 1) % period;
                        break;
                    case Directions.CW:
                        a = (a - 1 + period) % period;
                        break;
                }
            }
            return new UnitPosition(new Point(x, y), a);
        }

        private bool IsGoodPath(Map map, IEnumerable<Directions> path)
        {
            foreach (var d in path)
            {
                if (!map.IsSafeMovement(d))
                    return false;
                map = map.Move(d);
            }
            return true;
        }

        public IEnumerable<Directions> GetPath(Map map, UnitPosition target)
        {
            foreach (var phrase in Phrases.all.OrderByDescending(p => p.Length))
            {
                var directions = phrase.ToDirections().ToArray();
                int period = map.Unit.Unit.Period;
                var midPosition = GetMidPositionByPhrase(target, directions, period);
                var midPositionedUnit = new PositionedUnit(map.Unit.Unit, midPosition);
                var midMap = new Map(map.Id, map.Filled, midPositionedUnit, map.NextUnits, map.Scores);

                if (!IsGoodPath(midMap, directions))
                    continue;
                var path = dfsFinder.GetPath(map, midPosition);
                if (path == null)
                    continue;
                var result = path.Concat(directions).ToArray();
                if (IsGoodPath(map, result))
                    return result;
            }

            return dfsFinder.GetPath(map, target);
        }

        public IEnumerable<Map> GetReachablePositions(Map map)
        {
            return dfsFinder.GetReachablePositions(map);
        }
    }
}