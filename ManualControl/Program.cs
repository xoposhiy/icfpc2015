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
					return map.Filled[x, y] ? Occupation.Occupied : map.Unit.Members.Contains(new Point(x, y)) ? Occupation.Unit : Occupation.Empty;
				}
				else return Occupation.Occupied;

			};
			form.MovementRequested = dir =>
			{
				switch (dir)
				{
					case Directions.E:
						p1 = new Point(p1.X + 1, p1.Y);
						p2 = new Point(p2.X + 1, p2.Y);
						break;
					case Directions.W:
						p1 = new Point(p1.X - 1, p1.Y);
						p2 = new Point(p2.X - 1, p2.Y);
						break;
					case Directions.SE:
						p1 = new Point(p1.X - 1, p1.Y + 1);
						p2 = new Point(p2.X - 1, p2.Y + 1);
						break;
					case Directions.SW:
						p1 = new Point(p1.X - 1, p1.Y + 1);
						p2 = new Point(p2.X - 1, p2.Y + 1);
						break;
				}
			};
			Application.Run(form);
		}
	}
}