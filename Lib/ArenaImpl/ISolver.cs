using Lib.Models;

namespace Lib.ArenaImpl
{
    public interface ISolver
    {
        string Name { get; }
        SolverResult Solve(Map map);
    }
}