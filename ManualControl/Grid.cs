﻿using System;
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
        public Map Map => mapHistory.History.CurrentMap;
        private readonly Dictionary<Occupation, Pen> penTypes;
        private readonly Dictionary<Occupation, Brush> brushTypes;
        private readonly MainModel mapHistory;
        public int LabelXOffset;
        public int LabetYOffset;



        protected override void OnSizeChanged(EventArgs e)
        {
            UpdateRadius();
        }

        public void UpdateRadius()
        {
            Radius = (int)Math.Min(
                Width / (Map.Width * Geometry.Width),
                Height / (Map.Height * Geometry.YOffset));
        }

        public Grid(MainModel mapHistory)
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
            requestedLocation = null;
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
            //g.DrawString($"{kx - LabelXOffset},{ky - LabetYOffset}",
            //             new Font("Arial", Radius / 2.5f),
            //             Brushes.Black,
            //             new Rectangle(gx - 100, gy - 100, 200, 200),
            //             new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center }
            //    );
        }

        public void DrawHexagon(Graphics g, int x, int y, Pen pen, Brush brush, bool marked)
        {
            var p = Geometry.GetGeometricLocation(x, y);
            DrawHexagonInGraphicCoordinates(g, (int)(Radius * p.X + Radius * Geometry.Width / 2), (int)(Radius * p.Y + Radius * Geometry.Height / 2), x, y, pen, brush, marked);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            g.Clear(Map.IsOver ? (Map.Died ? Color.LightCoral : Color.Gold) : Color.White);

            for (var x = 0; x < Map.Width; x++)
            {
                for (var y = 0; y < Map.Height; y++)
                {
                    var p = new Point(x, y);
                    var occupation = Map.Filled[x, y] ? Occupation.Occupied
                        : Map.Unit.Members.Any(m => m.Equals(p)) ? Occupation.Unit
                        : Occupation.Empty;
                    DrawHexagon(g, x, y, penTypes[occupation], brushTypes[occupation], p.Equals(Map.Unit.Position.Point));
                }
            }

            if (requestedLocation != null &&  !mapHistory.Playing)
            {
                IEnumerable<Directions> path = null;
                path = mapHistory.Solver.Finder.GetPath(
                    Map,
                    requestedLocation.Position);
                bool exist = path != null;
                foreach (var member in requestedLocation.Members)
                {
                    DrawHexagon(e.Graphics, member.X, member.Y, Pens.Black, exist ? Brushes.Aqua : Brushes.MistyRose, false);
                }
            }


            foreach (var v in mapHistory.Suggestions.OraclePresense)
                DrawHexagon(e.Graphics, v.X, v.Y, Pens.Black, Brushes.Lime, false);


            if (mapHistory.Suggestions != null)
            {
                var s = mapHistory.Suggestions.GetCurrentSuggestion();
                if (s!= null)
                {
                    var sugg = new PositionedUnit(mapHistory.Suggestions.Unit, s.Position);
                    foreach(var member in sugg.Members)
                    {
                        DrawHexagon(e.Graphics, member.X, member.Y, Pens.Black, Brushes.Yellow, false);
                    }
                }
            }

        }


        PositionedUnit requestedLocation;
        bool requestedLocationIsReachable;
        int requestedAngle;

        void SetRequestedLocation(Point location, int angle)
        {
            if (mapHistory.Playing) return;

            if (requestedLocation != null
                && requestedLocation.Position.Point == location
                && requestedLocation.Position.Angle == angle)
                return;

            requestedLocation = new PositionedUnit(Map.Unit.Unit, new UnitPosition(location, angle));
            IEnumerable<Directions> path = null;
            path = mapHistory.Solver.Finder.GetPath(
                Map,
                requestedLocation.Position);
            requestedLocationIsReachable = path != null;
            Invalidate();
        }



        private Point GetLocation(MouseEventArgs e)
        {
            float pixelX = e.X - Radius * (float)Geometry.Width / 2;
            float pixelY = e.Y - Radius * (float)Geometry.Height / 2;
            var geometryPoint = new PointF()
            {
                X = (pixelX) / Radius,
                Y = (pixelY) / Radius
            };
            return Geometry.GetMapLocation(geometryPoint.X, geometryPoint.Y);
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            if (mapHistory.Playing) return;

            requestedAngle = e.Delta < 0 ? requestedAngle + 1 : requestedAngle - 1;
            requestedAngle = requestedAngle % Map.Unit.Unit.Period;
            SetRequestedLocation(GetLocation(e), requestedAngle);
        }


        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (mapHistory.Playing) return;

            SetRequestedLocation(GetLocation(e), requestedAngle);
        }

        public event Action<UnitPosition> MovementRequested;

        protected override void OnDoubleClick(EventArgs e)
        {
            if (mapHistory.Playing) return;

            if (requestedLocation != null)
                MovementRequested(requestedLocation.Position);
        }
    }

   
}
