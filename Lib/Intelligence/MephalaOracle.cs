﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lib.Finder;
using Lib.Models;

namespace Lib.Intelligence
{
    public class MephalaOracle: IOracle
    {
        private readonly List<WeightedMetric> metrics;
        private readonly IFinder finder;

        public MephalaOracle(IFinder finder, List<WeightedMetric> metrics)
        {
            this.metrics = metrics;
            this.finder = finder;
        }
         
        public IEnumerable<OracleSuggestion> GetSuggestions(Map map)
        {
            var finalMaps = finder.GetReachablePositions(map);

            var suggestions = new List<OracleSuggestion>();
            foreach (var finalMap in finalMaps)
            {
                var positionedUnit = finalMap.Unit;
                var lockedMap = finalMap.LockUnit();

                foreach (var dir in OracleServices.GetAllDirections())
                {
                    if (!finalMap.IsValidPosition(positionedUnit.Move(dir)))
                    {
                        var m = metrics.Sum(z => z.Function(map,lockedMap, positionedUnit) * z.Weight);
                        suggestions.Add(new OracleSuggestion(finalMap.Unit.Position, dir, lockedMap, m));
                        break;
                    }
                }
            }
            return suggestions.OrderByDescending(s => s.Metrics);
        }
    }
}
