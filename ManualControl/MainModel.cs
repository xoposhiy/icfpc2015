using Lib.Intelligence;
using Lib.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ManualControl
{
    public class MainModel
    {
        public History History;

        public event Action PlayingChanged;

        Timer timer;

        public bool Playing { get; private set; }

        public event Action ContinuationRequest;

        public MainModel()
        {
            timer = new Timer();
            timer.Interval = 10;
            timer.Tick += (s, a) =>
            {
                History.Forward();
                if (History.Ended)
                {
                    Pause();
                    if (ContinuationRequest != null) ContinuationRequest();

                }
            };
        }

        public void Play()
        {
            timer.Start();
            Playing = true;
            if (PlayingChanged != null) PlayingChanged();
        }

        public void Pause()
        {
            timer.Stop();
            Playing = false;
            if (PlayingChanged != null) PlayingChanged();
        }

        public Solver Solver { get; set; }

        public readonly SuggestionsModel Suggestions = new SuggestionsModel();
    }
}
