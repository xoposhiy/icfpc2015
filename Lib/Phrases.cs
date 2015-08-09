using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Lib.Models;
using NUnit.Framework;

namespace Lib
{
    public static class Phrases
    {
        public static Phrase[] Words =
        {
            "Ei!", // powerBits: 1
            "R'lyeh", // powerBits: 4
            "Ia! Ia!", // powerBits: 2
            "Yuggoth", //powerBits: 8
            "YogSothoth", //powerBits: 32
            "blue hades", //powerBits: 65536
            "Tsathoggua", //powerBits: 16
            "Necronomicon", //powerBits: 64
            "cthulhu fhtagn!", //powerBits: 256
            "Ph'nglui mglw'nafh Cthulhu R'lyeh wgah'nagl fhtagn." // powerBits: 516 = 512 + 4
        };

        public static string[] all = Words.Select(w => w.Original).ToArray();
        public static Directions[][] AsDirections = all.Select(p => p.ToDirections().ToArray()).ToArray();
        public static string[] AsCanonical = AsDirections.Select(ds => ds.ToPhrase()).ToArray();

        public static string ToOriginalPhrase(this string text)
        {
            var pairs = AsCanonical.Zip(all, (s1, s2) => new {canonical = s1, original = s2}).OrderByDescending(pair => pair.canonical.Length).ToList();
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
                        text = text.Substring(0, startIndex) + pair.original + text.Substring(startIndex + pair.canonical.Length);
                    }
                }
                if (!found) break;
            }
            return text;
        }

        public static int GetPowerScore(this string textInOriginalTongue)
        {
            int score;
            var words = GetPowerWords(textInOriginalTongue, out score);
            return score + words.Distinct().Count() * 300;
        }

        public static IEnumerable<string> GetPowerWords(this string textInOriginalTongue, out int scoreWithoutUniqueBonus)
        {
            var words = new List<string>();
            scoreWithoutUniqueBonus = 0;
            for (var index = 0; index < all.Length; index++)
            {
                var phrase = all[index];
                var startIndex = 0;
                int foundIndex;
                while ((foundIndex = textInOriginalTongue.IndexOf(phrase, startIndex, StringComparison.InvariantCulture)) >= 0)
                {
                    scoreWithoutUniqueBonus += 2 * phrase.Length;
                    startIndex = foundIndex + 1;
                    words.Add(phrase);
                }
            }
            return words;
        } 
    }

    [TestFixture]
    public class PhrasesTest
    {
        [Test]
        public void Test()
        {
            var ps = Phrases.AsDirections.Select(p => p.ToPhrase());
            foreach (var p in ps)
                Console.WriteLine(p);
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

        [Test]
        public void GetPowerScore()
        {
            Assert.That("Ei!".GetPowerScore(), Is.EqualTo(306));
            Assert.That("Ph'nglui mglw'nafh Cthulhu R'lyeh wgah'nagl fhtagn!".GetPowerScore(), Is.EqualTo(714));
        }

        [Test]
        public void Score_NestedPhrase()
        {
            Assert.That("Ph'nglui mglw'nafh Cthulhu R'lyeh wgah'nagl fhtagn!".ToDirections().ToPhrase().ToOriginalPhrase().GetPowerScore(), Is.EqualTo(714));
        }
    }
}