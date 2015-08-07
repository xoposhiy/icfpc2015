using System;
using System.Drawing;

namespace Lib.Models
{
    public static class Geometry
    {
        public static readonly double Width;
        public static readonly double Height;
        public static readonly double YOffset;

        static Geometry()
        {
            Width = 1.0 / Math.Tan(Math.PI / 6);
            var capHeight = Math.Sqrt(1.0 - Math.Pow(Width / 2, 2));
            Height = 2 * capHeight + 1;
            YOffset = 1.0 + capHeight;
        }

        public static PointF GetGeometricLocation(int mapX, int mapY)
        {
            var gx = mapX * Width;
            if (mapY % 2 != 0) gx += Width / 2;
            var gy = mapY * YOffset;
            return new PointF((float)gx, (float)(gy));
        }

        public static Point GetMapLocation(double x, double y)
        {
            var mapY = (int)(Math.Round(y / YOffset));
            if (mapY % 2 != 0) x -= Width / 2;
            var mapX = (int)(Math.Round(x / Width));
            return new Point {X = mapX, Y = mapY};
        }

        public static int floor2(int x)
        {
            if (x >= 0) return x / 2;
            return (x - 1) / 2;
        }

        public static Point RotateMapLocationCW60AroundZero(Point point)
        {
            var k = point.X - floor2(point.Y);
            var l = point.Y;
            if (l > 0)
                return new Point((k - l) / 2, k + l);
            return new Point(k / 2 - l, k + l);
        }

        public static Point RotateMapLocationCCW60AroundZero(Point point)
        {
            var k = point.X - floor2(point.Y);
            var l = point.Y;
            return new Point(k / 2 + l, -k);
        }
    }
}