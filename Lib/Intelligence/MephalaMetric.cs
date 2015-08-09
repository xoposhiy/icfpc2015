using Lib.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
