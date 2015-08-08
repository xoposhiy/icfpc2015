using System.Drawing;

namespace Lib.Models
{
    public class UnitPosition
    {
        public UnitPosition()
        {
        }   

        public UnitPosition(Point point, int angle)
        {
            Point = point;
            Angle = angle;
        }

        public Point Point { get; set; }
        public int Angle { get; set; }
        
        protected bool Equals(UnitPosition other)
        {
            return Point.Equals(other.Point) && Angle == other.Angle;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((UnitPosition)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (Point.GetHashCode() * 397) ^ Angle;
            }
        }
    }
}