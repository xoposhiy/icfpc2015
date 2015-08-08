using System.IO;
using ApprovalTests;
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
            var arena = new Arena();
//            var solver = new PhrasesOnlySolver();
//            var solver = new NamiraOracle();
            var solver = new AzuraOracle();
            var res = arena.RunAllProblems(solver);
//            File.WriteAllText("arena.json", JsonConvert.SerializeObject(res, Formatting.Indented));
            Approvals.Verify(res);
        }
    }
}