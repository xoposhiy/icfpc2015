using Lib.Finder;
using Lib.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ManualControl
{
    class ProgramPlayer
    {
        public string Program { get; private set; }
        public int ProgramPointer { get; private set; }
        Stack<Map> mapHistory;
        public Action MapUpdated;
        public Action<bool> LoadedUpdated;
        public Action<bool> AutoplayUpdated;

        public bool Loaded { get; private set; }
        public bool Autoplay { get; private set; }
        public int TimerInterval { get { return timer.Interval; } set { timer.Interval = value; } }

        Timer timer;

        public ProgramPlayer(Stack<Map> mapHistory)
        {
            this.mapHistory = mapHistory;
            timer = new Timer();
            timer.Interval = 10;
            timer.Tick += (s,a)=>Step();
        }


        public void InitializeProgram(string program)
        {
            if (Loaded) throw new Exception("Can't init program when loading");
            Program = program;
            ProgramPointer = 0;
            Loaded = true;
            if (LoadedUpdated!=null) LoadedUpdated(true);
        }

        bool stopWhenFinished;

        public void Play(bool stopWhenFinished=false)
        {
            if (!Loaded) throw new Exception("No program to run");
            timer.Start();
            Autoplay = true;
            this.stopWhenFinished = stopWhenFinished;
            if (AutoplayUpdated != null) AutoplayUpdated(true);
        }

        public void Pause()
        {
            if (!Loaded) throw new Exception("No program to run");
            timer.Stop();
            Autoplay = false;
            if (AutoplayUpdated != null) AutoplayUpdated(false);
        }

        public void Stop()
        {
            Pause();
            Program = null;
            Loaded = false;
            if (LoadedUpdated!=null) LoadedUpdated(false);
        }

        public void Step()
        {
            if (!Loaded) throw new Exception("No program to run");
            if (ProgramPointer >= Program.Length || mapHistory.Peek().IsOver)
            {
                Pause();
                if (stopWhenFinished)
                    Stop();
                return;
            }
            var c = Program[ProgramPointer];
            var dir = c.ToDirection();
            if (!mapHistory.Peek().IsOver)
                mapHistory.Push(mapHistory.Peek().Move(dir));
            
            ProgramPointer++;

            if (MapUpdated != null) MapUpdated(); 
        }

        public void Back()
        {
            if (ProgramPointer>0)
            {
                ProgramPointer--;
                mapHistory.Pop();
                if (MapUpdated != null) MapUpdated();
            }
        }
    }
}
