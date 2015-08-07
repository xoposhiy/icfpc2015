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
        public int RotationIndex;
        public Unit(List<Point> members, Point pivot)
        {
            Members = members.Select(c => c.Sub(pivot)).ToList();

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

	}
}