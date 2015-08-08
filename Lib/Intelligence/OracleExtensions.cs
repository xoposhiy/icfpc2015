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

    }
    
}