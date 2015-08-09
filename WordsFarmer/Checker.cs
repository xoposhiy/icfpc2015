using Lib;
using Lib.Models;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WordsFarmer
{
    class WordPost
    {
        public string Word;
        public int Seed;
        public WordStatus Status;
        public int ProblemId;
    }

    enum WordStatus
    {
        NotWord,
        Period,
        OK
    }

    [TestFixture]
    class WordChecker
    {
         static ProblemJson problem = Problems.LoadProblems()[4];

        public static WordPost PrepareWord(string s, int number)
        {
            s = s.Replace("-", " ");
            s = "alal" + s;
            var result = new WordPost { Word = s, Seed = problem.sourceSeeds[number], ProblemId=problem.id };
            
            result.Status = CheckWordAcceptance(result);
            return result;
        }


        public static WordStatus CheckWordAcceptance(WordPost post)
        {
            foreach (var c in post.Word)
            {
                try
                {
                    var dir=c.ToDirection();
                }
                catch { return WordStatus.NotWord; }
            }

            var map = problem.ToMap(post.Seed);
            if (map.Unit.Unit.Period != 6) throw new Exception();
            foreach (var c in post.Word)
                map=map.Move(c);
            if (map.IsOver) return WordStatus.Period;
            return WordStatus.OK;
        }

        [TestCase("q", WordStatus.OK)]
        [TestCase("ia ia!", WordStatus.OK)]
        [TestCase("ei!", WordStatus.OK)]
        [TestCase("alalalala!", WordStatus.OK)]
        [TestCase("qqqqqq", WordStatus.Period)]
        [TestCase("ia-ia", WordStatus.NotWord)]
        public void TestWordAcceptanceChecking(string word, WordStatus result)
        {
            Assert.AreEqual(result, CheckWordAcceptance(new WordPost { Word = word, Seed = 0 }));
        }

        
    }
}
