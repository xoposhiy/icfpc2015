using System;
using System.Collections.Generic;
using System.Linq;
using Lib.Intelligence;
using NUnit.Framework;

namespace Lib.Models
{
    public static class MapExtensions
    {
        public static IEnumerable<Directions> ToDirections(this string s)
        {
            return s.ToLowerInvariant()
                    .Where(c => !"\t\n\r".Contains(c))
                    .Select(ToDirection);
        }

        public static string ToPhrase(this IEnumerable<Directions> ds)
        {
            return new string(ds.Select(d => d.ToChar()).ToArray());
        }

        public static char ToChar(this Directions d)
        {
            return "labpdk"[(int)d];
        }

        public static Directions ToDirection(this char c)
        {
            c = Char.ToLowerInvariant(c);
            if ("p'!.03".Contains(c)) return Directions.W;
            if ("bcefy2".Contains(c)) return Directions.E;
            if ("aghij4".Contains(c)) return Directions.SW;
            if ("lmno 5".Contains(c)) return Directions.SE;
            if ("dqrvz1".Contains(c)) return Directions.CW;
            if ("kstuwx".Contains(c)) return Directions.CCW;
            throw new Exception(c.ToString());
        }

        public static Map Move(this Map map, IEnumerable<Directions> ds)
        {
            return ds.Aggregate(map, (m, d) => m.Move(d));
        }

        public static List<OracleSuggestion> SuggestAllFinalPositions(this Map map)
        {
            var goodStates = new List<OracleSuggestion>();

            foreach (var pos in OracleServices.GetAllUnitPositions(map))
            {
                var positionedUnit = map.Unit.WithNewPosition(pos);
                if (!map.IsValidPosition(positionedUnit)) continue;

                foreach (var dir in OracleServices.GetAllDirections())  
                {
                    var nextPosition = positionedUnit.Move(dir);
                    if (!map.IsValidPosition(nextPosition))
                    {
                        goodStates.Add(new OracleSuggestion(pos, dir));
                    }
                }
            }
            return goodStates;
        }
    }

    [TestFixture]
    public class MapExtensionsTest
    {
        [Test]
        public void Test()
        {
            var ds = string.Join("", Phrases.all).ToLower().ToDirections().ToList();
            var converted = ds.ToPhrase().ToDirections();
            CollectionAssert.AreEqual(ds, converted);
        }
    }
}