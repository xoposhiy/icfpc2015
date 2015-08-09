using System;
using System.Collections.Generic;
using System.Linq;
using Lib.Models;

namespace Lib.Finder
{
    public class HackedDfsFinder : IFinder
    {
        private Map Map;
        private Dictionary<UnitPosition, Tuple<Map, Directions>> Parents;
        private readonly Directions[] dirs = (Directions[])Enum.GetValues(typeof(Directions));

        public Tuple<int, IEnumerable<Directions>> GetSpellLengthAndPath(Map map, UnitPosition target)
        {
            UpdateMap(map);
            var path = !Parents.ContainsKey(target) ? null
                           : RestoreDirections(target).Reverse();
            return Tuple.Create(0, path); //TODO not zero!
        }

        public IEnumerable<Map> GetReachablePositions(Map map)
        {
            UpdateMap(map);
            return Parents.Values.Select(t => t == null ? map : t.Item1.Move(t.Item2));
        }

        private void UpdateMap(Map map)
        {
            if (ReferenceEquals(Map, map)) return;
            Map = map;
            Parents = new Dictionary<UnitPosition, Tuple<Map, Directions>>
            {
                {map.Unit.Position, null}
            };
            Dfs(map, new List<Directions>());
        }

        private IEnumerable<Directions> RestoreDirections(UnitPosition target)
        {
            while (true)
            {
                var tuple = Parents[target];
                if (tuple == null) yield break;
                yield return tuple.Item2;
                target = tuple.Item1.Unit.Position;
            }
        }

        private void Dfs(Map map, List<Directions> history)
        {
            var maxPrefixForDir = new Tuple<int, int>[dirs.Length];
            for (int i = 0; i < dirs.Length; i++)
                maxPrefixForDir[i] = Tuple.Create(-1, i);

            for (int i = 0; i < 1; i++)
            {
                var directions = Phrases.AsDirections[i];
                for (int j = Math.Min(directions.Length - 1, history.Count); j >= 0; j--)
                {
                    bool eq = true;
                    for (int k = 0; k < j && eq; k++)
                        if (directions[k] != history[history.Count - j + k])
                            eq = false;
                    if (eq && map.IsGoodPath(directions.Skip(j)))
                    {
                        int index = (int)directions[j];
                        var pair = maxPrefixForDir[index];
                        if (j >= pair.Item1)
                            maxPrefixForDir[index] = Tuple.Create(j, pair.Item2);
                        break;
                    }
                }
            }

            var dirsOrder = Enumerable.Range(0, dirs.Length).OrderByDescending(i => maxPrefixForDir[i]);
            foreach (var d in dirsOrder)
                DfsStep(map, (Directions)d, history);
        }

        private static readonly Random r = new Random();

        private void DfsStep(Map map, Directions d, List<Directions> history)
        {
            if (!map.IsSafeMovement(d)) return;
            var newMap = map.Move(d);
            var pos = newMap.Unit.Position;
            if (Parents.ContainsKey(pos)) return;
            Parents.Add(pos, Tuple.Create(map, d));
            history.Add(d);
            Dfs(newMap, history);
            history.RemoveAt(history.Count - 1);
        }
    }
}