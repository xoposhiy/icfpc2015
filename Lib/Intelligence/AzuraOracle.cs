using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lib.ArenaImpl;
using Lib.Finder;
using Lib.Models;

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
            var possible = map.SuggestAllFinalPositions();

            return possible.OrderByDescending(suggestion =>
            {
                var count = 0;
                for (int j = 0; j < map.Filled.GetLength(0); j++)
                {
                    if(map.Filled[j, suggestion.Position.Point.X])count++;
                }
                return count;
            }).ThenByDescending(z => z.Position.Point.Y);
        }
    }
}
