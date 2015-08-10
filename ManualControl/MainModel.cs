using System;
using System.Windows.Forms;
using Lib.ArenaImpl;
using Lib.Intelligence;

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
                for (int i = 0; i < FastForwardSteps; i++)
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

        public SuggestionsModel Suggestions = new SuggestionsModel();

        public int FastForwardSteps = 1;
    }
}
