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
        string program;
        int ProgramPointer;
        public bool Running;

        public ProgramController(Stack<Map> mapHistory)
        {
            this.mapHistory = mapHistory;
            timer = new Timer();
            timer.Interval = 30;
            timer.Tick += (s, a) => Step();
        }


        void Step()
        {
            if (program == null) return;
            if (ProgramPointer >= program.Length)
            {
                timer.Stop();
                if (Finished != null) Finished();
                Running = false;
                return;
            }
            var c = program[ProgramPointer];
            var dir = Finder.CharToDirection(c);
            mapHistory.Push(mapHistory.Peek().Move(dir));
            if (Updated != null) Updated();
            ProgramPointer++;
        }

        public void Run(string program)
        {
            this.program = program;
            ProgramPointer = 0;
            Running = true;
            if (Started != null) Started();
            timer.Start();
        }


    }
}
