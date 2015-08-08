using System;
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
        private readonly Func<Map, PositionedUnit, double> metrics;
        private readonly IFinder finder;

        public MephalaOracle(IFinder finder, Func<Map, PositionedUnit, double> metrics)
        {
            this.metrics = metrics;
            this.finder = finder;
        }
         
        public IEnumerable<OracleSuggestion> GetSuggestions(Map map)
        {
            var positions = finder.GetReachablePositions(map);

            var suggestions = new List<OracleSuggestion>();
            foreach (var position in positions)
            {
                var positionedUnit = map.Unit.WithNewPosition(position);

                foreach (var dir in OracleServices.GetAllDirections())
                {
                    var nextPosition = positionedUnit.Move(dir);
                    if (!map.IsValidPosition(nextPosition))
                    {
                        suggestions.Add(new OracleSuggestion(position, dir, metrics(map, positionedUnit)));
                        break;
                    }
                }
            }
            return suggestions.OrderByDescending(s => s.Metrics);
        }
    }
}
