using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lib.Model
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
            var gx = mapX * Width + Width / 2;
            if (mapY % 2 != 0) gx += Width / 2;
            var gy = mapY * YOffset + Height / 2;
            return new PointF((float)gx, (float)(gy));
        }

        public static Point GetMapLocation(double x, double y)
        {
            var mapY = (int)(Math.Round((y - Height / 2) / YOffset));
            if (mapY % 2 != 0) x -= Width / 2;
            var mapX = (int)(Math.Round((x - Width / 2) / Width));
            return new Point { X = mapX, Y = mapY };
        }

        public Point RotateMapLocationCW60AroundZero(Point point)
        {
            var k = point.X - point.Y / 2;
            var l = point.Y;
            if (l > 0)
                return new Point((k - l) / 2, k + l);
            else
                return new Point(k / 2 - l, k + l );
        }

        public Point RotateMapLocationCCW60AroundZero(Point point,)
        {
            var k = point.X - point.Y / 2;
            var l = point.Y;
            return new Point(k/2+l,-k);
        }


    }
}
