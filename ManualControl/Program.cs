using System.Drawing;
using System.Windows.Forms;
using Lib;
using Lib.Models;

namespace ManualControl
{
    internal class Program
    {
        private static Point p1 = new Point(-5, 0);
        private static Point p2 = new Point(-5, 1);

        public static void RunTest()
        {
            var form = new TetrisForm(20, 14);
            form.GetMap = (x, y) => Occupation.Empty;
            form.LabelXOffset = 9;
            form.LabetYOffset = 6;
            Application.Run(form);
        }

        public static void Main()
        {
            //RunTest(); return;
            var map = new MapBuilder().BuildFrom(Problems.LoadProblems()[7], 0);
            var form = new TetrisForm(map.Width, map.Height);
            form.GetMap = (x, y) =>
            {
                if (x.InRange(0, map.Width - 1) && y.InRange(0, map.Height - 1))
                {
                    if (map.Filled[x, y]) return Occupation.Occupied;
                    var shiftedPoint = new Point(x - map.Unit.Pivot.X, y - map.Unit.Pivot.Y);
                    if (map.Unit.Rotations[map.Unit.RotationIndex].Contains(shiftedPoint)) return Occupation.Unit;
                    return Occupation.Empty;
                }
                return Occupation.Occupied;
            };
            form.MovementRequested = dir => { map.Unit.Move(dir); };
            Application.Run(form);
        }
    }
}