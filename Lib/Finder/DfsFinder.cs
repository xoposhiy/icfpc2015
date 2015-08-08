using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Lib.Models;
using NUnit.Framework;

namespace Lib.Finder
{
    public class DfsFinder : IFinder
    {
        public Directions[] GetPath(Map map, UnitPosition target)
        {
            var parents = new Dictionary<UnitPosition, Tuple<UnitPosition, Directions>>
            {
                {map.Unit.Position, null}
            };
            Dfs(map, parents);
            if (!parents.ContainsKey(target)) return null;
            return RestoreDirections(parents, target).Reverse().ToArray();
        }

        private IEnumerable<Directions> RestoreDirections(Dictionary<UnitPosition, Tuple<UnitPosition, Directions>> parents, UnitPosition target)
        {
            while (true)
            {
                var tuple = parents[target];
                if (tuple == null) yield break;
                yield return tuple.Item2;
                target = tuple.Item1;
            }
        }

        private void Dfs(Map map, Dictionary<UnitPosition, Tuple<UnitPosition, Directions>> parents)
        {
            var dirs = (Directions[])Enum.GetValues(typeof(Directions));
            foreach (var d in dirs)
            {
                if (!map.IsSafeMovement(d)) continue;
                var newMap = map.Move(d);
                var pos = newMap.Unit.Position;
                if (parents.ContainsKey(pos)) continue;
                parents.Add(pos, Tuple.Create(map.Unit.Position, d));
                Dfs(newMap, parents);
            }
        }
    }

    [TestFixture]
    public class DfsFinderTest
    {
        [Test]
        public void Test([Range(0, 9)]int targetX, [Range(0, 9)]int targetY)
        {
            var map = Problems.LoadProblems()[0].ToMap(0);
            var path = new DfsFinder().GetPath(map, new UnitPosition(new Point(targetX, targetY), 0));
            Assert.IsNotNull(path);
            var map2 = map.Move(path);
            Assert.IsFalse(map2.IsOver);
            Assert.AreEqual(new Point(targetX, targetY), map2.Unit.Position.Point);
            Assert.AreEqual(map.NextUnits.Count(), map2.NextUnits.Count());
        }
        [Test]
        public void Unpassable()
        {
            var map = Problems.LoadProblems()[1].ToMap(0);
            var path = new DfsFinder().GetPath(map, new UnitPosition(new Point(2, 4), 0));
            Assert.IsNull(path);
        }
        [Test]
        public void BadRotation()
        {
            var map = Problems.LoadProblems()[0].ToMap(0);
            var path = new DfsFinder().GetPath(map, new UnitPosition(new Point(2, 4), 1));
            Assert.IsNull(path);
        }
    }
}