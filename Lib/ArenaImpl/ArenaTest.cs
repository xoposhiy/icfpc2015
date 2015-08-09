using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using ApprovalTests;
using Lib.Finder;
using Lib.Intelligence;
using Newtonsoft.Json;
using NUnit.Framework;

namespace Lib.ArenaImpl
{
    [TestFixture]
    public class ArenaTest
    {
        [Test]
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
            var res = arena.RunAllProblems(solver);
            File.WriteAllText("arena.json", JsonConvert.SerializeObject(res, Formatting.Indented));
            Approvals.Verify(res);
        }

        private static Solver EdgeSolver()
        {
            var finder = new MagicDfsFinder();
            var solver = new Solver(finder, new MephalaOracle(finder, MephalaMetric.HolesOnly));
            return solver;
        }
    }
}