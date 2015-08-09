﻿using System;
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

        public IEnumerable<Directions> GetPath(Map map, UnitPosition target)
        {
            UpdateMap(map);
            return !Parents.ContainsKey(target) ? null
                       : RestoreDirections(target).Reverse();
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
            int phraseIndex = 0;
            int phrasePrefix = 0;
            for (int i = 0; i < 3; i++)
            {
                var directions = Phrases.AsDirections[i];
                for (int j = 1; j <= Math.Min(directions.Length - 1, history.Count); j++)
                {
                    bool eq = true;
                    for (int k = 0; k < j && eq; k++)
                        if (directions[k] != history[history.Count - j + k])
                            eq = false;
                    if (eq && j >= phrasePrefix && map.IsGoodPath(directions.Skip(j)))
                    {
                        phraseIndex = i;
                        phrasePrefix = j;
                    }
                }
            }
            int charIndex = phrasePrefix;

            var phrase = Phrases.AsDirections[phraseIndex];
            if (charIndex >= phrase.Length)
                throw new Exception();
            var dir = phrase[charIndex];
            DfsStep(map, dir, history);
            foreach (var d in dirs)
                DfsStep(map, d, history);
        }

        private static Random r = new Random();

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