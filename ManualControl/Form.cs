using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Lib.Models;

namespace ManualControl
{
    internal class TetrisForm : Form
    {
        private readonly Stack<Map> mapHistory = new Stack<Map>();
        public int Radius = 20;
        private readonly Dictionary<Keys, Directions> keymap;
        private readonly int MapWidth;
        private readonly int MapHeight;
        private readonly int XMargin = 30;
        private readonly int YMargin = 60;
        private readonly Dictionary<Occupation, Pen> penTypes;
        private readonly Dictionary<Occupation, Brush> brushTypes;
        public Action<Directions> MovementRequested;

        public Map Map => mapHistory.Peek();

        public int LabelXOffset;
        public int LabetYOffset;
        private bool showHelp;

        public TetrisForm(Map map)
        {
            this.mapHistory.Push(map);
            keymap = new Dictionary<Keys, Directions>
            {
                [Keys.Q] = Directions.CCW,
                [Keys.W] = Directions.CW,
                [Keys.U] = Directions.W,
                [Keys.I] = Directions.SW,
                [Keys.O] = Directions.SE,
                [Keys.P] = Directions.E
            };
            MapWidth = map.Width;
            MapHeight = map.Height;

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

            ClientSize = new Size(
                XMargin + (int)(Geometry.Width * Radius * (MapWidth + 1)),
                YMargin + (int)(Geometry.YOffset * Radius * MapHeight + Geometry.Height * Radius));

        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            Text = $"ProblemId: {Map.Id} - W: {Map.Width}, H: {Map.Height}. Press 'H' for help!";
            DoubleBuffered = true;

        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (keymap.ContainsKey(e.KeyData) && MovementRequested != null)
                mapHistory.Push(Map.Move(keymap[e.KeyData]));
            if (e.KeyData == Keys.Z && mapHistory.Count > 1)
                mapHistory.Pop();
            if (e.KeyData == Keys.L && Map.Unit != null)
                mapHistory.Push(Map.LockUnit());
            if (e.KeyData == Keys.H)
                showHelp = !showHelp;
            if (e.KeyData == Keys.Escape)
                showHelp = false;
            Invalidate();
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
                var f = 0.7f*w;
                g.FillEllipse(Brushes.LightSkyBlue, gx -f, gy-f, 2*f, 2*f);
            }
            g.DrawLines(pen, points);
            g.DrawString($"{kx - LabelXOffset},{ky - LabetYOffset}",
                         new Font("Arial", Radius/2.5f),
                         Brushes.Black,
                         new Rectangle(gx - 100, gy - 100, 200, 200),
                         new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center }
                );
        }

        public void DrawHexagon(Graphics g, int x, int y, Pen pen, Brush brush, bool marked)
        {
            var p = Geometry.GetGeometricLocation(x, y);
            DrawHexagonInGraphicCoordinates(g, (int)(Radius * p.X) + XMargin, (int)(Radius * p.Y) + YMargin, x, y, pen, brush, marked);
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

            for (var x = 0; x < MapWidth; x++)
            {
                for (var y = 0; y < MapHeight; y++)
                {
                    var p = new Point(x, y);
                    var occupation = Map.Filled[x, y] ? Occupation.Occupied 
                        : Map.Unit.Members.Any(m => m.Equals(p)) ? Occupation.Unit 
                        : Occupation.Empty;
                    DrawHexagon(g, x, y, penTypes[occupation], brushTypes[occupation], p.Equals(Map.Unit.PivotLocation));
                }
            }
            if (showHelp)
            {
                g.FillRectangle(Brushes.Black, 0, 0, ClientSize.Width, 300);
                g.DrawString("UIOP — movement", new Font("Arial", 10), Brushes.Yellow, 10, 10);
                g.DrawString("QW — rotation", new Font("Arial", 10), Brushes.Yellow, 10, 30);
                g.DrawString("Z — undo", new Font("Arial", 10), Brushes.Yellow, 10, 50);
                g.DrawString("L — lock unit", new Font("Arial", 10), Brushes.Yellow, 10, 70);
            }
        }
    }
}