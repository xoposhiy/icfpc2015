using Lib.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Lib.Intelligence
{
    public class MephalaMetric
    {
        public readonly Func<Map, Map, PositionedUnit, double> Function;
        public readonly double Weight;
        public MephalaMetric(Func<Map, Map, PositionedUnit, double> function, double weight)
        {
            this.Function=function;
            this.Weight = weight;
        }

        public static readonly List<MephalaMetric> HolesOnly = new List<MephalaMetric> { new MephalaMetric(Metrics.ShouldNotCreateSimpleHoles, 1) };

        public static readonly List<MephalaMetric> Combined = new List<MephalaMetric> { new MephalaMetric(Metrics.ShouldNotCreateSimpleHoles, 1), new MephalaMetric(Metrics.ShouldEraseLines,2) };

        public static readonly List<MephalaMetric> LinesOnly = new List<MephalaMetric> { new MephalaMetric(Metrics.ShouldEraseLines, 1) };


        public static readonly List<MephalaMetric> Keening = new List<MephalaMetric>
        {
            new MephalaMetric(Metrics.GoDown, 1),
            new MephalaMetric(Metrics.ShouldEraseLines,3),
            new MephalaMetric(Metrics.MinimizeClosureIndex,1.5)
        };
    }

    public static class MephalaMetricListExtensions
    {
        public static OracleSuggestion Evaluate(this IEnumerable<MephalaMetric> metrics, Map before, OracleSuggestion suggestions)
        {
            var positionedUnit = new PositionedUnit(before.Unit.Unit, suggestions.Position);
            var after = before.TeleportUnit(suggestions.Position).LockUnit();
            return new OracleSuggestion(
                suggestions.Position,
                suggestions.LockingDirection,
                metrics.Sum(z => z.Function(before, after, positionedUnit) * z.Weight));
        }                
    }
    
}
