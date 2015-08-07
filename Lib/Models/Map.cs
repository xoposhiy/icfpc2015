using System;

namespace Lib.Model
{
	public class Map
	{
		public Map(int height, int width, bool[,] filled, Unit unit)
		{
			Height = height;
			Width = width;
			Filled = filled;
			Unit = unit;
		}

		public readonly int Height;
		public readonly int Width;

		public bool[,] Filled { get; private set; }

		public Unit Unit { get; private set; }

		public bool IsSafeMovement(Directions direction)
		{
			throw new NotImplementedException();
		}
	}
}