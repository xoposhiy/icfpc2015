using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Lib.Finder;
using Lib.Models;

namespace ManualControl
{
    public partial class Grid : UserControl
    {
        public int Radius = 20;
        public Map Map => mapHistory.Peek();
        private readonly Dictionary<Occupation, Pen> penTypes;
        private readonly Dictionary<Occupation, Brush> brushTypes;
        private readonly Stack<Map> mapHistory;
        public int LabelXOffset;
        public int LabetYOffset;

        PositionedUnit mousePositionedUnit;

        public Size GetDesiredSize()
        {
            return new Size(
            (int)(Geometry.Width * Radius * (Map.Width + 1)),
            (int)(Geometry.YOffset * Radius * Map.Height + Geometry.Height * Radius));
        }

        public Grid(Stack<Map> mapHistory)
        {
            this.mapHistory = mapHistory;
            penTypes = new Dictionary<Occupation, Pen>
            {
                [Occupation.Empty] = Pens.Black,
                [Occupation.Occupied] = Pens.Red,
                [Occupation.Unit] = Pens.LawnGreen,
            };
            brushTypes = new Dictionary<Occupation, Brush>
            {
                [Occupation.Empty] = Brushes.White,
                [Occupation.Occupied] = Brushes.Gray,
                [Occupation.Unit] = Brushes.LawnGreen,
            };
            DoubleBuffered = true;
            mousePositionedUnit = Map.Unit;
        }

        public void DrawHexagonInGraphicCoordinates(Graphics g, int gx, int gy, int kx, int ky, Pen pen, Brush brush, bool marked)
        {
            var w = (int)(Radius * Geometry.Width / 2);
            var h = (int)(Radius * Geometry.Height / 2);

            var points = new[]
            {
                new Point(gx + w, gy + Radius / 2),
                new Point(gx + w, gy - Radius / 2),
                new Point(gx, gy - h),
                new Point(gx - w, gy - Radius / 2),
                new Point(gx - w, gy + Radius / 2),
                new Point(gx, gy + h),
                new Point(gx + w, gy + Radius / 2)
            };
            g.FillPolygon(brush, points);
            if (marked)
            {
                var f = 0.7f * w;
                g.FillEllipse(Brushes.LightSkyBlue, gx - f, gy - f, 2 * f, 2 * f);
            }
            g.DrawLines(pen, points);
            g.DrawString($"{kx - LabelXOffset},{ky - LabetYOffset}",
                         new Font("Arial", Radius / 2.5f),
                         Brushes.Black,
                         new Rectangle(gx - 100, gy - 100, 200, 200),
                         new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center }
                );
        }

        public void DrawHexagon(Graphics g, int x, int y, Pen pen, Brush brush, bool marked)
        {
            var p = Geometry.GetGeometricLocation(x, y);
            DrawHexagonInGraphicCoordinates(g, (int)(Radius * p.X+Radius*Geometry.Width/2), (int)(Radius * p.Y + Radius*Geometry.Height/2), x, y, pen, brush, marked);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            g.Clear(Map.IsOver ? Color.LightCoral : Color.White);

            g.DrawString(
                Map.Scores.TotalScores.ToString(),
                new Font("Arial", 15),
                Brushes.Black,
                new Point(0, 0)
                );

            for (var x = 0; x < Map.Width; x++)
            {
                for (var y = 0; y < Map.Height; y++)
                {
                    var p = new Point(x, y);
                    var occupation = Map.Filled[x, y] ? Occupation.Occupied
                        : Map.Unit.Members.Any(m => m.Equals(p)) ? Occupation.Unit
                        : Occupation.Empty;
                    DrawHexagon(g, x, y, penTypes[occupation], brushTypes[occupation], p.Equals(Map.Unit.PivotLocation));
                }
            }

            if (mousePositionedUnit!= null)
            foreach (var member in mousePositionedUnit.Members)
            {
                DrawHexagon(e.Graphics, member.X, member.Y, pen, brush, false);
            }
        }

        private Point GetLocation(float pixelX, float pixelY)
        {
            var geometryPoint = new PointF()
            {
                X = (pixelX) / Radius,
                Y = (pixelY) / Radius
            };
            return Geometry.GetMapLocation(geometryPoint.X, geometryPoint.Y);
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            mousePositionedUnit = mousePositionedUnit.Move(Directions.CW);
            this.Invalidate();
        }

        Pen pen = new Pen(Color.Aqua);
        SolidBrush brush = new SolidBrush(Color.Azure);

        protected override void OnMouseMove(MouseEventArgs e)
        {
            var point = GetLocation(e.X - Radius * (float)Geometry.Width / 2, e.Y - Radius * (float)Geometry.Height / 2);
            mousePositionedUnit = new PositionedUnit(Map.Unit.Unit, mousePositionedUnit.RotationIndex, point);
            
            this.Invalidate();
        }

        public event Action<UnitState> MovementRequested;

        protected override void OnDoubleClick(EventArgs e)
        {
            if (mousePositionedUnit != null)
                MovementRequested(new UnitState { angle = mousePositionedUnit.RotationIndex, position = mousePositionedUnit.PivotLocation });
        }
    }
}
