using System;
using System.Collections.Generic;
using System.Linq;
using Lib.Models;
using NUnit.Framework;

namespace Lib
{
    public class Phrases
    {
        public static string[] DefaultPowerWords =
        {
            "Ei!", // powerBits: 1
            "R'lyeh", // powerBits: 4
            "Ia! Ia!", // powerBits: 2
            "Yuggoth", //powerBits: 8
            "Yoyodyne", //powerBits: 8192
            "Planet 10", //powerBits: 4096
            "YogSothoth", //powerBits: 32
            "blue hades", //powerBits: 65536
            "Tsathoggua", //powerBits: 16
            "Necronomicon", //powerBits: 64
            "cthulhu fhtagn!", //powerBits: 256
            "Ph'nglui mglw'nafh Cthulhu R'lyeh wgah'nagl fhtagn.", // powerBits: 516 = 512 + 4

			"vigintillion",
			"In his house at R'lyeh dead Cthulhu waits dreaming.",
			"The Laundry",
			"monkeyboy",
			"John Bigboote",
			"CASE NIGHTMARE GREEN",
        };

        public Phrases(string[] powerWords)
        {
            Words = powerWords.Select(x => new Phrase(x)).ToArray();
            All = Words.Select(w => w.Original).ToArray();
            AsDirections = All.Select(p => p.ToDirections().ToArray()).ToArray();
            AsCanonical = AsDirections.Select(ds => ds.ToPhrase()).ToArray();
        }

        public readonly Phrase[] Words;
        public readonly string[] All;
        public readonly Directions[][] AsDirections;
        public readonly string[] AsCanonical;

        public string ToOriginalPhrase(string text)
        {
            var pairs = AsCanonical.Zip(All, (s1, s2) => new {canonical = s1, original = s2}).OrderByDescending(pair => pair.canonical.Length).ToList();
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

        public int GetPowerScore(string textInOriginalTongue)
        {
            int score;
            var words = GetPowerWords(textInOriginalTongue, out score);
            return score + words.Count * 300;
        }

        public int GetPowerScoreWithoutUniqueBonus(string textInOriginalTongue)
        {
            int score;
            GetPowerWords(textInOriginalTongue, out score);
            return score;
        }

        public HashSet<string> GetPowerWords(string textInOriginalTongue, out int scoreWithoutUniqueBonus)
        {
            var distinctWords = new HashSet<string>();
            scoreWithoutUniqueBonus = 0;
            for (var index = 0; index < All.Length; index++)
            {
                var phrase = All[index];
                var startIndex = 0;
                int foundIndex;
                while ((foundIndex = textInOriginalTongue.IndexOf(phrase, startIndex, StringComparison.InvariantCulture)) >= 0)
                {
                    scoreWithoutUniqueBonus += 2 * phrase.Length;
                    startIndex = foundIndex + 1;
                    distinctWords.Add(phrase);
                }
            }
            return distinctWords;
        } 
    }

    [TestFixture]
    public class PhrasesTest
    {
        [Test]
        public void Test()
        {
            var ps = new Phrases(Phrases.DefaultPowerWords).AsDirections.Select(p => p.ToPhrase());
            foreach (var p in ps)
                Console.WriteLine(p);
        }

        [Test]
        public void ToOriginal()
        {
            var phrases = new Phrases(Phrases.DefaultPowerWords);
            var original = string.Join("", phrases.All);
            var canonical = original.ToDirections().ToPhrase();
            Console.WriteLine(canonical);
            var converted = phrases.ToOriginalPhrase(canonical);
            Console.WriteLine(converted);
            Assert.AreEqual(original, converted);
        }

        [Test]
        public void GetPowerScore()
        {
            var phrases = new Phrases(Phrases.DefaultPowerWords);
            Assert.That(phrases.GetPowerScore("Ei!"), Is.EqualTo(306));
            Assert.That(phrases.GetPowerScore("Ph'nglui mglw'nafh Cthulhu R'lyeh wgah'nagl fhtagn."), Is.EqualTo(714));
        }

        [Test]
        public void Score_NestedPhrase()
        {
            var phrases = new Phrases(Phrases.DefaultPowerWords);
            var testPhrase = "Ph'nglui mglw'nafh Cthulhu R'lyeh wgah'nagl fhtagn.".ToDirections().ToPhrase();
            var originalPhrase = phrases.ToOriginalPhrase(testPhrase);
            Assert.That(phrases.GetPowerScore(originalPhrase), Is.EqualTo(714));
        }
    }
}