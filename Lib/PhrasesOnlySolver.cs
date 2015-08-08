using System;
using System.Linq;
using System.Text;
using Lib.ArenaImpl;
using Lib.Models;
using NUnit.Framework;

namespace Lib
{
    public class PhrasesOnlySolver : ISolver
    {
        public static Tuple<string, Map> SolveMap(Map map)
        {
            var combo = "pbaldk" + string.Join("", Phrases.all);
            int comboIndex = 0;
            var commands = new StringBuilder();
            while (!map.IsOver)
            {
                var c = combo[comboIndex % combo.Length];
                var move = c.ToDirection();
                if (!map.IsCatastrophicMove(move))
                {
                    map = map.Move(move);
                    commands.Append(c);
                }
                comboIndex++;
            }
            return Tuple.Create(commands.ToString(), map);
        }

        public SolverResult Solve(Map map)
        {
            var res = SolveMap(map);
            return new SolverResult(res.Item2.Scores.TotalScores, res.Item1);
        }
    }

    [TestFixture]
    public class PhrasesOnlySolverTest
    {
        [Test]
        public void Test()
        {
            var solver = new PhrasesOnlySolver();
            var map = Problems.LoadProblems()[3].ToMap(0);
            var result = solver.Solve(map);
            Console.WriteLine(result);
        }
    }
}