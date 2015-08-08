using NUnit.Framework;

namespace Lib
{
    public class LinearGenerator
    {
        public LinearGenerator(int seed)
        {
            next = (ulong)seed;
        }

        private static ulong next = 1;

        public int Next()
        {
            var prev = next;
            next = next * 1103515245 + 12345;
            return (int)((prev >> 16) % 32768);
        }
    }

    [TestFixture]
    public class LinearGeneratorTest
    {
        [Test]
        public void Test()
        {
            var gen = new LinearGenerator(17);
            foreach (var expected in new[] {0, 24107, 16552, 12125, 9427, 13152, 21440, 3383, 6873, 16117})
                Assert.AreEqual(expected, gen.Next());
        }
    }
}