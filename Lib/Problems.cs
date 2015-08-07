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
			return 
				Directory.GetFiles(@"problems", "problem*.json")
				         .Select(File.ReadAllText)
				         .Select(JsonConvert.DeserializeObject<ProblemJson>)
				         .ToList();
		}
	}
}