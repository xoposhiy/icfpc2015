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
    class ProgramController
    {
        Timer timer = new Timer();
        public Action Updated;
        public Action Finished;
        public Action Started;
        Stack<Map> mapHistory;
        public int TimerInterval {  get { return timer.Interval; } set { timer.Interval = value; } }
        public string Program { get; private set; }
        public int ProgramPointer { get; private set; }
        public bool Running;

        public ProgramController(Stack<Map> mapHistory)
        {
            this.mapHistory = mapHistory;
            timer = new Timer();
            timer.Interval = 30;
            timer.Tick += (s, a) => Step();
        }


        public void Back()
        {
            if (ProgramPointer>0)
            {
                ProgramPointer--;
                mapHistory.Pop();
                Updated();
            }
        }


        public void Step()
        {
            if (Program == null) return;
            if (ProgramPointer >= Program.Length)
            {
                timer.Stop();
                if (Finished != null) Finished();
                Running = false;
                Program = null;
                return;
            }
            var c = Program[ProgramPointer];
            var dir = c.ToDirection();
            mapHistory.Push(mapHistory.Peek().Move(dir));
            if (Updated != null) Updated();
            ProgramPointer++;
        }

        public void Run(string program, bool step)
        {
            this.Program = program;
            ProgramPointer = 0;
            Running = true;
            if (Started != null) Started();
            if (!step) timer.Start();
        }
        


    }
}
