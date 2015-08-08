namespace Lib.ArenaImpl
{
    public class SolverResult
    {
        public SolverResult(string name, int score, string commands)
        {
            Name = name;
            Score = score;
            Commands = commands;
        }

        public override string ToString()
        {
            return $"Score: {Score}, Commands: {Commands}";
        }

        public readonly string Name;
        public readonly int Score;
        public readonly string Commands;
    }
}