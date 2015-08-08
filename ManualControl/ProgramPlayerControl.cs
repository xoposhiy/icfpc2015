using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ManualControl
{
    class ProgramPlayerControl : UserControl
    {
        ProgramPlayer player;

        TextBox program;
        Button start,play, pause, step, back, stop;
        Button[] buttons;

        public ProgramPlayerControl(ProgramPlayer player)
        {
            this.player = player;
            program = new TextBox();
            Controls.Add(program);

            program.Multiline = true;
            start = new Button();
            play = new Button();
            pause = new Button();
            step = new Button();
            back = new Button();
            stop = new Button();
            buttons = new[] { start, play, pause, step, back, stop };
            foreach (var e in buttons)
                Controls.Add(e);

            var names = new[] {"Start", "▶", "▌▌", "▶▌", "▌◀", "◼" };
            for (int i = 0; i < buttons.Length; i++)
                buttons[i].Text = names[i];

            player.AutoplayUpdated = z=> EnabledChanged();
            player.LoadedUpdated += z => EnabledChanged();
            player.LoadedUpdated += z => { if (z) program.Text = player.Program; };

            play.Click += (s, a) => player.Play();
            pause.Click += (s, a) => player.Pause();
            step.Click += (s, a) => player.Step();
            back.Click += (s, a) => player.Back();
            stop.Click += (s, a) => player.Stop();
            start.Click += (s, a) =>
              {
                  player.InitializeProgram(program.Text);
              };
        }

        void EnabledChanged()
        {
            start.Enabled = !player.Loaded;
            play.Enabled = !player.Autoplay && player.Loaded;
            pause.Enabled = player.Autoplay && player.Loaded;
            step.Enabled = !player.Autoplay && player.Loaded;
            back.Enabled = !player.Autoplay && player.Loaded;
            stop.Enabled = player.Loaded;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            OnSizeChanged(e);
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            program.Location = new Point(0, 0);
            program.Size = new Size(Size.Width, Size.Height - 30);
            int dw = Size.Width / buttons.Length;
            for (int i = 0; i < buttons.Length; i++)
            {
                buttons[i].Location = new Point(dw * i, program.Bottom);
                buttons[i].Size = new Size(dw, 30);
            }
        }

        public bool KeyboardOccupied
        {
            get
            {
                return program.Focused;
            }

        }
    }
}
