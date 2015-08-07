using System.Drawing;

namespace Lib.Models
{
    public static class PointExtensions
    {
        public static Point Add(this Point left, Point right)
        {
            return new Point(left.X + right.X, left.Y + right.Y);
        }

        public static Point Sub(this Point left, Point right)
        {
            return new Point(left.X - right.X, left.Y - right.Y);
        }

        public static Point Rotate(this Point p)
        {
            return Geometry.RotateMapLocationCCW60AroundZero(p);
        }

        public static Point Rotate(this Point point, int angle)
        {
            for (var i = 0; i < angle; i++)
                point = point.Rotate();
            return point;
        }

        public static Point Rotate(this Point point, Point center, double angle)
        {
            var geometryLocation = Geometry.GetGeometricLocation(point.X, point.Y);
            var geometryCenter = Geometry.GetGeometricLocation(center.X, center.Y);
            var rotated = geometryLocation.Rotate(geometryCenter, angle);
            return Geometry.GetMapLocation(rotated.X, rotated.Y);
        }
    }
}