using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Drawing;
using System.Linq;
using Lib.Models;

namespace Lib.Finder
{
    public class MagicDfsFinder : IFinder
    {
        private readonly IFinder dfsFinder = new DfsFinder();

        private static Tuple<Point, int> GetDeltaByPhrase(IEnumerable<Directions> directions, int period)
        {
            int dx = 0, dy = 0, da = 0;
            foreach (var dir in directions)
            {
                if (dir == Directions.W || dir == Directions.SW)
                    dx--;
                if (dir == Directions.E || dir == Directions.SE)
                    dx++;
                if (dir == Directions.SW || dir == Directions.SE)
                    dy++;
                if (dir == Directions.CW)
                    da = (da + 1) % period;
                if (dir == Directions.CCW)
                    da = (da - 1 + period) % period;
            }
            return Tuple.Create(new Point(dx, dy), da);
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
                var delta = GetDeltaByPhrase(directions, period);
                var newPoint = target.Point.Sub(delta.Item1);
                var newAngle = (target.Angle - delta.Item2 + period) % period;
                var midPosition = new UnitPosition(newPoint, newAngle);
                var midPositionedUnit = new PositionedUnit(map.Unit.Unit, midPosition);
                var midMap = new Map(map.Id, map.Filled, midPositionedUnit, map.NextUnits, ImmutableHashSet<PositionedUnit>.Empty, map.Scores);

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

        public IEnumerable<UnitPosition> GetReachablePositions(Map map)
        {
            return dfsFinder.GetReachablePositions(map);
        }
    }
}