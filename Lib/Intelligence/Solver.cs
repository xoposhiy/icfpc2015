using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Lib.ArenaImpl;
using Lib.Finder;
using Lib.Models;
using NLog;

namespace Lib.Intelligence
{

    public class AdaptiveSolver : ISolver
    {
        private readonly Phrases phrases;
        private MagicDfsFinder finder;
        private Solver fast;
        private Solver slowest;
        private Solver slow;

        public string Name => "Adaptive";

        public AdaptiveSolver(Phrases phrases)
        {
            this.phrases = phrases;
            finder = new MagicDfsFinder(phrases);
            IOracle mephalaOracle = new MephalaOracle(finder, WeightedMetric.Keening);
            fast = BuildSolver(mephalaOracle);
            slowest = BuildSolver(new HircineOracle(finder, WeightedMetric.Debug, 5, 5));
            slow = BuildSolver(new HircineOracle(finder, WeightedMetric.Debug, 3, 5));
        }

        private Solver BuildSolver(IOracle mephalaOracle)
        {
            return new Solver(phrases, finder, mephalaOracle);
        }

        public SolverResult Solve(Map map)
        {
            var sw = Stopwatch.StartNew();
            var res = fast.Solve(map);
            if (sw.Elapsed.TotalSeconds < 4 && IsSmall(map))
            {
                var solver = IsSmallest(map) ? slowest : slow;
                Console.WriteLine($"{res.Score} on fast. Try slow " + solver);
                var res2 = IsSmallest(map) ? slowest.Solve(map) : slow.Solve(map);
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

    public class Solver : ISolver
    {
        private readonly Phrases phrases;
        public readonly IFinder Finder;
        public readonly IOracle Oracle;
        private readonly int bestSugessionsCount;
        private readonly double metricEpsilon;

        public override string ToString()
        {
            return Name;
        }

        public Solver(Phrases phrases, IFinder finder, IOracle oracle, int bestSugessionsCount = 20, double metricEpsilon = 1)
        {
            this.phrases = phrases;
            Finder = finder;
            Oracle = oracle;
            this.bestSugessionsCount = bestSugessionsCount;
            this.metricEpsilon = metricEpsilon;
            Name = oracle.GetType().Name + "-" + finder.GetType().Name;
        }

        public string Name { get; }

        public List<Directions> MakeMove(Map map)
        {
            var suggestions = Oracle.GetSuggestions(map).ToList();
            log.Info(map);
            LogSugessions("all", suggestions);
            if (suggestions.Count == 0) return null;
            var bestMetric = suggestions[0].Metrics;
            var selectedSugessions = suggestions
                .Take(bestSugessionsCount)
                .Where(s => s.Metrics >= bestMetric * metricEpsilon).ToList();
            LogSugessions("selected", selectedSugessions);
            var sugestionsWithPaths =
                from s in selectedSugessions
                let path = GetPath(map, s).ToList()
                let phrase = path.ToPhrase()
                let powerScore = phrases.GetPowerScore(phrases.ToOriginalPhrase(phrase))
                select new {s, path, phrase, powerScore};
            var theOne = sugestionsWithPaths.MaxItem(s => s.powerScore);
            log.Info($"SelectedMove: {theOne.powerScore} {theOne.phrase}\r\n*{theOne.s} -> \r\n{theOne.s.LockedFinalMap}");
            return theOne.path;
        }

        private static Logger log = LogManager.GetCurrentClassLogger();

        private void LogSugessions(string which, IEnumerable<OracleSuggestion> suggestions)
        {
            log.Info(which + " sugessions:\r\n " + string.Join("\r\n", suggestions));
        }

        private IEnumerable<Directions> GetPath(Map map, OracleSuggestion s)
        {
            return Finder.GetSpellLengthAndPath(map, s.Position).Item2.Concat(new[] { s.LockingDirection });
        }

        public string ResultAsCommands(Map map)
        {
            return ResultAsTuple(map).Item1;
        }

        public Tuple<string, Map> ResultAsTuple(Map map)
        {
            var result = "";
            while (!map.IsOver)
            {
                var dirs = MakeMove(map).ToList();
                result += dirs.ToPhrase();
                map = dirs.Aggregate(map, (m, dir) => m.Move(dir));
            }
            return Tuple.Create(result, map);
        }

        public SolverResult Solve(Map map)
        {
            var t = ResultAsTuple(map);
            var commands = phrases.ToOriginalPhrase(t.Item1);
            var score = t.Item2.Scores.TotalScores + phrases.GetPowerScore(commands);
            return new SolverResult(Name, score, commands);
        }
    }
}