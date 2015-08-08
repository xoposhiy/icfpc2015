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
        private readonly Stack<Map> mapHistory = new Stack<Map>();
        public Map Map => mapHistory.Peek();
        private readonly Dictionary<Keys, Directions> keymap;
        public Action<Directions> MovementRequested;
        Grid grid;
        Label scores;
        Label help;

        ProgramPlayer player;
        ProgramPlayerControl playerControl;
        
        private bool showHelp;

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
           grid.Size = grid.GetDesiredSize();
            scores.Size = new Size(grid.Width, 30);
            grid.Location = new Point(0, 30);

            help.Size = new Size(ClientSize.Width-grid.Width, 100);
            help.Location = new Point(grid.Right, ClientSize.Height- help.Height);

           playerControl.Size = new Size(help.Width, ClientSize.Height - help.Height);
            playerControl.Location = new Point(help.Left, 0);
        }

        

        public TetrisForm(Map map)
        {
            this.mapHistory.Push(map);
            player = new ProgramPlayer(mapHistory);
            playerControl = new ProgramPlayerControl(player);
            grid = new Grid(mapHistory);
            this.KeyPreview = true;


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

            player.LoadedUpdated += z => grid.MouseDisabled = z;
            player.MapUpdated += () => UpdateAll();

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

        private void Grid_MovementRequested(UnitState obj)
        {
            if (player.Loaded) return;
            var program = Finder.GetPath(Map.Filled, Map.Unit.Unit, obj);
            player.InitializeProgram(program);
            player.Play();
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
            if (playerControl.KeyboardOccupied) return;
            if (player.Loaded) return;

           if (keymap.ContainsKey(e.KeyData) && MovementRequested != null && !Map.IsOver)
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