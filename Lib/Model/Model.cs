using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lib.Model
{
    enum Directions
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

    struct Point
    {
        public int X { get; set; }
        public int Y { get; set; }

        public Point(int x, int y)
        {
            X = x;
            Y = y;
        }        
    }

    struct PointF
    {
        public double X { get; set; }
        public double Y { get; set; }

        public PointF(double x, double y)
        {
            X = x;
            Y = y;
        }

        public PointF Rotate(PointF center, double angle)
        {
            var vector = new Vector(center, this).Rotate(angle);
            return new PointF(vector.X + center.X, vector.Y + center.Y);
        }
    }

    struct Vector
    {
        public double X { get; set; }
        public double Y { get; set; }

        
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
                X = X * Math.Cos(angle) - Y * Math.Sin(angle),
                Y = Y * Math.Cos(angle) + X * Math.Sin(angle)
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
