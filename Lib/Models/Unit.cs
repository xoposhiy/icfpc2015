using System;
using System.Collections.Generic;
using System.Drawing;

namespace Lib.Models
{
	public class Unit
	{
		public List<Point> Members;
		public Point Pivot;

		public Unit(List<Point> members, Point pivot)
		{
			Members = members;
			Pivot = pivot;
		}

		public bool IsSafePath(IEnumerable<Directions> path)
		{
			throw new NotImplementedException();
		}

		public void Move(Directions direction)
		{
			switch (direction)
			{
				case Directions.E:
					Pivot = new Point(Pivot.X + 1, Pivot.Y);
					return;

				case Directions.W:
					Pivot = new Point(Pivot.X - 1, Pivot.Y);
					return;

				case Directions.SE:
					Pivot = new Point(Pivot.X + (Pivot.Y % 2 != 0 ? 1 : 0), Pivot.Y + 1);
                    return;

				case Directions.SW:
					Pivot = new Point(Pivot.X - (Pivot.Y % 2 == 0 ? 1 : 0), Pivot.Y + 1);
                    return;

				case Directions.CW:

					return;
				case Directions.CCW:
					return;
			}
		}
	}
}