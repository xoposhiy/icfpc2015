using System;
using System.IO;
using System.Windows.Forms;
using Lib;
using Lib.ArenaImpl;
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
            if (Directory.Exists("logs"))
                Directory.Delete("logs", true);
            var map = Problems.LoadProblems()[9].ToMap(0);

            var model = new MainModel() {FastForwardSteps = 1};
            var solver = new AdaptiveSolver(phrases);
            model.Solver = solver.fast;
            model.History = new History(map);
            var form = new TetrisForm(model) {FastForwardSteps = model.FastForwardSteps};
            form.MovementRequested = dir => { map.Unit.Move(dir); };
            Application.Run(form);
        }
    }
}