using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace Lib
{
	public class Problems
	{
		public static List<ProblemJson> LoadProblems()
		{
			return LoadProblems(Directory.GetFiles(@"problems", "problem*.json"))
				.OrderBy(p => p.id)
				.ToList();
		}

		public static List<ProblemJson> LoadProblems(IEnumerable<string> filenames)
		{
			return filenames
				.Select(File.ReadAllText)
				.Select(JsonConvert.DeserializeObject<ProblemJson>)
				.ToList();
		}
	}
}