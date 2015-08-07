using System;
using System.Drawing;
using Lib.Models;

namespace Lib.Finder
{
    public class FinderUnit
    {
        private Point[] points;
        public int period { get; set; }

        public FinderUnit(Point[] points, Point pivot)
        {
            this.points = points;
            for (int i = 0; i < this.points.Length; i++)
                this.points[i] = this.points[i].Sub(pivot);
            period = GetPeriod();
        }

        private FinderUnit(Point[] points, Point pivot, int period)
        {
            this.points = points;
            for (int i = 0; i < this.points.Length; i++)
                this.points[i] = this.points[i].Sub(pivot);
            this.period = period;
        }

        private int GetPeriod()
        {
            var rotated = new Point[points.Length];
            for (int i = 0; i < points.Length; i++)
                rotated[i] = points[i];

            for (int i = 1; ; i++)
            {
                for (int j = 0; j < points.Length; j++)
                    rotated[j] = rotated[j].Rotate();
                bool ok = true;
                for (int k = 0; k < points.Length && ok; k++)
                    if (points[k].X != rotated[k].X || points[k].Y != rotated[k].Y)
                        ok = false;
                if (ok)
                    return i;
            }
        }

        public Point[] FixAt(State state)
        {
            var result = new Point[points.Length];
            for (int i = 0; i < points.Length; i++)
                result[i] = state.position.Add(points[i].Rotate(state.angle));
            return result;
        }

        public Point GetStartPosition(int width)
        {
            int minX = int.MaxValue, maxX = int.MinValue;
            int minY = int.MaxValue;
            for (int i = 0; i < points.Length; i++)
            {
                minX = Math.Min(minX, points[i].X);
                maxX = Math.Max(maxX, points[i].X);
                minY = Math.Min(minY, points[i].Y);
            }
            int prefixX = ((width - maxX) + minX) / 2;
            return new Point(prefixX, -minY);
        }
    }
}