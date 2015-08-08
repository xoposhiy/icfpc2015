﻿
using System;
using System.Collections.Generic;
using System.Linq;
using Lib.ArenaImpl;
using Lib.Finder;
using Lib.Models;

namespace Lib.Intelligence
{
    public class Solver : ISolver
    {
        private IFinder finder;
        private IOracle oracle;
        private readonly string name;

        public Solver(IFinder finder, IOracle oracle)
        {
            this.finder = finder;
            this.oracle = oracle;
            name = oracle.GetType().Name + "-" + finder.GetType().Name;
        }

        public IEnumerable<Directions> MakeMove(Map map)
        {
            foreach (var e in oracle.GetSuggestions(map))
            {
                Directions[] result = finder.GetPath(map, e.Position);
                if (result == null) continue;
                return result.Concat(new[] { e.LockingDirection });
            }
            return null;
        }
       
        public string ResultAsCommands(Map map)
        {
            return ResultAsTuple(map).Item1;
        }

        public Tuple<string, Map> ResultAsTuple(Map map)
        {
            string result = "";
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
            return new SolverResult(name, t.Item2.Scores.TotalScores, t.Item1);
        }
    }
}