using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lib.ArenaImpl;
using Lib.Finder;
using Lib.Models;
using NUnit.Framework.Constraints;

namespace Lib.Intelligence
{
    public class AzuraOracle : IOracle
    {
        public override string ToString()
        {
            return "Azura";
        }

        public IEnumerable<OracleSuggestion> GetSuggestions(Map map)
        {
            var allUnitPositions = OracleServices.GetAllUnitPositions(map);

            var suggestions = new List<OracleSuggestion>();
            foreach (var position in allUnitPositions)
            {
                var positionedUnit = map.Unit.WithNewPosition(position);
                if (!map.IsValidPosition(positionedUnit)) continue;
                if(map.IsEmptyPosition(positionedUnit)) continue;

                foreach (var dir in OracleServices.GetAllDirections())
                {
                    var nextPosition = positionedUnit.Move(dir);
                    if (!map.IsValidPosition(nextPosition))
                    {
                        suggestions.Add(new OracleSuggestion(position, dir));
                        break;
                    }
                }
            }
            return suggestions;
        }
    }
}
