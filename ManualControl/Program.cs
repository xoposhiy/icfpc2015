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
            var phrases = new Phrases(Phrases.DefaultPowerWords);
            var map = Problems.LoadProblems()[9].ToMap(0);

            var model = new MainModel() {FastForwardSteps = 1};
            var dfsFinder = new MagicDfsFinder(phrases);
            var mephala = new MephalaOracle(dfsFinder, WeightedMetric.Keening);
            var hircine = new HircineOracle(dfsFinder, mephala, WeightedMetric.Keening, 4, 7);
            //            model.Solver = new Lib.Intelligence.Solver(dfsFinder, new AzuraOracle());
            model.Solver = new Solver(phrases, dfsFinder, mephala);
            model.History = new History(map);
            var form = new TetrisForm(model) {FastForwardSteps = model.FastForwardSteps};
            form.MovementRequested = dir => { map.Unit.Move(dir); };
            Application.Run(form);
        }
    }
}