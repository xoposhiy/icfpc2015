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
        private readonly SubmitionClient miningClient = SubmitionClient.ForMining;

        [Test, Explicit]
        public void SendOne()
        {
            miningClient.PostSubmissions(new SubmitionJson
            {
                problemId = 1,
                seed = 0,
                tag = "SubmissionClientTest.SendOne-" + DateTime.Now,
                solution = "Ei!"
            });
        }

		[Test, Explicit]
		public void SendSinglePhrase()
		{
		    var tag = "SendSinglePhrase-" + DateTime.Now;
            var submissions =
				from p in Problems.LoadProblems()
				select new SubmitionJson
				{
					problemId = p.id,
					seed = 0,
					solution = "icfpc2015!",
					tag = tag
				};
            miningClient.PostSubmissions(submissions.ToArray());
		}

		[Test, Explicit]
        public void GetResults()
        {
            var res = client.GetSubmissions();
            Console.WriteLine(res.Length);
            foreach (var submission in res.OrderByDescending(x => x.createdAt).Take(200))
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
	        foreach (var submitionJson in submissions.Select(Log))
	        {
	            client.PostSubmissions(submitionJson);
	        }
        }

        private SubmitionJson Log(SubmitionJson submission)
        {
            Console.WriteLine(submission.problemId + " " + submission.seed);
            return submission;
        }

        private static SubmitionJson Solve(ProblemJson p, int seed)
        {
            var phrases = new Phrases(Phrases.DefaultPowerWords);

            var map = p.ToMap(seed);
            var finder = new MagicDfsFinder(phrases);
            var bestRes = new Solver(phrases, finder, new MephalaOracle(finder, WeightedMetric.Keening)).Solve(map);
//            var s2 = new Solver(finder, new AzuraOracle()).Solve(map);
//            var bestRes = new[] { s1, s2 }.OrderByDescending(s => s.Score).First();
            return new SubmitionJson
            {
                problemId = p.id,
                seed = seed,
                solution = phrases.ToOriginalPhrase(bestRes.Commands),
                tag = bestRes.Name + "-" + DateTime.Now
            };
        }
    }
}