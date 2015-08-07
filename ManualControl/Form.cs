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
        public int Size = 50;
        Dictionary<Keys, Directions> keymap;
        readonly int MapWidth;
        readonly int MapHeight;
        int XMargin = 30;
        int YMargin = 30;
        Dictionary<Occupation,Pen> penTypes;
        Dictionary<Occupation,Brush> brushTypes;

        public TetrisForm(int mapWidth, int mapHeight)
        {
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
                (int)(Geometry.Width*Size*(mapWidth+1)),
                (int)(Geometry.YOffset*Size*mapHeight+Geometry.Height* Size));

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

        public void DrawHexagonInGraphicCoordinates(Graphics g, int gx, int gy, int kx, int ky, Pen pen, Brush brush)
        {
            var w = (int)(Size * Geometry.Width / 2);
            var h = (int)(Size * Geometry.Height / 2);
            
            var points = new[]
            {
                new Point(gx+w,gy+Size/2),
                new Point(gx+w,gy-Size/2),
                new Point(gx,gy-h),
               new Point(gx-w,gy-Size/2),
                new Point(gx-w,gy+Size/2),
                new Point(gx,gy+h),
                new Point(gx+w,gy+Size/2),
                               
            };
            g.FillPolygon(brush, points);
            g.DrawLines(pen, points);
            g.DrawString(kx.ToString() + "," + ky.ToString(),
                new Font("Arial", 14),
                Brushes.Black,
                new Rectangle(gx - 100, gy - 100, 200, 200),
                new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center }
                );
        }

        public void DrawHexagon(Graphics g, int x, int y, Pen pen, Brush brush)
        {
            var p = Geometry.GetGeometricLocation(x, y);
            DrawHexagonInGraphicCoordinates(g, (int)(Size *p.X )+ XMargin, (int)(Size *p.Y)+ YMargin, x,y ,pen, brush);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.Clear(Color.White);
             for (int x=0;x< MapWidth;x++)
                for (int y=0;y< MapHeight;y++)
                {
                    var occupation = GetMap(x, y);
                    DrawHexagon(e.Graphics, x, y, penTypes[occupation], brushTypes[occupation]);
                }
        }
    }

   
}
