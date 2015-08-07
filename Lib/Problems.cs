using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using NUnit.Framework;

namespace Lib
{
	public class Problems
	{
		public static List<ProblemJson> LoadProblems()
		{
			return 
				Directory.GetFiles(@"problems", "problem*.json")
				         .Select(File.ReadAllText)
				         .Select(JsonConvert.DeserializeObject<ProblemJson>)
				         .ToList();
		}
	}

	[TestFixture]
	public class ProblemsTest
	{
		[Test]
		public void Test()
		{
			Problems.LoadProblems();
		}
	}
}