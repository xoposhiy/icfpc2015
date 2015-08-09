using System;
using System.Collections.Generic;
using System.Linq;
using Lib.Models;

namespace Lib.Finder
{
    public class BfsNoMagicFinder : IFinder
    {
        private Map Map;
        private Dictionary<UnitPosition, Tuple<Map, Directions>> Parents;
        private readonly Directions[] dirs = (Directions[])Enum.GetValues(typeof(Directions));

        public Tuple<int, IEnumerable<Directions>> GetSpellLengthAndPath(Map map, UnitPosition target)
        {
            UpdateMap(map);
            var path = !Parents.ContainsKey(target) ? null
                           : RestoreDirections(target).Reverse();
            return Tuple.Create(0, path);
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
            Bfs(map);
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

        private void Bfs(Map startMap)
        {
            var queue = new Queue<Map>();
            queue.Enqueue(startMap);
            while (queue.Count > 0)
            {
                var map = queue.Dequeue();
                foreach (var d in dirs)
                    BfsStep(map, d, queue);
            }
        }

        private void BfsStep(Map map, Directions d, Queue<Map> queue)
        {
            if (!map.IsSafeMovement(d)) return;
            var newMap = map.Move(d);
            var pos = newMap.Unit.Position;
            if (Parents.ContainsKey(pos)) return;
            Parents.Add(pos, Tuple.Create(map, d));
            queue.Enqueue(newMap);
        }
    }
}