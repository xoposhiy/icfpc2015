
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
        private readonly string name;

        public string Name => name;
        public Solver(IFinder finder, IOracle oracle)
        {
            this.Finder = finder;
            this.Oracle = oracle;
            name = oracle.GetType().Name + "-" + finder.GetType().Name;
        }

        public IEnumerable<Directions> MakeMove(Map map)
        {
            var suggestions = Oracle.GetSuggestions(map).ToList();

            int magicNumber = 0;

            double bestMetrics = -1;
            IEnumerable<Directions> bestPath = null;
            int bestSuggestionIndex = -1;

            for (int i = 0; i < suggestions.Count; i++)
            {
                if (suggestions[i].Metrics / suggestions[0].Metrics < 0.99) break;
                magicNumber++;

                var result = Finder.GetSpellLengthAndPath(map, suggestions[i].Position);
                if (result == null) continue;
                
                var metrics = Math.Min(1, suggestions[i].Metrics + result.Item1 / 100.0);
                if (metrics > bestMetrics)
                {
                    bestMetrics = metrics;
                    bestPath = result.Item2;
                    bestSuggestionIndex = i;
                }
            }
            if(bestPath != null) return bestPath.Concat(new[] { suggestions[bestSuggestionIndex].LockingDirection });

            for (int i = magicNumber; i < suggestions.Count; i++)
            {
                var result = Finder.GetPath(map, suggestions[i].Position);
                if (result == null) continue;
                return result.Concat(new[] { suggestions[i].LockingDirection });
            }

            return null;
        }
       
        public string ResultAsCommands(Map map)
        {
            return ResultAsTuple(map).Item1;
        }

        public Tuple<string, Map> ResultAsTuple(Map map)
        {
            string result = "";
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
            return new SolverResult(name, t.Item2.Scores.TotalScores, t.Item1);
        }
    }
}