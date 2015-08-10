using System;
using System.Collections.Generic;
using System.Linq;
using Lib.Finder;
using Lib.Models;

namespace Lib.Intelligence
{
    public class HircineOracle : IOracle
    {
        private readonly IFinder finder;
        private readonly List<WeightedMetric> metric;
        private readonly int lookupDepth;
        private readonly int lookupWidth;

        public HircineOracle(IFinder finder, List<WeightedMetric> metric, int lookupDepth, int lookupWidth)
        {
            this.finder = finder;
            this.metric = metric;
            this.lookupDepth = lookupDepth;
            this.lookupWidth = lookupWidth;
        }

        private Tuple<double, Map> Evaluate(int depth, Map map, double metricValue)
        {
            if (depth == lookupDepth)
                return Tuple.Create(map.Scores.TotalScores + metricValue, map);
            if (map.IsOver)
                return Tuple.Create(map.Scores.TotalScores + 0.0, map);
            var bestMap = EvaluateSuggestions(depth, map).MaxItem(z => z.Metrics);
            return Tuple.Create(bestMap.Metrics, bestMap.LockedFinalMap);
        }

        public IEnumerable<OracleSuggestion> EvaluateSuggestions(int depth, Map map)
        {
            var suggestions = OracleServices
                .GetReachableSugessions(finder, map)
                .Select(suggestion => metric.Evaluate(map, suggestion))
                .OrderByDescending(z => z.Metrics)
                .Take(Math.Max(2, lookupWidth - depth))
                .ToList();

            return
                from s in suggestions
                let evaluatedFinalMap = Evaluate(depth + 1, s.LockedFinalMap, s.Metrics)
                select new OracleSuggestion(
                    s.Position, s.LockingDirection,
                    evaluatedFinalMap.Item2, evaluatedFinalMap.Item1);
        }

        public IEnumerable<OracleSuggestion> GetSuggestions(Map map)
        {
            return EvaluateSuggestions(0, map).OrderByDescending(z => z.Metrics);
        }
    }
}