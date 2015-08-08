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
        public Action Executed;
        Stack<Map> mapHistory;
        string program;
        int ProgramPointer;

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
                timer.Start();
                if (Executed != null) Executed();
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
            timer.Start();
        }


    }
}
