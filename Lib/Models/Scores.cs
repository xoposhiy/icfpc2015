using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lib.Models
{
    public class Scores
    {

        public readonly int TotalScores;
        public readonly int ClearedLinesCountAtThisMap;
        public Scores(int totalScores, int clearedLinesCountAtThisMap)
        {
            TotalScores = totalScores;
            ClearedLinesCountAtThisMap = clearedLinesCountAtThisMap;
        }
    }
}
