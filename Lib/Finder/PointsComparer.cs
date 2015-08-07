using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lib.Finder
{
    class PointsComparer : Comparer<Point>
    {
        public override int Compare(Point left, Point right)
        {
            if (left.X == right.X)
                return left.Y - right.Y;
            return left.X - right.X;
        }
    }
}
