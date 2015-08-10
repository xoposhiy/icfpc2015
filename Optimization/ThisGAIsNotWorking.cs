using AIRLab.GeneticAlgorithms;
using Lib;
using Lib.Finder;
using Lib.Intelligence;
using Lib.Intelligence.Metrics;
using Lib.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SetPartition
{
    static class SetPartition
    {
        static Random rnd = new Random();
        static Tuple<int, int>[] mapIndices = new Tuple<int, int>[]
            {
                Tuple.Create(0,0)
            };
        static Map[] maps;
        static double[] baseline;


        static List<Func<Map, Map, PositionedUnit, double>> functions; 

        static double Run(ArrayChromosome<double> argument)
        {
            var Sunder = new List<WeightedMetric>();
            for (int i = 0; i < functions.Count; i++)
                Sunder.Add(new WeightedMetric(functions[i], argument.Code[i]));

            var result = maps.Select(z => Run(z, Sunder)).ToArray();
            for (int i = 0; i < result.Length; i++)
                result[i] /= baseline[i];
            return result.Average();
          }

        static double Run(Map map, List<WeightedMetric> metric)
        {
            var finder = new BfsNoMagicFinder();
            var mephala = new MephalaOracle(finder, metric);
            var solver = new Solver(finder, mephala);
            // Console.Write("Solving ");
            //Console.WriteLine(argument.Code.Select(z => Math.Round(z, 3).ToString()).Aggregate((a, b) => a + " " + b));
            var result = solver.Solve(map);
            //Console.WriteLine("Result" + result.Score + "\n\n");
            return result.Score;
        }

        static string Print(ArrayChromosome<double> argument)
        {
            var sum = argument.Code.Select(z=>Math.Abs(z)).Sum();
            return argument.Code.Select(z => Math.Round(z/sum, 3).ToString()).Aggregate((a, b) => a + " " + b);
        }

        static double Baseline;


        static void MainX()
        {
            functions= WeightedMetric.KnownFunctions.ToList();


            maps = mapIndices
                .Select(z => Problems.LoadProblems()[z.Item1].ToMap(z.Item2))
                .ToArray();
            baseline = maps.Select(z => Run(z, WeightedMetric.Keening)).ToArray();

            var ga = new GeneticAlgorithm<ArrayChromosome<double>>(
                () => new ArrayChromosome<double>(functions.Count)
                , rnd);

            Solutions.AppearenceCount.MinimalPoolSize(ga, 10);
            Solutions.MutationOrigins.Random(ga,0.5);
            //Solutions.CrossFamilies.Random(ga, z => z * 0.5);
            Solutions.Selections.Threashold(ga, 8);



            ArrayGeneSolutions.Appearences.Double(ga);
            ArrayGeneSolutions.Mutators.Double(ga,0.1,0.1,0.5);
           // ArrayGeneSolutions.Crossover.Mix(ga);



            ga.Evaluate = chromosome =>
                {
                    chromosome.Value = Run(chromosome); 
                };

            ConsoleGui.Run(ga, 1, Print);
        }
    }
}
