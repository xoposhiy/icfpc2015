using System;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using ApprovalTests;
using ApprovalUtilities.Utilities;
using Lib.Finder;
using Lib.Intelligence;
using Newtonsoft.Json;
using NUnit.Framework;

namespace Lib.ArenaImpl
{
    [TestFixture]
    public class ArenaTest
    {
        [Test, Explicit]
        [MethodImpl(MethodImplOptions.NoInlining)]
        public void EvaluateEdgeSolver()
        {
            EvaluateSolver(CuttingEdgeSolver(Phrases.DefaultPowerWords));
        }
        [Test]
        [MethodImpl(MethodImplOptions.NoInlining)]
        public void FastEvaluate()
        {
            int[] smallMaps = { 0, 1, 10, 13, 15, 16, 17, 19, 20, 21, 22, 23 };
            EvaluateSolver(CuttingEdgeSolver(Phrases.DefaultPowerWords), smallMaps);
        }

        [Test, Explicit]
        [MethodImpl(MethodImplOptions.NoInlining)]
        public void FastOnBadProblems()
        {
            int[] smallMaps = { 24 };
            EvaluateSolver(CuttingEdgeSolver(Phrases.DefaultPowerWords), smallMaps);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void EvaluateSolver(ISolver solver, params int[] maps)
        {
            var ps = Problems.LoadProblems();
            if (maps.Length == 0)
                maps = Enumerable.Range(0, ps.Count).ToArray();
            var arena = new Arena(maps.Select(i => ps[i]).ToArray());
            ArenaModel res = arena.RunAllProblems(solver);
            File.WriteAllText("arena.json", JsonConvert.SerializeObject(res, Formatting.Indented));
            File.WriteAllText("solutions.json", JsonConvert.SerializeObject(GetSubmissions(res), Formatting.Indented));
            Approvals.Verify(res);
        }

        private static SubmitionJson[] GetSubmissions(ArenaModel model)
        {
            var tag = model.SolverName + "-" + DateTime.Now;
            var submissions = model.Problems.SelectMany(
                p => p.MapResults.Select(r => new SubmitionJson
                {
                    problemId = p.Id,
                    seed = r.Seed,
                    solution = r.Result.Commands,
                    tag = tag
                })).ToArray();
            return submissions;
        }

        [Test, Explicit]
        public void AssertAvgScores()
        {
            var model = LoadModel();
            var actual = model.Problems.Select(x => x.AvgScore).ToArray();
            Console.Out.WriteLine("Mine: {0}", string.Join(", ", actual));
            var theirs = ScoreboardClient.GetMyAvScores(SubmitionClient.ForMining.TeamId);
            Console.Out.WriteLine("Theirs: {0}", string.Join(", ", theirs));
            Assert.That(actual, Is.EqualTo(theirs));
        }

        [Test, Explicit]
        public void SendJson()
        {
            var client = SubmitionClient.Default;
            var model = LoadModel();
            var submissions = GetSubmissions(model);
            Console.WriteLine("submit problems: " + string.Join(", ", submissions.Select(s => s.problemId)));
            client.PostSubmissions(submissions);
        }

        [Test]
        [MethodImpl(MethodImplOptions.NoInlining)]
        [Explicit]
        public void EvaluateJson()
        {
            var model = LoadModel();
            Approvals.Verify(model);
        }

        private static ArenaModel LoadModel()
        {
            var ps = Problems.LoadProblems();
            var model = JsonConvert.DeserializeObject<ArenaModel>(File.ReadAllText("arena.json"));
            model.Problems.Zip(ps, Tuple.Create).ForEach(t => t.Item1.Problem = t.Item2);
            return model;
        }

        public static ISolver CuttingEdgeSolver(string[] powerWords)
        {
            var phrases = new Phrases(powerWords);
            return new AdaptiveSolver(phrases);
        }
    }
}