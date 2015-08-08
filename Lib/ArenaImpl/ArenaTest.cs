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
        public void Test()
        {
            var arena = new Arena(Problems.LoadProblems().Take(5).ToArray());
            var finder = new DfsFinder();
            var solver = new Solver(finder, new MephalaOracle(finder, Metrics.ShouldNotCreateSimpleHoles));
            var res = arena.RunAllProblems(solver);
            File.WriteAllText("arena.json", JsonConvert.SerializeObject(res, Formatting.Indented));
            Approvals.Verify(res);
        }
    }
}