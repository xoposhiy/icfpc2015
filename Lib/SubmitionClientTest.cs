using System;
using System.Linq;
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
			client.PostSubmitions(new SubmitionJson
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
			var res = client.GetSubmitions();
			Console.WriteLine(res.Length);
			foreach (var submission in res.OrderByDescending(x => x.createdAt))
				Console.WriteLine(submission);
		}

		[Test, Explicit]
		public void SendAllProblems()
		{
			var submissions =
				from p in Problems.LoadProblems()
				from seed in p.sourceSeeds
				select new SubmitionJson
				{
					problemId = p.id,
					seed = seed,
					tag = "SubmissionClientTest.SendAllProblems-" + DateTime.Now,
					solution = Solve(p.ToMap(seed))
                    };
			client.PostSubmitions(submissions.ToArray());
		}

	    private static string Solve(Map map)
	    {
	        var s1 = new PhrasesOnlySolver().Solve(map);
	        var s2 = new NamiraOracle().Solve(map);
	        if (s1.Score > s2.Score) return s1.Commands;
	        return s2.Commands;
	    }
	}
}