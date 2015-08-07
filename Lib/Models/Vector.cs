using System;
using System.Drawing;

namespace Lib.Models
{
	class Vector
	{
		public float X { get; set; }
		public float Y { get; set; }

		public Vector()
		{

		}

		public Vector(PointF center, PointF point)
		{
			X = point.X - center.X;
			Y = point.Y - center.Y;
		}

		//CCW
		public Vector Rotate(double angle)
		{
			return new Vector()
			{
				X = X * (float)Math.Cos(angle) - Y * (float)Math.Sin(angle),
				Y = Y * (float)Math.Cos(angle) + X * (float)Math.Sin(angle)
			};
		}
	}
}