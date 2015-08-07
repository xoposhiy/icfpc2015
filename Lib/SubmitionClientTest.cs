using System;
using System.Linq;
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
				problemId = 0,
				seed = 0,
				tag = "SubmissionClientTest.SendOne-" + DateTime.Now,
				solution = "Ph'nglui mglw'nafh Cthulhu R'lyeh wgah'nagl fhtagn!Ei!Ei!"
			});
		}

		[Test, Explicit]
		public void GetResults()
		{
			var res = client.GetSubmitions();
			Console.WriteLine(res.Length);
			foreach (var submission in res)
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
					solution = "Ei!Ph'nglui mglw'nafh Cthulhu R'lyeh wgah'nagl fhtagn!Ei!Ei!Ph'nglui mglw'nafh Cthulhu R'lyeh wgah'nagl fhtagn!"
				};
			client.PostSubmitions(submissions.ToArray());
		}
	}
}