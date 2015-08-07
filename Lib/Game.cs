using Lib.Models;

namespace Lib
{
    public class Game
    {
        public readonly ProblemJson Problem;
        public Map Map;
        private readonly MapBuilder builder = new MapBuilder();
        private readonly LinearGenerator gen = new LinearGenerator();

        public Game(ProblemJson problem)
        {
            Problem = problem;
            gen.SetSeed(problem.sourceSeeds[0]);
            Map = builder.BuildFrom(problem, (int)(gen.Next() % problem.units.Count));
        }
    }
}