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
        MainModel mapHistory;

        HistoryControl program;
        Button play, pause, step, back;
        Button[] buttons;
        Timer timer;

        public ProgramPlayerControl(MainModel mapHistory)
        {
            this.mapHistory = mapHistory;
            program = new HistoryControl(mapHistory.History);
            Controls.Add(program);
            timer = new Timer();
            timer.Interval = 10;
            timer.Tick += Timer_Tick;

            play = new Button();
            pause = new Button();
            step = new Button();
            back = new Button();

            Controls.Add(program);

            buttons = new[] {  play, pause, step, back};
            foreach (var e in buttons)
                Controls.Add(e);

            var names = new[] {"▶", "▌▌", "▶▌", "▌◀" };
            for (int i = 0; i < buttons.Length; i++)
                buttons[i].Text = names[i];


            mapHistory.PlayingChanged += EnabledChanged;
            play.Click += (s, a) => { timer.Start(); mapHistory.Playing = true; };
            pause.Click += (s, a) => Pause();
            step.Click += (s, a) => { mapHistory.History.Forward(); };
            back.Click += (s, a) =>
            {
                mapHistory.History.Backward();
            };
            EnabledChanged();
            
        }

        void Pause()
        {
            timer.Stop(); mapHistory.Playing = false;
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            mapHistory.History.Forward();
            if (mapHistory.History.Ended) Pause();
        }

        void EnabledChanged()
        {
            play.Enabled = !mapHistory.Playing;
            step.Enabled = !mapHistory.Playing;
            back.Enabled = !mapHistory.Playing;
            pause.Enabled = mapHistory.Playing;
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
