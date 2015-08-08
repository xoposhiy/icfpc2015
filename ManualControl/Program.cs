using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Lib;
using Lib.Models;

namespace ManualControl
{
    internal class Program
    {
        public static void Main()
        {
            //RunTest(); return;
            var map = Problems.LoadProblems()[0].ToMap(0);
            var form = new TetrisForm(map);
            form.MovementRequested = dir => { map.Unit.Move(dir); };
            Application.Run(form);
        }
    }
}