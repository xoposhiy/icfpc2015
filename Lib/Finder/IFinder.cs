using Lib.Models;
using System.Linq;

namespace Lib.Finder
{
    public interface IFinder
    {
        Directions[] GetPath(Map map, UnitPosition target);
    }

    public class NullFinder : IFinder
    {
        public Directions[] GetPath(Map map, UnitPosition target)
        {
            return "lllllllllllll".ToDirections().ToArray();
        }
    }
}