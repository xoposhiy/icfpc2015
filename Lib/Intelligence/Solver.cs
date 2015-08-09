using System;
using System.Collections.Generic;
using System.Linq;
using Lib.ArenaImpl;
using Lib.Finder;
using Lib.Models;

namespace Lib.Intelligence
{
    public class Solver : ISolver
    {
        public readonly IFinder Finder;
        public readonly IOracle Oracle;
        private readonly int bestSugessionsCount;
        private readonly double metricEpsilon;

        public Solver(IFinder finder, IOracle oracle, int bestSugessionsCount = 20, double metricEpsilon = 1)
        {
            Finder = finder;
            Oracle = oracle;
            this.bestSugessionsCount = bestSugessionsCount;
            this.metricEpsilon = metricEpsilon;
            Name = oracle.GetType().Name + "-" + finder.GetType().Name;
        }

        public string Name { get; }

        public IEnumerable<Directions> MakeMove(Map map)
        {
            var suggestions = Oracle.GetSuggestions(map).ToList();
            if (suggestions.Count == 0) return null;
            var bestMetric = suggestions[0].Metrics;
            var selectedSugessions = suggestions
                .Take(bestSugessionsCount)
                .Where(s => s.Metrics >= bestMetric * metricEpsilon).ToList();
            return
                selectedSugessions
                    .Select(s => GetPath(map, s))
                    .MaxItem(path => path.ToPhrase().ToOriginalPhrase().GetPowerScore());
        }

        private IEnumerable<Directions> GetPath(Map map, OracleSuggestion s)
        {
            return Finder.GetSpellLengthAndPath(map, s.Position).Item2.Concat(new[] { s.LockingDirection });
        }

        public string ResultAsCommands(Map map)
        {
            return ResultAsTuple(map).Item1;
        }

        public Tuple<string, Map> ResultAsTuple(Map map)
        {
            var result = "";
            while (!map.IsOver)
            {
                var dirs = MakeMove(map).ToList();
                result += dirs.ToPhrase();
                map = dirs.Aggregate(map, (m, dir) => m.Move(dir));
            }
            return Tuple.Create(result, map);
        }

        public SolverResult Solve(Map map)
        {
            var t = ResultAsTuple(map);
            var commands = t.Item1.ToOriginalPhrase();
            var score = t.Item2.Scores.TotalScores + commands.GetPowerScore();
            return new SolverResult(Name, score, commands);
        }
    }
}