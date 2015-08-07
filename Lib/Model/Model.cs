using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lib.Model
{
   public enum Directions
    {
        SE,
        SW,
        E,
        W,
        CW,
        CCW
    }


	class Model
	{
		public readonly int Height;
		public readonly int Width;
		public bool[,] Occupied { get; private set; }
		public Figure Figure { get; private set; }

        public bool IsSafeMovement(Directions direction)
        {
            throw new NotImplementedException();
        }

    }

    struct Point
    {
        public int X { get; set; }
        public int Y { get; set; }
    }


    class Figure
    {
        public bool[,] Occupied;
        public Point PivotRelativeLocation;
        public bool IsSafePath(IEnumerable<Directions> path)
        {
            throw new NotImplementedException();
        }
    }

}
