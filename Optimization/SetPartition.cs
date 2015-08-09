using AIRLab.GeneticAlgorithms;
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
        static int Count = 5000;
        static int MaxWeight = 5000;
        static int[] Weights;
        static Random rnd = new Random(1);

        static void Main()
        {
            Weights = Enumerable.Range(0, Count).Select(z => rnd.Next(MaxWeight)).ToArray();


            var ga = new GeneticAlgorithm<ArrayChromosome<bool>>(
                () => new ArrayChromosome<bool>(Count)
                , rnd);

            Solutions.AppearenceCount.MinimalPoolSize(ga, 40);
            Solutions.MutationOrigins.Random(ga, 0.5);
            Solutions.CrossFamilies.Random(ga, z => z * 0.5);
            Solutions.Selections.Threashold(ga, 40);



            ArrayGeneSolutions.Appearences.Bool(ga);
            ArrayGeneSolutions.Mutators.Bool(ga);
            ArrayGeneSolutions.Crossover.Mix(ga);



            ga.Evaluate = chromosome =>
                {
                    chromosome.Value =
                        1.0 / (1 + Math.Abs(Enumerable.Range(0, Count).Sum(z => Weights[z] * (chromosome.Code[z] ? -1 : 1))));
                };

            ConsoleGui.Run(ga, 10);
        }
    }
}
