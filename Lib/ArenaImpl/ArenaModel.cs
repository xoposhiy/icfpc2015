using System;
using System.Linq;

namespace Lib.ArenaImpl
{
    public class ArenaModel
    {
        public string SolverName;
        public ArenaProblem[] Problems;

        public int TotalScore { get { return Problems.Sum(p => p.AvgScore); } }
        public override string ToString()
        {
            var lines = Problems.Select(p => p.ToString());
            return 
                SolverName + Environment.NewLine
                + "TotalScore: " + TotalScore + Environment.NewLine
                + "ProblemId\tAvgScore\tScoreBySeeds" + Environment.NewLine 
                + string.Join(Environment.NewLine, lines);
        }
    }
}
