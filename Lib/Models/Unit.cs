using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Lib.Models
{
	public class Unit
	{
		public List<Point> Members;
        public List<Point>[] Rotations;
		public Point Pivot;
        public int RotationIndex;
        public Unit(List<Point> members, Point pivot)
        {
            Members = members;
            Pivot = pivot;

            double cv = Math.PI / 3;
            Rotations = new List<Point>[6];
            Rotations[0] = members.Select(z => z.Rotate(new Point(0, 0), cv)).ToList();
            for (int i = 1; i < 6; i++)
                Rotations[i] = Rotations[i - 1].Select(z => z.Rotate(new Point(0, 0), cv)).ToList();
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
                    RotationIndex = (RotationIndex + 1) % 6;
                    return;
				case Directions.CCW:
                    RotationIndex = (RotationIndex + 5) % 6;
                    return;
			}
		}
	}
}