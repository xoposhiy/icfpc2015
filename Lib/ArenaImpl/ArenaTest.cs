using System;
using System.Collections.Generic;
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
            EvaluateSolver(EdgeSolver());
        }
        [Test]
        [MethodImpl(MethodImplOptions.NoInlining)]
        public void FastEvaluate()
        {
            int[] smallMaps = { 0, 1, 10, 13, 15, 16, 17, 19, 20, 21, 22, 23 };
            EvaluateSolver(EdgeSolver(), smallMaps);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static void EvaluateSolver(Solver solver, params int[] maps)
        {
            var ps = Problems.LoadProblems();
            if (maps.Length == 0)
                maps = Enumerable.Range(0, ps.Count).ToArray();
            var arena = new Arena(maps.Select(i => ps[i]).ToArray());
            ArenaModel res = arena.RunAllProblems(solver);
            File.WriteAllText("arena.json", JsonConvert.SerializeObject(res, Formatting.Indented));
            Approvals.Verify(res);
        }

        [Test, Explicit]
        public void SendJson()
        {
            var client = SubmitionClient.Default;
            var model = LoadModel();
            var submissions = model.Problems.SelectMany(
                p => p.MapResults.Select(r => new SubmitionJson
                {
                    problemId = p.Id,
                    seed = r.Seed,
                    solution = r.Result.Commands,
                    tag = model.SolverName + "-" + DateTime.Now
                })).ToArray();
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

        private static Solver EdgeSolver()
        {
            var finder = new MagicDfsFinder();

            var mephala = new MephalaOracle(finder, MephalaMetric.Combined);
            var hircine = new HircineOracle(mephala,MephalaMetric.Combined,3, 5);

            var solver = new Solver(finder, mephala);
            return solver;
        }
    }
}