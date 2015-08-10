using System;
using System.Diagnostics;
using Lib.ArenaImpl;
using Lib.Finder;
using Lib.Models;

namespace Lib.Intelligence
{
    public class AdaptiveSolver : ISolver
    {
        private readonly Phrases phrases;
        private MagicDfsFinder finder;
        public Solver fast;
        public Solver fast2;
        public Solver slowest;
        public Solver slow;
        public Solver fastest;

        public string Name => "Adaptive";

        public AdaptiveSolver(Phrases phrases)
        {
            this.phrases = phrases;
            finder = new MagicDfsFinder(phrases);
            fastest = BuildSolver(new MephalaOracle(new MagicDfsFinder(phrases, 2), WeightedMetric.Keening));
            fast = BuildSolver(new MephalaOracle(finder, WeightedMetric.Keening));
            fast2 = BuildSolver(new MephalaOracle(finder, WeightedMetric.Sunder));
            slowest = BuildSolver(new HircineOracle(finder, WeightedMetric.Debug, 4, 5));
            slow = BuildSolver(new HircineOracle(finder, WeightedMetric.Debug, 3, 5));
        }

        private Solver BuildSolver(IOracle mephalaOracle)
        {
            return new Solver(phrases, finder, mephalaOracle);
        }

        public SolverResult Solve(Map map)
        {
            if (map.Width * map.Height > 50 * 50)
            {
                return fastest.Solve(map);
            }

            var sw = Stopwatch.StartNew();
            var res = fast.Solve(map);

            if (sw.Elapsed.TotalSeconds < 10)
            {
                var res2 = fast2.Solve(map);
                if (res2.Score > res.Score) res = res2;
            }

            if (sw.Elapsed.TotalSeconds < 10 && IsSmall(map))
            {
                var solver = IsSmallest(map) ? slowest : slow;
                var res2 = solver.Solve(map);
                if (res2.Score > res.Score) return res2;
            }
            return res;
        }

        private static bool IsSmall(Map map)
        {
            return map.Width * map.Height <= 15 * 10;
        }

        private static bool IsSmallest(Map map)
        {
            return map.Width * map.Height <= 10 * 10;
        }
    }
}