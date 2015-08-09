﻿using Lib.Intelligence.Metrics;
using Lib.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lib.Intelligence
{
    public class WeightedMetric
    {
        public readonly Func<Map, Map, PositionedUnit, double> Function;
        public readonly double Weight;
        public WeightedMetric(Func<Map, Map, PositionedUnit, double> function, double weight)
        {
            this.Function=function;
            this.Weight = weight;
        }

        public static IEnumerable<Func<Map,Map,PositionedUnit,double>> KnownFunctions
        {
            get
            {
                yield return SimpleMetrics.GoDown;
                yield return SimpleMetrics.EraseLines;
                yield return ClosureIndex.Minimize;
                yield return LineSlots.Maximize;
            }
        }

        public static List<WeightedMetric> CreateCombination(params double[] weights)
        {
            var list = new List<WeightedMetric>();
            var functions = KnownFunctions.ToArray();
            for (int i = 0; i < weights.Length; i++)
                if (Math.Abs(weights[i]) > 0.01)
                    list.Add(new WeightedMetric(functions[i], weights[i]));
            return list;
        }

        public static readonly List<WeightedMetric> Keening = CreateCombination(0.25, 0.5, 0.25, 0);

        public static readonly List<WeightedMetric> Sunder = CreateCombination(0.25, 0.5, 0.25, 0.1);
    }

    public static class MephalaMetricListExtensions
    {
        public static OracleSuggestion Evaluate(this IEnumerable<WeightedMetric> metrics, Map before, OracleSuggestion suggestions)
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