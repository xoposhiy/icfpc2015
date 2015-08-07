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

		public static void Main()
		{
			var map = new MapBuilder().BuildFrom(Problems.LoadProblems()[3], 0);
			var form = new TetrisForm(map.Width, map.Height);
			form.GetMap = (x, y) =>
			{
				if (x.InRange(0, map.Width - 1) && y.InRange(0, map.Height - 1))
				{
                    if (map.Filled[x, y]) return Occupation.Occupied;
                    var shiftedPoint = new Point(x - map.PivotPointLocation.X, y - map.PivotPointLocation.Y);
                    if (map.Unit.Members.Contains(shiftedPoint)) return Occupation.Unit;
                    return Occupation.Empty;
				}
				else return Occupation.Occupied;

			};
			form.MovementRequested = dir =>
			{
                var p = map.PivotPointLocation;
				switch (dir)
				{
					case Directions.E:
                        map.PivotPointLocation = new Point(p.X + 1, p.Y);
                        break;

					case Directions.W:
                        map.PivotPointLocation = new Point(p.X - 1, p.Y);
                        break;

                    case Directions.SE:
                        map.PivotPointLocation = new Point(p.X + (p.Y%2!=0?1:0), p.Y + 1);
						break;
					case Directions.SW:
                        map.PivotPointLocation = new Point(p.X - (p.Y % 2 == 0 ? 1 : 0), p.Y + 1);
                        break;
				}
			};
			Application.Run(form);
		}
	}
}