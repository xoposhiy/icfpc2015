﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Lib.Models;
using Lib.Finder;
using Lib.Intelligence;

namespace ManualControl
{
    internal class TetrisForm : Form
    {
        private readonly Stack<Map> mapHistory = new Stack<Map>();
        public Map Map => mapHistory.Peek();
        private readonly Dictionary<Keys, Directions> keymap;
        public Action<Directions> MovementRequested;
        Grid grid;
        Label scores;
        Label help;
        TextBox program;
        Button play;

        Button playBot;

        ProgramController controller;

        private bool showHelp;

        public TetrisForm(Map map)
        {
            controller = new ProgramController(mapHistory);

            this.KeyPreview = true;
            this.mapHistory.Push(map);
            grid = new Grid(mapHistory);
            scores = new Label();
            help = new Label();
            help.Text = "UIOP - movement\r\nQW - rotate\r\nZ - undo\r\nL - lock";
            help.BackColor = Color.Black;
            help.Font = new Font("Arial", 10);
            help.ForeColor = Color.Yellow;
            program = new TextBox();
            play = new Button();
            playBot = new Button();

            Controls.Add(grid);
            Controls.Add(scores);
            Controls.Add(help);
            Controls.Add(program);
            Controls.Add(play);
            Controls.Add(playBot);

            scores.Size = new Size(100, 30);
            grid.Location = new Point(0, 30);
            grid.Size = grid.GetDesiredSize();
            help.Size = new Size(150, 100);
            help.Location = new Point(grid.Right, grid.Bottom - help.Height);
            ClientSize = new Size(help.Right, help.Bottom);
            program.Size = new Size(150, 200);
            program.Multiline = true;
            program.Location = new Point(grid.Right, 0);
            play.Size = new Size(program.Width, 20);
            play.Location = new Point(program.Left, program.Bottom);
            play.Text = "Play";
            playBot.Click += PlayBot_Click;
            playBot.Text = "Play bot";
            playBot.Location = new Point(play.Left, play.Bottom);
            playBot.Size = play.Size;


            grid.MovementRequested += Grid_MovementRequested;
            play.Click += (s, a) => controller.Run(program.Text);
            controller.Updated = UpdateAll;
            controller.Started += () => play.Enabled = false;
            controller.Finished += () => play.Enabled = true;


        



            keymap = new Dictionary<Keys, Directions>
            {
                [Keys.Q] = Directions.CCW,
                [Keys.W] = Directions.CW,
                [Keys.U] = Directions.W,
                [Keys.I] = Directions.SW,
                [Keys.O] = Directions.SE,
                [Keys.P] = Directions.E
            };
        
        }

        private void PlayBot_Click(object sender, EventArgs e)
        {
            var pr=new NamiraOracle().PlayGame(Map);
            //var pr = new AzuraOracle().PlayGame(Map);
            controller.TimerInterval = 1;
            if (pr != null)
                controller.Run(pr);
        }

        private void Grid_MovementRequested(UnitState obj)
        {
            var text = Finder.GetPath(Map.Filled, Map.Unit.Unit, obj);
            controller.Run(text);
        }
        

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            Text = $"ProblemId: {Map.Id} - W: {Map.Width}, H: {Map.Height}. Press 'H' for help!";
            DoubleBuffered = true;

        }

        void UpdateAll()
        {
            Invalidate();
            grid.Invalidate();
            scores.Text = Map.Scores.TotalScores.ToString();
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (program.Focused) return;
            if (controller.Running) return;
           if (keymap.ContainsKey(e.KeyData) && MovementRequested != null)
                mapHistory.Push(Map.Move(keymap[e.KeyData]));
            if (e.KeyData == Keys.Z && mapHistory.Count > 1)
                mapHistory.Pop();
            if (e.KeyData == Keys.L && Map.Unit != null)
                mapHistory.Push(Map.LockUnit());
            if (e.KeyData == Keys.H)
                showHelp = !showHelp;
            if (e.KeyData == Keys.Escape)
                showHelp = false;
            UpdateAll();
        }

        
       
    }
}