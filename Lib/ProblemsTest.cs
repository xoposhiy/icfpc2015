using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
            var ps = Problems.LoadProblems();
            foreach (var problem in ps)
            {
                var unitsCells = (int)problem.units.Average(u => u.members.Count) * problem.sourceLength;
                var cells = unitsCells + problem.filled.Count;
                var lines = cells / problem.width;
                var scoreEstimate = unitsCells + 100 * lines;
                Console.WriteLine($"{problem.id}\t{cells}\t{lines}\t{scoreEstimate}");
            }
        }
    }
}