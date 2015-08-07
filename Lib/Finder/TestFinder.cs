using NUnit.Framework;

namespace Lib
{
    [TestFixture]
    public class FinderTest
    {
        [Test]
        public void Test()
        {
            var field = new bool[5, 10];
            var figure = new FinderUnit(new PointInt[] { new PointInt(1, 0), new PointInt(2, 0) }, new PointInt(1, 0));
            var target = new PointInt(1, 9);
            var x = Finder.GetPath(field, figure, target);
        }
    }
}