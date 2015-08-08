namespace Lib.ArenaImpl
{
    public class ArenaMapResult
    {
        public int Seed;
        public SolverResult Result;

        public override string ToString()
        {
            if (Result == null) return "";
            return Result.Score.ToString();
        }
    }
}