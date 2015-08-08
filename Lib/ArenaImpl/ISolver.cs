using Lib.Models;

namespace Lib.ArenaImpl
{
    public interface ISolver
    {
        SolverResult Solve(Map map);
    }
}