using NUnit.Framework;
using System.Drawing;
using Lib.Models;
using System;

namespace Lib.Tests
{
    [TestFixture]
    public class MoveTest
    {
        double cw = Math.PI / 3;
        double ccw = 5 * Math.PI / 3;
        Point center = new Point(0, 0);

        [TestCase(1, 0, 0, 1)]
        [TestCase(3, 1, 1, 4)]
        [TestCase(5, 2, 1, 6)]
        [TestCase(2, -1, 2, 2)]
        [TestCase(3, -2, 3, 2)]
        public void TestRotateCW(int x, int y, int expX, int expY)
        {           
            Assert.AreEqual(new Point(expX, expY), new Point(x, y).Rotate(center, cw));           
        }

        [TestCase(0, -1, -1, -1)]
        [TestCase(0, 1, 1, 0)]
        public void TestRotateCCW(int x, int y, int expX, int expY)
        {
            Assert.AreEqual(new Point(expX, expY), new Point(x, y).Rotate(center, ccw));
        }

        [Test]
        public void CheckNewMethodCCW([Range(-10, 10)] int x, [Range(-5, 5)] int y)
        {
            var point = new Point(x, y);
            Assert.AreEqual(point.Rotate(center, ccw), Geometry.RotateMapLocationCCW60AroundZero(point));
        }

        [Test]
        public void CheckNewMethodCW([Range(-10, 10)] int x, [Range(-5, 5)] int y)
        {
            var point = new Point(x, y);
            Assert.AreEqual(point.Rotate(center, cw), Geometry.RotateMapLocationCW60AroundZero(point));
        }
    }
}
