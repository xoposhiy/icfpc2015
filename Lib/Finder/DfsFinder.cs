using System;
using System.Collections.Generic;
using System.Linq;
using Lib.Models;

namespace Lib.Finder
{
    public class DfsFinder : IFinder
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
            Dfs(map, 0, 0);
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

        private void Dfs(Map map, int phraseIndex, int charIndex)
        {

            var phrase = Phrases.AsDirections[phraseIndex];
            if (charIndex >= phrase.Length)
            {
                charIndex = 0;
                phraseIndex = SelectNextPhrase(phraseIndex, map);
                phrase = Phrases.AsDirections[phraseIndex];
            }
            var dir = phrase[charIndex];
            DfsStep(map, dir, phraseIndex, charIndex);
            foreach (var d in dirs)
            {
                DfsStep(map, d, phraseIndex, charIndex);
            }
        }

        private static int SelectNextPhrase(int phraseIndex, Map map)
        {
            int lastPhraseIndex = phraseIndex;
            while (true)
            {
                phraseIndex = (phraseIndex + 1) % Phrases.all.Length;
                if (phraseIndex == lastPhraseIndex) return 0;
                var unitAfterPhrase = Phrases.Words[phraseIndex].Move(map.Unit);
                if (map.IsValidPosition(unitAfterPhrase))
                    return phraseIndex;
            }
        }

        private static Random r = new Random();

        private void DfsStep(Map map, Directions d, int phraseIndex, int charIndex)
        {
            if (!map.IsSafeMovement(d)) return;
            var newMap = map.Move(d);
            var pos = newMap.Unit.Position;
            if (Parents.ContainsKey(pos)) return;
            Parents.Add(pos, Tuple.Create(map, d));
            var phrase = Phrases.AsDirections[phraseIndex];
            if (charIndex < phrase.Length && phrase[charIndex] == d)
                Dfs(newMap, phraseIndex, charIndex + 1);
            else
                Dfs(newMap, phraseIndex, int.MaxValue);
        }
    }
}