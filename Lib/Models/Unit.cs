using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

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

        double ccv = 2 * Math.PI - Math.PI / 3;
        double cv = Math.PI / 3;

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
                    Members = Members.Select(z => z.Rotate(new Point(0, 0), cv)).ToList();
					return;
				case Directions.CCW:
                    Members = Members.Select(z => z.Rotate(new Point(0, 0), ccv)).ToList();
                    return;
			}
		}
	}
}