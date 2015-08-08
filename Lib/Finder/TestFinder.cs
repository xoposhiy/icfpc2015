using System.Collections.Immutable;
using System.Drawing;
using Lib.Models;
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
            field[3, 0] = true;
            var figure = new Unit(new[] {new Point(2, 2), new Point(2, 3)}, new Point(3, 2));
            var zzz = ImmutableStack.Create(figure);
            var y = Map.PositionNewUnit(5, zzz);
            var target = new UnitState {position = new Point(2, 8), angle = 0};
            var x = Finder.GetPath(field, figure, target);
        }
    }
}