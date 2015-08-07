using System.Drawing;
using System.Windows.Forms;
using Lib.Models;

namespace ManualControl
{
	internal class Program
	{
		private static Point p1 = new Point(-5, 0);
		private static Point p2 = new Point(-5, 1);

		public static void Main()
		{
			var form = new TetrisForm(30, 30);
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