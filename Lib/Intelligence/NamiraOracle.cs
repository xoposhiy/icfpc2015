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
   public class NamiraOracle : IOracle, ISolver
    {
        public IEnumerable<OracleSuggestion> GetSuggestions(Map map)
        {
            var goodStates = new List<OracleSuggestion>();

            foreach(var state in OracleServices.GetAllStates(map))
            {
                var positionedUnit = map.Unit.TranslateToState(state);
                if (map.IsLockingState(positionedUnit)) continue;

                foreach (var dir in OracleServices.GetAllDirections())
                {
                    var nextPosition = positionedUnit.Move(dir);
                    if (map.IsLockingState(nextPosition))
                    {
                        goodStates.Add(new OracleSuggestion(state, dir));
                    }
                }
            }

            return goodStates.OrderByDescending(z => z.State.position.Y);
        }

        public override string ToString()
        {
            return "Namira";
        }

       public SolverResult Solve(Map map)
       {
           var res = this.PlayExtended(map);
           return new SolverResult(res.Item2.Scores.TotalScores, res.Item1); 
       }
    }
}