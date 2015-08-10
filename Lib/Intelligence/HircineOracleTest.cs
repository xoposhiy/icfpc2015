using System;
using System.Collections.Immutable;
using System.Drawing;
using Lib.Models;
using NUnit.Framework;

namespace Lib.Intelligence
{
    [TestFixture]
    public class HircineOracleTest
    {
        [Test]
        public void Test()
        {
            var map = CreateMap(new[]
            {
                "....",
                @"....",
                "....",
                @"#.##",
                "#.##",
            }, new Unit(new[] {new Point(0, 0), new Point(0, 1) }, new Point(0, 0)));
            var oracle = new HircineOracle(null, WeightedMetric.Keening, 3, 5);
            var sg = oracle.EvaluateSuggestions(0, map);
            foreach (var s in sg)
            {
                Console.WriteLine(s.Metrics + " " + s.Position.Point + " " + s.Position.Angle);
            }
        }

        private Map CreateMap(string[] mapLines, Unit unit)
        {
            var f = new bool[mapLines[0].Length, mapLines.Length];
            for(int y=0; y<mapLines.Length; y++)
                for (int x = 0; x < mapLines[0].Length; x++)
                    f[x, y] = mapLines[y][x] == '#';
            var positionedUnit = new PositionedUnit(unit, new UnitPosition(new Point(3, 0), 0));
            return new Map(0, f, positionedUnit, ImmutableStack<Unit>.Empty, ImmutableHashSet<PositionedUnit>.Empty, new Scores(0, 0));
        }
    }
}