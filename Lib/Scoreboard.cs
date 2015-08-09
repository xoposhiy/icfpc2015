using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace Lib
{
    public class TeamRank
    {
        public int score { get; set; }
        public int teamId { get; set; }
        public string team { get; set; }
        public int power_score { get; set; }
        public int rank { get; set; }
        public List<string> tags { get; set; }
    }

    public class ProblemRanking
    {
        [JsonProperty("setting")]
        public int ProblemId { get; set; }
        public List<TeamRank> rankings { get; set; }
    }

    public class ScoreboardClient
    {
        [Test, Explicit]
        public void FindBadProblems()
        {
            var myId = 37;
            var problems = LoadProblemRankings();
            var myRes = problems.Select(
                    p => Tuple.Create(p, p.rankings.Single(r => r.teamId == myId)))
                .OrderByDescending(r => r.Item2.rank)
                .ToList();
            foreach (var r in myRes)
            {
                Console.WriteLine(r.Item1.ProblemId + "\t" + r.Item2.rank);   
            }
            Console.WriteLine(myRes.Sum(r => r.Item2.rank));
        }

        public static ProblemRanking[] LoadProblemRankings()
        {
            var s = new HttpClient().GetStringAsync("https://davar.icfpcontest.org/rankings.js").Result;
            JObject v = (JObject)JsonConvert.DeserializeObject(s.Substring(11));
            var problems = v["data"]["settings"].ToObject<ProblemRanking[]>();
            return problems;
        }

        public static int[] GetMyAvScores(int? teamId = null)
        {
            return LoadProblemRankings().Select(p => p.rankings.Single(r => r.teamId == (teamId ?? SubmitionClient.Default.TeamId)).score).ToArray();
        }

        [Test]
        public void MyAvScores()
        {
            foreach (var score in GetMyAvScores())
            {
                Console.WriteLine(score);
            }
        }
    }
}