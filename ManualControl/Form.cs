using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Lib.Models;

namespace ManualControl
{
	internal class TetrisForm : Form
	{
		public int Size = 10;
		private readonly Dictionary<Keys, Directions> keymap;
		private readonly int MapWidth;
		private readonly int MapHeight;
		private readonly int XMargin = 30;
		private readonly int YMargin = 30;
		private readonly Dictionary<Occupation, Pen> penTypes;
		private readonly Dictionary<Occupation, Brush> brushTypes;
		public Action<Directions> MovementRequested;
		public Func<int, int, Occupation> GetMap;

       public int LabelXOffset;
        public int LabetYOffset;

		public TetrisForm(int mapWidth, int mapHeight)
		{
		    throw new InvalidOperationException("TODO");
		}

	    public TetrisForm(Map map)
	    {
            Text = "Id: " + map.Id + " - W: " + map.Width + ", H: " + map.Height;
            keymap = new Dictionary<Keys, Directions>();
            keymap[Keys.Q] = Directions.CCW;
            keymap[Keys.W] = Directions.CW;
            keymap[Keys.U] = Directions.W;
            keymap[Keys.I] = Directions.SW;
            keymap[Keys.O] = Directions.SE;
            keymap[Keys.P] = Directions.E;
            MapWidth = map.Width;
            MapHeight = map.Height;

            penTypes = new Dictionary<Occupation, Pen>();
            brushTypes = new Dictionary<Occupation, Brush>();
            penTypes[Occupation.Empty] = Pens.Black;
            penTypes[Occupation.Occupied] = Pens.Red;
            penTypes[Occupation.Unit] = Pens.LawnGreen;
            brushTypes[Occupation.Empty] = Brushes.White;
            brushTypes[Occupation.Occupied] = Brushes.Gray;
            brushTypes[Occupation.Unit] = Brushes.LawnGreen;

            ClientSize = new Size(
                XMargin + (int)(Geometry.Width * Size * (MapWidth + 1)),
                YMargin + (int)(Geometry.YOffset * Size * MapHeight + Geometry.Height * Size));

            DoubleBuffered = true;
        }

	    protected override void OnKeyDown(KeyEventArgs e)
		{
			if (keymap.ContainsKey(e.KeyData) && MovementRequested != null)
				MovementRequested(keymap[e.KeyData]);
			Invalidate();
		}

		public void DrawHexagonInGraphicCoordinates(Graphics g, int gx, int gy, int kx, int ky, Pen pen, Brush brush)
		{
			var w = (int)(Size * Geometry.Width / 2);
			var h = (int)(Size * Geometry.Height / 2);

			var points = new[]
			{
				new Point(gx + w, gy + Size / 2),
				new Point(gx + w, gy - Size / 2),
				new Point(gx, gy - h),
				new Point(gx - w, gy - Size / 2),
				new Point(gx - w, gy + Size / 2),
				new Point(gx, gy + h),
				new Point(gx + w, gy + Size / 2)
			};
			g.FillPolygon(brush, points);
			g.DrawLines(pen, points);
            var str = (kx - LabelXOffset).ToString() + "," + (ky - LabetYOffset).ToString();
            g.DrawString(str,
			             new Font("Arial", 6),
			             Brushes.Black,
			             new Rectangle(gx - 100, gy - 100, 200, 200),
			             new StringFormat {Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center}
				);
		}

		public void DrawHexagon(Graphics g, int x, int y, Pen pen, Brush brush)
		{
			var p = Geometry.GetGeometricLocation(x, y);
			DrawHexagonInGraphicCoordinates(g, (int)(Size * p.X) + XMargin, (int)(Size * p.Y) + YMargin, x, y, pen, brush);
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			e.Graphics.Clear(Color.White);
			for (var x = 0; x < MapWidth; x++)
			{
				for (var y = 0; y < MapHeight; y++)
				{
					var occupation = GetMap(x, y);
					DrawHexagon(e.Graphics, x, y, penTypes[occupation], brushTypes[occupation]);
				}
			}
		}
	}
}