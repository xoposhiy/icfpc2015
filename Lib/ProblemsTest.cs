using System.IO;
using Newtonsoft.Json;
using NUnit.Framework;

namespace Lib
{
    [TestFixture]
    public class ProblemsTest
    {
        [Test]
        public void Test()
        {
            Problems.LoadProblems();
        }

        [Test]
        public void Beautify()
        {
            Problems.LoadProblems().ForEach(
                p => File.WriteAllText("problems\\beauty_" + p.id + ".json", JsonConvert.SerializeObject(p, Formatting.Indented)));
        }
    }
}