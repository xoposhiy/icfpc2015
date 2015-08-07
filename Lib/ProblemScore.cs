using System;
using ApprovalTests;
using ApprovalTests.Reporters;
using Newtonsoft.Json;
using NUnit.Framework;

namespace Lib
{
	public class ProblemScoreJson
	{
		public int powerScore { get; set; }
		public int seed { get; set; }
		public string tag { get; set; }
		public DateTime createdAt { get; set; }
		public int score { get; set; }
		public int authorId { get; set; }
		public int teamId { get; set; }
		public int problemId { get; set; }
		public string solution { get; set; }
	}

	[TestFixture]
	public class ProblemScoreTest
	{
		[Test, UseReporter(typeof(DiffReporter))]
		public void Test()
		{
			var settings = new JsonSerializerSettings
			{
				Formatting = Formatting.Indented
			};
			
			var s = JsonConvert.SerializeObject(new ProblemScoreJson(), settings);
			Approvals.Verify(s);
		}
	}
}