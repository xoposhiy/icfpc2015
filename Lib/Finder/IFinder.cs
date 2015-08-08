using Lib.Models;

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
            return null;
        }
    }
}