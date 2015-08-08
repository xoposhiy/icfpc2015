using System;
using System.IO;
using System.Linq;
using Lib.Finder;
using Lib.Intelligence;
using Lib.Models;
using NUnit.Framework;

namespace Lib
{
    [TestFixture]
    public class SubmitionClientTest
    {
        private readonly SubmitionClient client = SubmitionClient.Default;

        [Test, Explicit]
        public void SendOne()
        {
            client.PostSubmissions(new SubmitionJson
            {
                problemId = 1,
                seed = 0,
                tag = "SubmissionClientTest.SendOne-" + DateTime.Now,
                solution = "Ei! ".Repeat(30)
            });
        }

        [Test, Explicit]
        public void GetResults()
        {
            var res = client.GetSubmissions();
            Console.WriteLine(res.Length);
            foreach (var submission in res.OrderByDescending(x => x.createdAt))
                Console.WriteLine(submission);
        }

        [Test, Explicit]
        public void SendAllSolutionsFromFile()
        {
	        var payload = File.ReadAllText(@"..\\..\\..\\all-solutions.json");
	        //Console.Out.WriteLine(payload);
	        client.PostSubmissions(payload);
        }

	    [Test, Explicit]
        public void SendAllProblems()
        {
            var submissions =
                from p in Problems.LoadProblems()
                from seed in p.sourceSeeds
                select Solve(p,seed);
            client.PostSubmissions(submissions.ToArray());
        }

        private static SubmitionJson Solve(ProblemJson p, int seed)
        {
            var map = p.ToMap(seed);
            var s1 = new PhrasesOnlySolver().Solve(map);
            var s2 = new Solver(new NullFinder(), new NamiraOracle()).Solve(map);
            var bestRes = new[] { s1, s2 }.OrderByDescending(s => s.Score).First();
            return new SubmitionJson
            {
                problemId = p.id,
                seed = seed,
                solution = bestRes.Commands,
                tag = bestRes.Name + "-" + DateTime.Now
            };
        }
    }
}