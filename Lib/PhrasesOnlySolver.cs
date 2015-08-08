using System.Linq;
using Lib.Models;

namespace Lib
{
    public class PhrasesOnlySolver
    {
        public static string Solve(Map map)
        {
            var p =
                from w in Phrases.all
                where !map.Move(w.ToDirections()).IsOver
                select w;
            return p.Last();
        }
    }
}