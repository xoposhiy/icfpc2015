using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ManualControl
{
    class Program
    {
        static Point p1 = new Point(5,0);
        static Point p2 = new Point(5, 1);

        public static void Main()
        {
            var form = new TetrisForm(10, 10);
            form.GetMap = (x, y) =>
               {
                   if (x == p1.X && y == p1.Y) return Occupation.Occupied;
                   if (x == p2.X && y == p2.Y) return Occupation.Occupied;
                   return Occupation.Empty;
               };
            form.MovementRequested = dir =>
            {
                switch (dir)
                {
                    case Lib.Model.Directions.E:
                        p1 = new Point(p1.X + 1, p1.Y);
                        p2 = new Point(p2.X + 1, p2.Y);
                        break;
                    case Lib.Model.Directions.W:
                        p1 = new Point(p1.X - 1, p1.Y);
                        p2 = new Point(p2.X - 1, p2.Y);
                        break;
                    case Lib.Model.Directions.SE:
                        p1 = new Point(p1.X - 1, p1.Y+1);
                        p2 = new Point(p2.X - 1, p2.Y+1);
                        break;
                    case Lib.Model.Directions.SW:
                        p1 = new Point(p1.X - 1, p1.Y+1);
                        p2 = new Point(p2.X - 1, p2.Y+1);
                        break;
                }

            };
            Application.Run(form);
        }
    }
}
