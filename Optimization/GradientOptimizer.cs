﻿using Lib;
using Lib.Finder;
using Lib.Intelligence;
using Lib.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Optimization
{
    class GradientOptimizer
    {
        class Result
        {
            public double[] vector;
            public double value;
        }


        static Random rnd = new Random();
        static double step = 0.05;
        static double[] baseline = null;
        static int[] mapIndices = new int[]
        { 
       //     0,4,9,23,  // играть в тетрис
        //   1,5,7,// разбирать говно
         //   13,15,20 // хуи вместо фигур
    };
        static int Seed = 4;
        static Map[] maps;

        static Result Shift(Result current, int coordinate, int direction, Func<double[], double> evaluator)
        {
            var newVector = (double[])current.vector.Clone();
            newVector[coordinate] += step * (direction == 0 ? 1 : -1);
            var newValue = evaluator(newVector);
            return new Result { vector = newVector, value = newValue };
        }

        static Result Optimize(Result current, Func<double[],double> evaluator)
        {
            bool[,] tried = new bool[current.vector.Length, 2];
            int coordinate = 0;
            int direction = 0;
            bool cont = false;
            Result newResult = null;
            while(true)
            {
                if (tried.Cast<bool>().All(z => z)) break;
                coordinate = rnd.Next(current.vector.Length);
                direction = rnd.Next(2);
                if (tried[coordinate, direction]) continue;
                tried[coordinate, direction] = true;
                newResult = Shift(current, coordinate, direction, evaluator);
                if (newResult.value > current.value)
                {
                    cont = true;
                    break;
                }
            }
            if (!cont) return null;
            for (int i=0;i<10;i++)
            {
                var veryNew = Shift(newResult, coordinate, direction, evaluator);
                if (veryNew.value <= newResult.value) return newResult;
                newResult = veryNew;
            }
            return newResult;
        }

        static string Print(double[] vector)
        {
            return vector.Select(z => Math.Round(z, 3).ToString()).Aggregate((a, b) => a + "\t" + b);
        }

        static double Evaluate(double[] vector)
        {
            Console.Write(Print(vector)+"\t : ");
            var result = Run(vector);
            for (int i = 0; i < result.Length; i++) result[i] /= baseline[i];
            var n = result.Average();
            Console.WriteLine(n);
            return n;
        }

        static double[] Run(double[] vector)
        {
            var Sunder = new List<WeightedMetric>();
            var functions = WeightedMetric.KnownFunctions.ToArray();
            for (int i = 0; i < functions.Length; i++)
                Sunder.Add(new WeightedMetric(functions[i], vector[i]));

            var result = new double[maps.Length];
            for (int i = 0; i < maps.Length; i++)
            {
                result[i] = Run(maps[i], Sunder);
         //       Console.Write("." + maps[i].Id);
            }
            return result;
        }

        static double Run(Map map, List<WeightedMetric> metric)
        {
            var finder = new BfsNoMagicFinder();
            var mephala = new MephalaOracle(finder, metric);
            var phrases = new Phrases(Phrases.DefaultPowerWords);
            var solver = new Solver(phrases, finder, mephala);
            // Console.Write("Solving ");
            //Console.WriteLine(argument.Code.Select(z => Math.Round(z, 3).ToString()).Aggregate((a, b) => a + " " + b));
            var result = solver.Solve(map);
            //Console.WriteLine("Result" + result.Score + "\n\n");
            return result.Score;
        }



        public static void Main()
        {
            maps = mapIndices
               .Select(z => Problems.LoadProblems()[z].ToMap(Seed))
               .ToArray();


            var vector = new double[WeightedMetric.KnownFunctions.Count()];
            for (int i=0;i<WeightedMetric.KnownFunctions.Count();i++)
            {
                var test = WeightedMetric.Test.Where(z => z.Function == WeightedMetric.KnownFunctions.ElementAt(i)).FirstOrDefault();
                if (test == null) continue;
                vector[i] = test.Weight;
            } 

            Console.Write(Print(vector)+"\t BASELINE");
            baseline = Run(vector);
            var current = new Result { vector = vector, value = 1 };
            Console.WriteLine("  OK");
            while (true)
            {
                var newCurrent = Optimize(current, Evaluate);
                if (newCurrent != null)
                {
                    current = newCurrent;
                    Console.WriteLine(Print(current.vector) + "\t OPT\t" + current.value);
                    continue;
                }
                else
                {
                    Console.WriteLine(Print(current.vector) + "\t END\t" + current.value);
                    Console.ReadKey();
                    break;
                }
            }
        }
    }
}
