﻿using System.Drawing;

namespace Lib.Models
{
	public static class CommonExtensions
	{
		public static bool InRange(this int x, int low, int high)
		{
			return low <= x && x <= high;
		}
	}
	
	public static class PointExtensions
	{
		public static Point Rotate(this Point point, Point center, double angle)
		{
            var geometryLocation = Geometry.GetGeometricLocation(point.X, point.Y);
            var geometryCenter = Geometry.GetGeometricLocation(center.X, center.Y);
            var rotated = geometryLocation.Rotate(geometryCenter, angle);
			return Geometry.GetMapLocation(rotated.X, rotated.Y);
		}
	}
}