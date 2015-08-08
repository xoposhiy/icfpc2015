using System.IO;
using System.Linq;
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
            EvaluateSolver(EdgeSolver(), 3);
        }

        private static void EvaluateSolver(Solver solver, int count = int.MaxValue)
        {
            var arena = new Arena(Problems.LoadProblems().Take(count).ToArray());
            var res = arena.RunAllProblems(solver);
            File.WriteAllText("arena.json", JsonConvert.SerializeObject(res, Formatting.Indented));
            Approvals.Verify(res);
        }

        private static Solver EdgeSolver()
        {
            var finder = new MagicDfsFinder();
            var solver = new Solver(finder, new MephalaOracle(finder, Metrics.ShouldNotCreateSimpleHoles));
            return solver;
        }
    }
}