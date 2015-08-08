using System;
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
        private readonly MainModel mapHistory = new MainModel();
        public Map Map => mapHistory.History.CurrentMap;
        private readonly Dictionary<Keys, Directions> keymap;
        public Action<Directions> MovementRequested;
        Grid grid;
        Label scores;
        Label help;

        ProgramPlayerControl playerControl;
        Button suggest,runBotIteration, runBotGame;
       
        
        private bool showHelp;

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            grid.Size = grid.GetDesiredSize();
            scores.Size = new Size(100, 30);
            grid.Location = new Point(0, 30);

            help.Size = new Size(ClientSize.Width-grid.Width, 100);
            help.Location = new Point(grid.Right, ClientSize.Height- help.Height);

            playerControl.Size = new Size(help.Width, ClientSize.Height - help.Height);
            playerControl.Location = new Point(help.Left, 0);

           

            runBotIteration.Location = new Point(scores.Right, 0);
            runBotIteration.Size = new Size(100, scores.Height);

            runBotGame.Location = new Point(runBotIteration.Right, 0);
            runBotGame.Size = new Size(100, scores.Height);


        }

        public TetrisForm(Map map)
        {
            mapHistory.History = new History(map);
            mapHistory.History.Updated += UpdateAll;
            playerControl = new ProgramPlayerControl(mapHistory);
            grid = new Grid(mapHistory);
            this.KeyPreview = true;
            runBotGame = new Button();
            runBotIteration = new Button();
            suggest = new Button();
            runBotIteration.Text = "Iter";
            runBotGame.Text = "Game";
            suggest.Text = "Oracle";

            scores = new Label();
            help = new Label();
            help.Text = "UIOP - movement\r\nQW - rotate\r\nZ - undo\r\nL - lock";
            help.BackColor = Color.Black;
            help.Font = new Font("Arial", 10);
            help.ForeColor = Color.Yellow;

            grid.MovementRequested += Grid_MovementRequested;

            Controls.Add(grid);
            Controls.Add(playerControl);
            Controls.Add(scores);
            Controls.Add(help);
            Controls.Add(runBotGame);
            Controls.Add(runBotIteration);
            Controls.Add(suggest);


            runBotGame.Click += RunBotGame_Click;
            runBotIteration.Click += RunBotIteration_Click;
            suggest.Click += Suggest_Click;

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

        private void Suggest_Click(object sender, EventArgs e)
        {
            if (mapHistory.Playing) return;
            var suggestion = new NamiraOracle().GetSuggestions(Map);
            
        }

        int IterationNumber;

        private void RunBotIteration_Click(object sender, EventArgs e)
        {
            if (mapHistory.Playing) return;
            var program = new NamiraOracle().MakeMove(Map);
            mapHistory.History.Append(program, "Iter"+IterationNumber);
            IterationNumber++;
            mapHistory.Play();
        }

        private void RunBotGame_Click(object sender, EventArgs e)
        {
            if (mapHistory.Playing) return;
            var program = new NamiraOracle().PlayGame(Map);
            mapHistory.History.Append(program, "Game");
            mapHistory.Play();
        }
        private void Grid_MovementRequested(UnitState obj)
        {
            if (mapHistory.Playing) return;
            var program = Finder.GetPath(Map.Filled, Map.Unit.Unit, obj);
            mapHistory.History.Append(program, "Hand");
            mapHistory.Play();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            Text = $"ProblemId: {Map.Id} - W: {Map.Width}, H: {Map.Height}. Press 'H' for help!";
            DoubleBuffered = true;
            WindowState = FormWindowState.Maximized;
        }

        void UpdateAll()
        {
            Invalidate();
            grid.Invalidate();
            scores.Text = Map.Scores.TotalScores.ToString();
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (mapHistory.Playing) return;

            if (keymap.ContainsKey(e.KeyData) && MovementRequested != null && !Map.IsOver)
            {
                mapHistory.History.Append("Kbd", keymap[e.KeyData], e.KeyCode.ToString().ToUpper()[0]);
                mapHistory.History.Forward();
            }
            if (e.KeyData == Keys.Z && mapHistory.History.CurrentPosition > 0)
                mapHistory.History.Backward();
        }
    }
}