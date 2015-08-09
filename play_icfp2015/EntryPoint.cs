using System;
using System.Collections.Generic;
using System.Linq;
using Lib;
using Lib.ArenaImpl;
using Newtonsoft.Json;

namespace play_icfp2015
{
    internal class EntryPoint
    {
        private static void Main(string[] args)
        {
            var powerWords = ParseMultiArgs(args, "-p").ToArray();
            var solver = ArenaTest.CuttingEdgeSolver(powerWords);
            var tag = "final-submission-" + DateTime.Now;
            var submissions =
                from problem in LoadProblems(args)
                from seed in problem.sourceSeeds
                select new SubmitionJson
                {
                    problemId = problem.id,
                    seed = seed,
                    tag = tag,
                    solution = solver.Solve(problem.ToMap(seed)).Commands
                };
            var submitionJsons = submissions.ToArray();
            Console.Out.WriteLine(JsonConvert.SerializeObject(submitionJsons, Formatting.Indented));
        }

        private static IEnumerable<ProblemJson> LoadProblems(string[] args)
        {
            return Problems.LoadProblems(ParseMultiArgs(args, "-f"));
        }

        private static List<string> ParseMultiArgs(string[] args, string argName)
        {
            var argValues = new List<string>();
            for (var i = 0; i < args.Length; i++)
            {
                if (args[i].ToLower() == argName)
                {
                    argValues.Add(args[i + 1]);
                    i++;
                }
            }
            return argValues;
        }
    }
}