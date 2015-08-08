using System.IO;
using ApprovalTests;
using Newtonsoft.Json;
using NUnit.Framework;

namespace Lib.ArenaImpl
{
    [TestFixture]
    public class ArenaModelTest
    {
        [Test]
        public void Test()
        {
            var arena = new Arena();
            var solver = new PhrasesOnlySolver();
            var res = arena.RunAllProblems(solver);
//            File.WriteAllText("arena.json", JsonConvert.SerializeObject(res, Formatting.Indented));
            Approvals.Verify(res);
        }
    }
}