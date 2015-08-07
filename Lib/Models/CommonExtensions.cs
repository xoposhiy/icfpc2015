using System.Linq;

namespace Lib.Models
{
	public static class CommonExtensions
	{
		public static string Repeat(this string s, int count)
		{
			return string.Join("", Enumerable.Range(0, count).Select(i => s));
		}

		public static bool InRange(this int x, int low, int high)
		{
			return low <= x && x <= high;
		}
	}
}