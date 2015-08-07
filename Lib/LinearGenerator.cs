using NUnit.Framework;

namespace Lib
{
    public class LinearGenerator
    {
        private static ulong next = 1;

        public uint Next()
        {
            var prev = next;
            next = next * 1103515245 + 12345;
            return (uint)(prev >> 16) % 32768;
        }

        public void SetSeed(int seed)
        {
            next = (uint)seed;
        }
    }

    [TestFixture]
    public class LinearGeneratorTest
    {
        [Test]
        public void Test()
        {
            var gen = new LinearGenerator();
            gen.SetSeed(17);
            foreach (var expected in new[] {0, 24107, 16552, 12125, 9427, 13152, 21440, 3383, 6873, 16117})
                Assert.AreEqual(expected, gen.Next());
        }
    }
}