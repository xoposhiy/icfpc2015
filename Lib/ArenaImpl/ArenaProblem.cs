using System.Linq;
using Newtonsoft.Json;

namespace Lib.ArenaImpl
{
    public class ArenaProblem
    {
        public ArenaMapResult[] MapResults;
        [JsonIgnore]
        public ProblemJson Problem;

        public int Id => Problem.id;

        public int AvgScore { get { return (int)MapResults.Average(r => r.Result.Score); } }

        public override string ToString()
        {
            var mapRes = string.Join(",", MapResults.Select(m => m.ToString()));
            return $"{Problem.id}\t{AvgScore}\t{mapRes}";
        }
    }
}