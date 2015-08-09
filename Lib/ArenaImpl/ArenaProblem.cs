using System.Linq;
using Newtonsoft.Json;

namespace Lib.ArenaImpl
{
    public class ArenaProblem
    {
        public ArenaMapResult[] MapResults;
        [JsonIgnore]
        public ProblemJson Problem;

        public int Id;

        public int AvgScore { get { return (int)MapResults.Average(r => r.Result.Score); } }

        public override string ToString()
        {
            var mapRes = string.Join(",", MapResults.Select(m => m.ToString()));
            var performance = 100 * AvgScore / Problem.ScoreEstimate;
            return $"{Problem.id}\t{performance}%\t{AvgScore}\t{mapRes}";
        }
    }
}