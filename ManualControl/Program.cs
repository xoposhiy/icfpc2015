using System;
using System.Windows.Forms;
using Lib;
using Lib.Finder;
using Lib.Intelligence;

namespace ManualControl
{
    internal class Program
    {
        [STAThread]
        public static void Main()
        {
            var map = Problems.LoadProblems()[5].ToMap(2);

            var model = new MainModel() {FastForwardSteps = 1};
            var dfsFinder = new BfsNoMagicFinder();
            var mephala = new MephalaOracle(dfsFinder, WeightedMetric.Keening);
            //            model.Solver = new Lib.Intelligence.Solver(dfsFinder, new AzuraOracle());
            model.Solver = new Solver(dfsFinder, mephala);
            model.History = new History(map);
            var form = new TetrisForm(model) {FastForwardSteps = model.FastForwardSteps};
            form.MovementRequested = dir => { map.Unit.Move(dir); };
            Application.Run(form);
        }
    }
}