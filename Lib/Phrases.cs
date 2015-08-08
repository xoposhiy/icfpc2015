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
            "Ei!", // powerScore: 1
            "Ia! Ia!", // powerScore: 2
            "R'lyeh", // powerScore: 4
            "Yuggoth", //powerScore: 8
            "Ph'nglui mglw'nafh Cthulhu R'lyeh wgah'nagl fhtagn!", // powerScore: 516 = 512 + 4
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