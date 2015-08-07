using NUnit.Framework;
using System.Drawing;
using Lib.Model;
using System;

namespace Lib.Tests
{
    [TestFixture]
    public class MoveTest
    {
        [SetUp]
        public void SetUp()
        {
            
        }

        [Test]
        public void TestRotate()
        {
            var center = new Point(6, 4);

            Assert.AreEqual(new Point(5, 3), new Point(6, 3));
        }
    }
}
