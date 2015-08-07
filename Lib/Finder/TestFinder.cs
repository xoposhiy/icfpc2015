using System.Drawing;
using NUnit.Framework;

namespace Lib.Finder
{
    [TestFixture]
    public class FinderTest
    {
        [Test]
        public void Test()
        {
            var field = new bool[5, 10];
            var figure = new FinderUnit(new Point[] { new Point(1, 0), new Point(2, 0) }, new Point(1, 0));
            var target = new Point(1, 9);
            var x = Lib.Finder.Finder.GetPath(field, figure, target);
        }
    }
}