using System.Drawing;

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
			var resultF = Geometry.GetGeometricLocation(point.X, point.Y).Rotate(Geometry.GetGeometricLocation(center.X, center.Y), angle);
			return Geometry.GetMapLocation(resultF.X, resultF.Y);
		}
	}
}