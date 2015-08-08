namespace Lib.ArenaImpl
{
    public class SolverResult
    {
        public SolverResult(int score, string commands)
        {
            Score = score;
            Commands = commands;
        }

        public override string ToString()
        {
            return $"Score: {Score}, Commands: {Commands}";
        }

        public readonly int Score;
        public readonly string Commands;
    }
}