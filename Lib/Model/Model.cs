using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lib.Model
{
   public enum Directions
    {
        SE,
        SW,
        E,
        W,
        CW,
        CCW
    }


    class Model
    {
        public readonly int Height;
        public readonly int Width;
        public bool[,] Occupied { get; private set; }
        public Figure Figure { get; private set; }

        public bool IsSafeMovement(Directions direction)
        {
            throw new NotImplementedException();
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

    public static class PointFExtensions
    { 
        public static PointF Rotate(this PointF point, PointF center, double angle)
        {
            var vector = new Vector(center, point).Rotate(angle);
            return new PointF(vector.X + center.X, vector.Y + center.Y);
        }
    }

    class Vector
    {
        public float X { get; set; }
        public float Y { get; set; }

        
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


    class Figure
    {
        public List<Point> Occupied;
        public Point PivotRelativeLocation;
        public bool IsSafePath(IEnumerable<Directions> path)
        {
            throw new NotImplementedException();
        }

        public void Move(Directions direction)
        {
            switch (direction)
            {
                case Directions.E:
                    PivotRelativeLocation = new Point(PivotRelativeLocation.X + 1, PivotRelativeLocation.Y);
                    return;

                case Directions.W:
                    PivotRelativeLocation = new Point(PivotRelativeLocation.X - 1, PivotRelativeLocation.Y);
                    return;

                case Directions.SE:
                    PivotRelativeLocation = new Point(PivotRelativeLocation.X, PivotRelativeLocation.Y + 1);
                    return;

                case Directions.SW:
                    PivotRelativeLocation = new Point(PivotRelativeLocation.X - 1, PivotRelativeLocation.Y + 1);
                    return;

                case Directions.CW:
                                       
                    return;
                case Directions.CCW:
                    return;
            } 

        }
    }

}
