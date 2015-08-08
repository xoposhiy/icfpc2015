using System.Collections.Generic;
using Lib.Models;
using System.Linq;

namespace Lib.Finder
{
    public interface IFinder
    {
        IEnumerable<Directions> GetPath(Map map, UnitPosition target);

        IEnumerable<UnitPosition> GetReachablePositions(Map map);
    }
}