using System.Drawing;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Windows.Forms;
using Lib;
using Lib.Models;
using Lib.Finder;
using Lib.Intelligence;

namespace ManualControl
{
    internal class Program
    {
        public static void Main()
        {
            //RunTest(); return;

            var map = Problems.LoadProblems()[0].ToMap(0);

            var model = new MainModel();
            var dfsFinder = new DfsFinder();
//            model.Solver = new Lib.Intelligence.Solver(dfsFinder, new AzuraOracle());
            model.Solver = new Lib.Intelligence.Solver(dfsFinder, new MephalaOracle(dfsFinder, MephalaMetric.HolesOnly));
            model.History = new History(map);
            var form = new TetrisForm(model);
            form.MovementRequested = dir => { map.Unit.Move(dir); };
            Application.Run(form);
        }
    }
}