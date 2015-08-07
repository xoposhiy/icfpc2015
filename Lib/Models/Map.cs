using System;
using System.Drawing;

namespace Lib.Models
{
	public class MapBuilder
	{
		public Map BuildFrom(ProblemJson problem, int unitIndex)
		{
			var f = new bool[problem.width, problem.height];
			foreach (var cell in problem.filled)
				f[cell.x, cell.y] = true;
			var u = problem.units[unitIndex].ToUnit();
			return new Map(problem.height, problem.width, f, u, problem.id);
		}
	}

	public class Map
	{
		public readonly int Height;
		public readonly int Width;

		public Map(int height, int width, bool[,] filled, Unit unit, int id)
		{
			Height = height;
			Width = width;
			Filled = filled;
			Unit = unit;
		    Id = id;
		}

	    public int Id { get; private set; }

	    public bool[,] Filled { get; private set; }

	    public Unit Unit { get; private set; }
        

		public bool IsSafeMovement(Directions direction)
		{
			throw new NotImplementedException();
		}
	}
}