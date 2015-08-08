using System;
using System.Collections.Generic;
using System.Drawing;

namespace Lib.Models
{
    internal class PointsComparer : Comparer<PointF>
    {
        public override int Compare(PointF left, PointF right)
        {
            if (Math.Abs(left.X - right.X) < Geometry.Eps)
                return Math.Sign(left.Y - right.Y);
            return Math.Sign(left.X - right.X);
        }
    }
}