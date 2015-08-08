using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lib.ArenaImpl;
using Lib.Models;

namespace Lib.Intelligence
{
    public class AzuraOracle : IOracle, ISolver
    {
        public override string ToString()
        {
            return "Azura";
        }

        public SolverResult Solve(Map map)
        {
            var res = this.PlayExtended(map);
            return new SolverResult(res.Item2.Scores.TotalScores, res.Item1);
        }

        public IEnumerable<OracleSuggestion> GetSuggestions(Map map)
        {
            var possible = this.SuggestAllFinalPositions(map);
            return possible.OrderByDescending(suggestion =>
            {
                var count = 0;
                for (int j = 0; j < map.Filled.GetLength(1); j++)
                {
                    if(map.Filled[suggestion.State.position.X, j])count++;
                }
                return count;
            });
        }
    }
}
