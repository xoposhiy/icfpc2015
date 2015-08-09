using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Lib.Models;

namespace Lib.Finder
{
    public class MagicDfsFinder : IFinder
    {
        private readonly IFinder dfsFinder = new HackedDfsFinder();

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

        private const int MaxDepth = 3;

        private IEnumerable<List<Directions[]>> GenerateSpellsSequences(List<Directions[]> list)
        {
            if (list.Count < MaxDepth)
            {
                foreach (var spell in Phrases.all.OrderByDescending(p => p.Length))
                {
                    list.Add(spell.ToDirections().ToArray());
                    foreach (var res in GenerateSpellsSequences(list))
                        yield return res;
                    list.RemoveAt(list.Count - 1);
                }
            }

            if (list.Count > 0)
                yield return list;
        }

        public IEnumerable<Directions> GetPath(Map map, UnitPosition target)
        {
            foreach (var sequence in GenerateSpellsSequences(new List<Directions[]>()))
            {
                var midPositions = new UnitPosition[sequence.Count];
                var finish = target;
                for (int i = 0; i < sequence.Count; i++)
                    midPositions[i] = finish = GetMidPositionByPhrase(finish, sequence[i], map.Unit.Unit.Period);

                var maps = midPositions.Select(p => new Map(map.Id, map.Filled, new PositionedUnit(map.Unit.Unit, p),
                                                            map.NextUnits, map.UsedPositions, map.Scores)).ToArray();
                bool ok = true;
                for (int i = 0; i < sequence.Count && ok; i++)
                    ok &= maps[i].IsGoodPath(sequence[i]);
                if (!ok)
                    continue;

                var path = dfsFinder.GetPath(map, finish);
                if (path == null)
                    continue;
                var result = path.Concat(((IEnumerable<Directions[]>)sequence).Reverse().SelectMany(s => s)).ToArray();
                if (map.IsGoodPath(result))
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