using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ManualControl
{
    class HistoryControl : UserControl
    {
        History history;


        public HistoryControl(History history)
        {
            this.history = history;
            history.Updated += () => Invalidate();
            DoubleBuffered = true;
        }

        protected override void OnPaint(PaintEventArgs __e)
        {


            var g = __e.Graphics;
            g.Clear(Color.White);
            int x = 0;
            int y = -1;
            var kx = 13;
            var margin = 50;
            var ky = 13;
            string token = null;
            var font = new Font("Consolas", 10);
            var maxx = (Width- margin)/kx;

            int ptr = 0;
            foreach(var e in history.Items.Skip(1))
            {
                ptr++;
                if (e.Token != token)
                {
                    y++;
                    x = -1;
                    token = e.Token;
                    g.DrawString(token, font, Brushes.Black, new Point(0, y * kx));
                }

                x++;
                if (x>= maxx) { x = 0;y++; }

                var color = Brushes.Black;
                if (e.Map.IsOver) color = Brushes.Red;
                if (ptr == history.CurrentPosition)
                    color = Brushes.Cyan;

                g.DrawString(
                    e.Char.ToString(),
                    font,
                    color,
                    new Point(margin+ kx * x, ky * y));
                
            }
        }
    }
}
