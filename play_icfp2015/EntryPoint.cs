using System;
using System.Collections.Generic;
using System.Linq;
using Lib;
using Lib.Intelligence;
using Lib.Models;
using Newtonsoft.Json;

namespace play_icfp2015
{
	internal class EntryPoint
	{
		private static void Main(string[] args)
		{
			var submissions =
				from problem in LoadProblems(args)
				from seed in problem.sourceSeeds
				select new SubmitionJson
				{
					problemId = problem.id,
					seed = seed,
					tag = "NamiraOracle-" + DateTime.Now,
					solution = Solve(problem.ToMap(seed))
				};
			var submitionJsons = submissions.ToArray();
			Console.Out.WriteLine(JsonConvert.SerializeObject(submitionJsons, Formatting.Indented));
		}

		private static IEnumerable<ProblemJson> LoadProblems(string[] args)
		{
			var filenames = new List<string>();
			for (var i = 0; i < args.Length; i++)
			{
				if (args[i].ToLower() == "-f")
				{
					filenames.Add(args[i + 1]);
					i++;
				}
			}
			return Problems.LoadProblems(filenames);
		}

		private static string Solve(Map map)
		{
			var s2 = new NamiraOracle().Solve(map);
			return s2.Commands;
		}
	}
}