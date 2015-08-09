using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Lib;
using Lib.Finder;
using Lib.Models;

namespace ManualControl
{
    internal class TetrisForm : Form
    {
        private readonly MainModel mapHistory;
        public Map Map => mapHistory.History.CurrentMap;
        private readonly Dictionary<Keys, Directions> keymap;
        public Action<Directions> MovementRequested;
        Grid grid;
        Label scores;
        Label help;
        ProgramPlayerControl player;
        Button suggest,runBotIteration, runBotGame, say;

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            scores.Size = new Size(100, 30);
            grid.Location = new Point(0, 30);
            grid.Size = new Size(ClientSize.Width / 2, ClientSize.Height - 30);

            help.Size = new Size(ClientSize.Width-grid.Width, 100);
            help.Location = new Point(grid.Right, ClientSize.Height- help.Height);

            player.Location = new Point(grid.Right, 0);
            player.Size = new Size(help.Width, ClientSize.Height - help.Height);

            suggest.Location = new Point(scores.Right, 0);
            suggest.Size = new Size(100, scores.Height);

            runBotIteration.Location = new Point(suggest.Right, 0);
            runBotIteration.Size = suggest.Size;
            runBotGame.Location = new Point(runBotIteration.Right, 0);
            runBotGame.Size = suggest.Size;

            say.Location = new Point(runBotGame.Right, 0);
            say.Size = runBotGame.Size;
        }

        public TetrisForm(MainModel model)
        {
            mapHistory = model;
            mapHistory.History.Updated += UpdateAll;
            mapHistory.Suggestions.Updated += () => suggest.Text = "Oracle " + mapHistory.Suggestions.Position;
            grid = new Grid(mapHistory);
            this.KeyPreview = true;
            runBotGame = new Button();
            runBotIteration = new Button();
            suggest = new Button();
            say = new Button();
            player = new ProgramPlayerControl(mapHistory);
            runBotIteration.Text = "Iter";
            runBotGame.Text = "Game";
            suggest.Text = "Oracle";
            say.Text = "Say";

            scores = new Label();
            help = new Label();
            help.Text = "UIOP — movement\r\nQW — rotate\r\nZ — undo\r\nAS — switch between maps\r\nCtrl+C — copy current commamds to clipboard";
            help.BackColor = Color.Black;
            help.Font = new Font("Arial", 10);
            help.ForeColor = Color.Yellow;

            grid.MovementRequested += Grid_MovementRequested1;

            Controls.Add(grid);
            Controls.Add(scores);
            Controls.Add(help);
            Controls.Add(runBotGame);
            Controls.Add(runBotIteration);
            Controls.Add(suggest);
            Controls.Add(player);
            Controls.Add(say);
            currentProblemIndex = Map.Id;

            runBotGame.Click += RunBotGame_Click;
            runBotIteration.Click += RunBotIteration_Click;
            suggest.Click += Suggest_Click;
            say.Click += Say_Click;

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

        private void Say_Click(object sender, EventArgs e)
        {
            var form = new Form
            {
                ClientSize = new Size(800, 350),
                Controls =
                {
                    new TextBox
                    {
                        Size=new Size(800,300),
                        Multiline=true,
                    },
                    new Button
                    {
                        Size=new Size(100,50),
                        Location=new Point(500,300),
                        DialogResult= DialogResult.OK,
                        Text="OK"
                    },
                    new Button
                    {
                        Size=new Size(100,50),
                        Location=new Point(700,300),
                        DialogResult= DialogResult.Cancel,
                        Text="Cancel"
                    }
                }
            };
            var dlg = form.ShowDialog();
            if (dlg != DialogResult.OK) return;
            var text = form.Controls[0].Text;

            mapHistory.History.Append(text, "Say");
            mapHistory.Play();
        }

        private void Grid_MovementRequested1(UnitPosition obj)
        {
            if (mapHistory.Playing) return;
            var path = mapHistory.Solver.Finder.GetPath(Map,obj);
            if (path == null)
            {
                MessageBox.Show("Сам туда иди!");
                return;
            }
            var program = path.ToPhrase().ToOriginalPhrase();
            mapHistory.History.Append(program, "Hand");
            mapHistory.Play();
        }

        private void Suggest_Click(object sender, EventArgs e)
        {
            if (mapHistory.Playing) return;
            if (mapHistory.Suggestions.GetCurrentSuggestion()==null
                || mapHistory.Suggestions.Unit!=Map.Unit.Unit
                || !mapHistory.Suggestions.Next())
                { 
                var suggestion = mapHistory.Solver.Oracle.GetSuggestions(Map);
                mapHistory.Suggestions.Load(suggestion, Map.Unit.Unit);
                }
            UpdateAll();
        }

        int IterationNumber;

        private void RunBotIteration_Click(object sender, EventArgs e)
        {
            if (mapHistory.Playing) return;
            MakeMove();
        }

        void MakeMove()
        {
            var program = mapHistory.Solver.MakeMove(Map)?.ToPhrase()?.ToOriginalPhrase();
            if (program == null) return;
            mapHistory.History.Append(program, "Iter" + IterationNumber);
            IterationNumber++;
            mapHistory.Play();
        }

        private void RunBotGame_Click(object sender, EventArgs e)
        {
            if (mapHistory.Playing) return;
            mapHistory.ContinuationRequest += () =>
            {
                if (!mapHistory.History.Items.Last().Map.IsOver)
                    MakeMove();
            };
            MakeMove();
        }
  

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            Text = $"ProblemId: {Map.Id} - W: {Map.Width}, H: {Map.Height}.";
            DoubleBuffered = true;
            WindowState = FormWindowState.Maximized;
        }

        void UpdateAll()
        {
            Text = $"ProblemId: {Map.Id} - W: {Map.Width}, H: {Map.Height}.";
            Invalidate();
            grid.Invalidate();
            scores.Text = Map.Scores.TotalScores.ToString();
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (mapHistory.Playing) return;

            if (keymap.ContainsKey(e.KeyData) && MovementRequested != null && !Map.IsOver)
            {
                mapHistory.History.Append("Kbd", keymap[e.KeyData], keymap[e.KeyData].ToChar());
                mapHistory.History.Forward();
            }
            if (e.KeyData == Keys.Z && mapHistory.History.CurrentPosition > 0)
                mapHistory.History.Backward();
            if (e.KeyData == Keys.Escape)
                mapHistory.Suggestions.Clear();
            if (e.KeyData == Keys.A)
                ProblemIndex--;
            if (e.KeyData == Keys.S)
                ProblemIndex++;

            if (e.Control && e.KeyCode == Keys.C)
                Clipboard.SetText(string.Join("", mapHistory.History.Items.Skip(1).Select(x => x.Char)).ToOriginalPhrase());
        }

        private List<ProblemJson> problems = Problems.LoadProblems();

        private int ProblemIndex
        {
            get
            {
                return currentProblemIndex;
            }
            set
            {
                currentProblemIndex = (value + problems.Count) % problems.Count;
                mapHistory.History = new History(problems[currentProblemIndex].ToMap(0));
                mapHistory.History.Updated += UpdateAll;
                grid.UpdateRadius();
                grid.Invalidate();
                UpdateAll();

            }
        }

        private int currentProblemIndex = 0;
    }
}