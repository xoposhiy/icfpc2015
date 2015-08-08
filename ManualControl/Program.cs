using System.Drawing;
using System.Linq;
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
            var map = Problems.LoadProblems()[3].ToMap(0);
            var model = new MainModel();
            model.Solver = new Lib.Intelligence.Solver(new NullFinder(), new NamiraOracle());
            model.History = new History(map);
            var form = new TetrisForm(model);
            form.MovementRequested = dir => { map.Unit.Move(dir); };
            Application.Run(form);
        }
    }
}