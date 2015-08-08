using System.Collections.Generic;
using System.Linq;
using Lib.Models;

namespace Lib.Intelligence
{
    public class NamiraOracle : IOracle
    {
        public IEnumerable<OracleSuggestion> GetSuggestions(Map map)
        {
            return map.SuggestAllFinalPositions().OrderByDescending(z => z.Position.Point.Y);
        }

        public override string ToString()
        {
            return "Namira";
        }
    }
}