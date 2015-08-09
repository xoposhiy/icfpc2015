using Lib;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WordsFarmer
{
    [TestFixture]
    class Program
    {

        const string tag = "WordFarming";
        static SubmitionClient client = SubmitionClient.ForMining;


        static List<WordPost> posts;
        static List<WordPost> wrongs;

        static void CheckWord(string word)
        {
            var post = WordChecker.PrepareWord(word, posts.Count);
            if (post.Status == WordStatus.OK)
                posts.Add(post);
            else
                wrongs.Add(post);
        }

        [Test]
        static void PostWords()
        {
            var words = File.ReadAllLines("..\\..\\words.txt").ToList();

            posts = new List<WordPost>();
            wrongs = new List<WordPost>();

            while(posts.Count<49)
            {
                if (words.Count == 0) break;
                var word = words[0];
                words.RemoveAt(0);
                CheckWord(word + "!");
            }

            File.AppendAllLines("..\\..\\wrongWords.txt",
                wrongs.Select(z => string.Format("{0,-8}{1,-20}{2}", z.Status, z.Word,z.OriginalWord))
                );

            File.AppendAllLines ("..\\..\\submittedWords.txt",
                posts.Select(z => string.Format("{0,-8}{1,-20}{2}", z.Status, z.Word,z.OriginalWord))
                );

            File.WriteAllLines("..\\..\\words.txt", words);
            
           

            var subs = posts
                .Select(z => new SubmitionJson
                {
                    problemId = z.ProblemId,
                    seed = z.Seed,
                    tag = tag + DateTime.Now.ToString(),
                    solution = z.Word
                }).ToArray();

            client.PostSubmissions(subs);

            Console.WriteLine($"Submitted {posts.Count}, Rejected {wrongs.Count}, Left {words.Count}");
        }

        public static void ReadSubmissions()
        {
          
            var res = client.GetSubmissions();
            foreach (var submission in res.Where(z => z.tag.StartsWith(tag) && z.powerScore!=0))
                Console.WriteLine("{0,-6}{1,-6}{2}", submission.powerScore, submission.score,submission.solution);
        }


        public static void Main()
        {
           //PostWords(); return;
           ReadSubmissions();
        }
    }
}
