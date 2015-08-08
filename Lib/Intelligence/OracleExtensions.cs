using System.Collections;
using System.Collections.Generic;
using Lib.Models;
using System.Linq;

namespace Lib.Intelligence
{

    public static class OracleExtensions
    {
        public static string MakeMove(this IOracle oracle, Map map)
        {
            foreach (var e in oracle.GetSuggestions(map))
            {
                var result = Finder.Finder.GetPath(map.Filled, map.Unit.Unit, e.State);
                if (result == null) continue;
                result += Finder.Finder.DirectionToChar(e.LockingDirection);
                return result;
            }
            return null;
        }

        public static string PlayGame(this IOracle oracle, Map map)
        {
            string result = "";
            while(!map.IsOver)
            {
                try
                {
                    var str = oracle.MakeMove(map);
                    result += str;
                    foreach (var e in str.Select(z => Finder.Finder.CharToDirection(z)))
                        map = map.Move(e);
                }
                catch
                { 
                break;
                }
            }
            return result;
        }

        public static List<OracleSuggestion> SuggestAllFinalPositions(this IOracle oracle, Map map)
        {
            var goodStates = new List<OracleSuggestion>();

            foreach (var state in OracleServices.GetAllStates(map))
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
            return goodStates;
        }
    }
    
}