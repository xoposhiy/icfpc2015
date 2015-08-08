using System;
using System.Linq;

namespace Lib.ArenaImpl
{
    public class Arena
    {
        public readonly ArenaProblem[] Problems;

        public Arena() : this(Lib.Problems.LoadProblems().ToArray())
        {
        }

        public Arena(ProblemJson[] problems)
        {
            this.Problems = problems.Select(p => new ArenaProblem
            {
                Problem = p,
                MapResults = p.sourceSeeds.Select(seed => new ArenaMapResult { Seed = seed }).ToArray()
            }).ToArray();
        }

        public ArenaModel RunAllProblems(ISolver solver)
        {
            foreach (var arenaProblem in Problems)
            {
                RunProblem(arenaProblem, solver);
            }
            return new ArenaModel { Problems = Problems, SolverName = solver.Name };
        }

        public ArenaModel RunProblem(ArenaProblem problem, ISolver solver)
        {
            for (int i = 0; i < problem.Problem.sourceSeeds.Count; i++)
            {
                Run(problem, i, solver);
            }
            return new ArenaModel { Problems = Problems };
        }

        public void Run(ArenaProblem problem, int seedIndex, ISolver solver)
        {
            try
            {
                var prob = problem.Problem;
                var result = solver.Solve(prob.ToMap(prob.sourceSeeds[seedIndex]));
                problem.MapResults[seedIndex].Result = result;
            }
            catch (Exception exception)
            {
                throw new Exception($"problemId: {problem.Problem.id}, seed: {seedIndex}. {exception.Message}", exception);
            }
        }
    }
}