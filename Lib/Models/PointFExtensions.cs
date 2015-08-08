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

        public static PointF Sub(this PointF a, PointF b)
        {
            return new PointF(a.X - b.X, a.Y - b.Y);
        }

        public static PointF Add(this PointF a, PointF b)
        {
            return new PointF(a.X + b.X, a.Y + b.Y);
        }

        public static Point ToMap(this PointF a)
        {
            return Geometry.GetMapLocation(a.X, a.Y);
        }

    }
}