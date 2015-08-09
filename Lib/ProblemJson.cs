using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Runtime.CompilerServices;
using ApprovalTests;
using ApprovalTests.Reporters;
using Lib.Models;
using Newtonsoft.Json;
using NUnit.Framework;

namespace Lib
{
    public class ProblemJson
    {
        [JsonIgnore]
        public int ScoreEstimate
        {
            get
            {
                var unitsCells = (int)units.Average(u => u.members.Count) * sourceLength;
                var cells = unitsCells + filled.Count;
                var lines = cells / width;
                return unitsCells + 100 * lines + 300 * (Phrases.DefaultPowerWords.Length - 1);
            }
        }
        public int height { get; set; }

        public int width { get; set; }

        public List<int> sourceSeeds { get; set; }

        public List<UnitJson> units { get; set; }

        public int id { get; set; }

        public List<CellJson> filled { get; set; }

        public int sourceLength { get; set; }

        public Map ToMap(int seedValue)
        {
            var f = new bool[width, height];
            foreach (var cell in filled)
                f[cell.x, cell.y] = true;
            var us = units.Select(u => u.ToUnit()).ToList();
            var g = new LinearGenerator(seedValue);
            var unitsSeq = Enumerable
                .Range(0, sourceLength)
                .Select(i => us[g.Next() % us.Count])
                .Reverse()
                .Aggregate(ImmutableStack<Unit>.Empty, (stack, unit) => stack.Push(unit));
            return new Map(id, f, unitsSeq, new Scores(0, 0));
        }
    }

    [TestFixture]
    public class ProblemJsonTest
    {
        [MethodImpl(MethodImplOptions.NoInlining)]
        [Test, UseReporter(typeof(DiffReporter))]
        public void Test()
        {
            var p = new ProblemJson
            {
                filled = new List<CellJson> { new CellJson { x = 1, y = 2 } },
                height = 10,
                width = 12,
                id = 42,
                sourceLength = 22,
                sourceSeeds = new List<int> { 1, 2, 3 },
                units = new List<UnitJson>
                {
                    new UnitJson
                    {
                        members = new List<CellJson> {new CellJson {x = 10, y = 11}},
                        pivot = new CellJson {x = 10, y = 11}
                    }
                }
            };
            var s = JsonConvert.SerializeObject(p, Formatting.Indented);
            Approvals.Verify(s);
        }
    }
}