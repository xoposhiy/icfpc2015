using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Lib.Models
{
	public static class CommonExtensions
	{
        public static Random r = new Random(2345432);
        public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> items, Random random = null)
        {
            random = random ?? r;
            var copy = items.ToList();
            for (int i = 0; i < copy.Count; i++)
            {
                var nextIndex = random.Next(i, copy.Count);
                yield return copy[nextIndex];
                copy[nextIndex] = copy[i];
            }
        }

        public static T MaxItem<T>(this IEnumerable<T> items, Func<T, double> getKey)
	    {
	        var best = default(T);
	        double bestKey = double.MinValue;
	        foreach (var item in items)
	        {
	            var key = getKey(item);
	            if (key >= bestKey)
	            {
	                bestKey = key;
	                best = item;
	            }
	        }
	        return best;
	    }

	    public static ImmutableStack<T> TryPop<T>(this ImmutableStack<T> s)
	    {
	        return s.IsEmpty ? s : s.Pop();
	    }

		public static string Repeat(this string s, int count)
		{
			return string.Join("", Enumerable.Range(0, count).Select(i => s));
		}

		public static bool InRange(this int x, int low, int high)
		{
			return low <= x && x <= high;
		}

        public static bool IsGoodPath(this Map map, IEnumerable<Directions> path)
        {
            foreach (var d in path)
            {
                if (!map.IsSafeMovement(d))
                    return false;
                map = map.Move(d);
            }
            return true;
        }
    }
}