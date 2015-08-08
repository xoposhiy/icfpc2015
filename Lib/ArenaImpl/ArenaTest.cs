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
            var arena = new Arena(Problems.LoadProblems().Take(4).ToArray());
            var solver = new Solver(new DfsFinder(), new NamiraOracle());
            var res = arena.RunAllProblems(solver);
            File.WriteAllText("arena.json", JsonConvert.SerializeObject(res, Formatting.Indented));
            Approvals.Verify(res);
        }
    }
}