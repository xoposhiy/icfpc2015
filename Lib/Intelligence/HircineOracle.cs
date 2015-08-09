using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lib.Models;

namespace Lib.Intelligence
{
    public class HircineOracle : IOracle
    {
        IOracle backupOracle;
        List<WeightedMetric> metric;
        int lookupDepth;
        int lookupWidth;


        public HircineOracle(IOracle backupOracle,List<WeightedMetric> metric, int lookupDepth, int lookupWidth)
        {
            this.backupOracle = backupOracle;
            this.metric = metric;
            this.lookupDepth = lookupDepth;
            this.lookupWidth = lookupWidth;
        }


        double Evaluate(int step, Map map)
        {
            if (step == lookupDepth) return map.Scores.TotalScores;
            if (map.IsOver) return map.Scores.TotalScores;
            return EvaluateSuggestions(step, map).OrderByDescending(z => z.Metrics).First().Metrics;
            
        }

        IEnumerable<OracleSuggestion> EvaluateSuggestions(int step, Map map)
        {
            var suggestions = OracleServices
                .GetAllFinalPositions(map)
                .Select(suggestion => metric.Evaluate(map, suggestion))
                .OrderByDescending(z => z.Metrics)
                .Take(lookupWidth)
                .ToList();

            var list = new List<OracleSuggestion>();

            foreach (var s in suggestions)
            {
                var newMap = map.TeleportUnit(s.Position).LockUnit();
                list.Add(new OracleSuggestion(s.Position,s.LockingDirection,Evaluate(step+1, map)));
            }

            return list;
        }


        public IEnumerable<OracleSuggestion> GetSuggestions(Map map)
        {
            var result = EvaluateSuggestions(0, map).OrderByDescending(z => z.Metrics).ToList();
            foreach (var e in result)
                yield return e;
            foreach (var e in backupOracle.GetSuggestions(map))
                yield return e;
        }
    }
}
