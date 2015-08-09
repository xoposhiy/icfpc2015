using System;
using System.Diagnostics;
using System.Linq;
using Lib.Models;
using NUnit.Framework;

namespace Lib
{
    public static class Phrases
    {
        public static string[] all =
        {
			"Ei!", // powerBits: 1
			"Ia! Ia!", // powerBits: 2
			"R'lyeh", // powerBits: 4
			"Yuggoth", //powerBits: 8
			"YogSothoth", //powerBits: 32
            "Necronomicon", // powerBits: 64
			"Ph'nglui mglw'nafh Cthulhu R'lyeh wgah'nagl fhtagn!", // powerBits: 516 = 512 + 4
        };

        public static string ToOriginalPhrase(this string text)
        {
            int score;
            return text.ToOriginalPhrase(out score);
        }
        public static string ToOriginalPhrase(this string text, out int score)
        {
            int[] pScore = new int[all.Length];
            var pairs = AsCanonical.Zip(all, (s1, s2) => new { canonical = s1, original = s2 }).OrderByDescending(pair => pair.canonical.Length).ToList();
            while (true)
            {
                var found = false;
                for (int index = 0; index < pairs.Count; index++)
                {
                    var pair = pairs[index];
                    var startIndex = text.IndexOf(pair.canonical, StringComparison.InvariantCulture);
                    if (startIndex >= 0)
                    {
                        found = true;
                        if (pScore[index] == 0) pScore[index] = 300;
                        pScore[index] += 2 * pair.original.Length;
                        var s2 = text.Substring(0, startIndex) + pair.original + text.Substring(startIndex + pair.canonical.Length);
                        Debug.Assert(s2.Length == text.Length);
                        text = s2;
                    }
                }
                if (!found) break;
            }
            score = pScore.Sum();
            return text;
        }

        public static Directions[][] AsDirections = all.Select(p => p.ToDirections().ToArray()).ToArray();
        public static string[] AsCanonical= AsDirections.Select(ds => ds.ToPhrase()).ToArray();
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

        [Test]
        public void ToOriginal()
        {
            var original = string.Join("", Phrases.all);
            var canonical = original.ToDirections().ToPhrase();
            Console.WriteLine(canonical);
            var converted = canonical.ToOriginalPhrase();
            Console.WriteLine(converted);
            Assert.AreEqual(original, converted);
        }
    }
}