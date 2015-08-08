using System;
using System.Linq;
using Lib.Models;
using NUnit.Framework;

namespace Lib
{
    public class Phrases
    {
        public static string[] all =
        {
			"Ei!", // powerBits: 1
			"Ia! Ia!", // powerBits: 2
			"R'lyeh", // powerBits: 4
			"Yuggoth", //powerBits: 8
			"YogSothoth", //powerBits: 32
			"Ph'nglui mglw'nafh Cthulhu R'lyeh wgah'nagl fhtagn!", // powerBits: 516 = 512 + 4
        };

        public static Directions[][] AsDirections = all.Select(p => p.ToDirections().ToArray()).ToArray();
    }

    [TestFixture]
    public class PhrasesTest
    {
        [Test]
        public void Test()
        {
            var ps = Phrases.AsDirections.Select(p => p.ToPhrase());
            foreach (var p in ps)
            {
                Console.WriteLine(p);
            }
        }
    }
}