using System;
using System.Collections.Generic;
using Lib.Models;
using System.Linq;

namespace Lib.Finder
{
    public interface IFinder
    {
        Tuple<int, IEnumerable<Directions>> GetSpellLengthAndPath(Map map, UnitPosition target);

        IEnumerable<Map> GetReachablePositions(Map map);
    }

    public static class FinderExtensions
    {
        public static IEnumerable<Directions> GetPath(this IFinder finder, Map map, UnitPosition target)
        {
            return finder.GetSpellLengthAndPath(map, target).Item2;
        }
    }
}