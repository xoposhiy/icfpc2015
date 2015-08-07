using Lib.Model;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ManualControl
{
    class TetrisForm : Form
    {
        int Size = 20;
        int Width;
        int Height;
        int YOffset;
        Dictionary<Keys, Directions> keymap;
        readonly int MapWidth;
        readonly int MapHeight;
        Dictionary<Occupation,Pen> penTypes;
        Dictionary<Occupation,Brush> brushTypes;

        public TetrisForm(int mapWidth, int mapHeight)
        {
            Width = (int)((double) Size / Math.Tan(Math.PI / 6));

            var capHeight = (int)Math.Sqrt(Size * Size - Math.Pow(Width / 2, 2));

            Height = 2*capHeight + Size;
            YOffset = Size + capHeight;

            keymap = new Dictionary<Keys, Directions>();
            keymap[Keys.Q] = Directions.CCW;
            keymap[Keys.W] = Directions.CW;
            keymap[Keys.U] = Directions.W;
            keymap[Keys.I] = Directions.SW;
            keymap[Keys.O] = Directions.SE;
            keymap[Keys.P] = Directions.E;
            this.MapWidth = mapWidth;
            this.MapHeight = mapHeight;

            penTypes = new Dictionary<Occupation, Pen>();
            brushTypes = new Dictionary<Occupation, Brush>();
            penTypes[Occupation.Empty] = Pens.Black;
            penTypes[Occupation.Occupied] = Pens.Red;
            brushTypes[Occupation.Empty] = Brushes.White;
            brushTypes[Occupation.Occupied] = Brushes.Gray;

            ClientSize = new Size(
                (MapWidth + 1) * Width,
                (mapHeight) * YOffset +Height);

            DoubleBuffered = true;

        }

        public Action<Directions> MovementRequested;
        public Func<int, int, Occupation> GetMap; 

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (keymap.ContainsKey(e.KeyData) && MovementRequested != null)
                MovementRequested(keymap[e.KeyData]);
            Invalidate();
        }

        public void DrawHexagonInGraphicCoordinates(Graphics g, int gx, int gy, Pen pen, Brush brush)
        {
            var points = new[]
            {
                new Point(gx+Width/2,gy+Size/2),
                new Point(gx+Width/2,gy-Size/2),
                new Point(gx,gy-Height/2),
               new Point(gx-Width/2,gy-Size/2),
                new Point(gx-Width/2,gy+Size/2),
                new Point(gx,gy+Height/2),
                new Point(gx+Width/2,gy+Size/2),
                               
            };
            g.FillPolygon(brush, points);
            g.DrawLines(pen, points);
        }

        public void DrawHexagon(Graphics g, int x, int y, Pen pen, Brush brush)
        {
            var gx = x * Width + Width / 2;
            if (y % 2 != 0) gx += Width / 2;
            var gy =y * YOffset + Height / 2;
            DrawHexagonInGraphicCoordinates(g, gx, gy, pen, brush);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
             for (int x=0;x< MapWidth;x++)
                for (int y=0;y< MapHeight;y++)
                {
                    var occupation = GetMap(x, y);
                    DrawHexagon(e.Graphics, x, y, penTypes[occupation], brushTypes[occupation]);
                }
        }
    }

   
}
