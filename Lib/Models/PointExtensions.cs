using System;
using System.Collections.Generic;
using System.Drawing;

namespace Lib.Models
{
    public static class PointExtensions
    {
        public static Point Move(this Point p, Directions direction)
        {
            switch (direction)
            {
                case Directions.E:
                    return new Point(p.X + 1, p.Y);
                case Directions.W:
                    return new Point(p.X - 1, p.Y);
                case Directions.SE:
                    return new Point(p.X + (p.Y % 2 != 0 ? 1 : 0), p.Y + 1);
                case Directions.SW:
                    return new Point(p.X - (p.Y % 2 == 0 ? 1 : 0), p.Y + 1);
                default:
                    throw new Exception("Rotation?! O_o");
            }
        }

        public static Point Add(this Point left, Point right)
        {
            return new Point(left.X + right.X, left.Y + right.Y);
        }

        public static Point Sub(this Point left, Point right)
        {
            return new Point(left.X - right.X, left.Y - right.Y);
        }

        public static Point Rotate(this Point p, int angle = 1)
        {
            return p.Rotate(new Point(0, 0), angle * (Math.PI / 3));
        }

        public static Point Rotate(this Point point, Point center, double angle)
        {
            var geometryLocation = Geometry.GetGeometricLocation(point.X, point.Y);
            var geometryCenter = Geometry.GetGeometricLocation(center.X, center.Y);
            var rotated = geometryLocation.Rotate(geometryCenter, angle);
            return Geometry.GetMapLocation(rotated.X, rotated.Y);
        }

        public static PointF ToGeometry(this Point point)
        {
            return Geometry.GetGeometricLocation(point.X, point.Y);
        }

        public static IEnumerable<Point> HexaNeighbours(this Point p)
        {
            if (p.Y % 2 == 0)
            {
                yield return new Point(p.X - 1, p.Y);
                yield return new Point(p.X - 1, p.Y - 1);
                yield return new Point(p.X, p.Y - 1);
                yield return new Point(p.X + 1, p.Y);
                yield return new Point(p.X, p.Y + 1);
                yield return new Point(p.X - 1, p.Y + 1);
            }
            else
            {
                yield return new Point(p.X - 1, p.Y);
                yield return new Point(p.X, p.Y - 1);
                yield return new Point(p.X + 1, p.Y - 1);
                yield return new Point(p.X + 1, p.Y);
                yield return new Point(p.X + 1, p.Y + 1);
                yield return new Point(p.X, p.Y + 1);
            }
        }
    }
}