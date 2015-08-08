using System.Drawing;
using System.Linq;
using Lib.Models;
using NUnit.Framework;

namespace Lib.Finder
{
    [TestFixture]
    public class DfsFinderTest
    {
        [Test]
        public void Test([Range(0, 9)] int targetX, [Range(0, 9)] int targetY)
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