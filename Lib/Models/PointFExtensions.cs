using System.Drawing;

namespace Lib.Models
{
	public static class PointFExtensions
	{ 
		public static PointF Rotate(this PointF point, PointF center, double angle)
		{
			var vector = new Vector(center, point).Rotate(angle);
			return new PointF(vector.X + center.X, vector.Y + center.Y);
		}
	}
}