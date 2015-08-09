using Lib.Models;
using System;
using System.Collections.Generic;

namespace Lib.Intelligence
{
    public class MephalaMetric
    {
        public readonly Func<Map, PositionedUnit, double> Function;
        public readonly double Weight;
        public MephalaMetric(Func<Map, PositionedUnit, double> function, double weight)
        {
            this.Function=function;
            this.Weight = weight;
        }

        public static readonly List<MephalaMetric> HolesOnly = new List<MephalaMetric> { new MephalaMetric(Metrics.ShouldNotCreateSimpleHoles, 1) };

        public static readonly List<MephalaMetric> Combined = new List<MephalaMetric> { new MephalaMetric(Metrics.ShouldNotCreateSimpleHoles, 1), new MephalaMetric(Metrics.ShouldEraseLines,2) };

        public static readonly List<MephalaMetric> LinesOnly = new List<MephalaMetric> { new MephalaMetric(Metrics.ShouldEraseLines, 1) };
    }

    public static class MephalaMetricListExtensions
    {
        public static OracleSuggestion Evaluate(this IEnumerable<MephalaMetric> metrics, Map map, OracleSuggestion suggestions)
        {
            var positionedUnit = new PositionedUnit(map.Unit.Unit, suggestions.Position);
            return new OracleSuggestion(
                suggestions.Position,
                suggestions.LockingDirection,
                metrics.Sum(z => z.Function(map, positionedUnit) * z.Weight));
        }                
    }
    
}
