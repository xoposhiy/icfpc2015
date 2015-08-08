using System.Collections;
using System.Collections.Generic;
using Lib.Models;

namespace Lib.Intelligence
{

    public static class OracleExtensions
    {
        public static string MakeMove(this IOracle oracle, Map map)
        {
            foreach(var e in oracle.GetSuggestions(map))
            {
                var result = Finder.Finder.GetPath(map.Filled, map.Unit.Unit, e.State);
                if (result == null) continue;
                result += Finder.Finder.DirectionToChar(e.LockingDirection);
                return result;
            }
            return null;
        }
    }
    
}