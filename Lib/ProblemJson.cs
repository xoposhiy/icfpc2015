using System.Collections.Generic;
using ApprovalTests;
using ApprovalTests.Reporters;
using Newtonsoft.Json;
using NUnit.Framework;

namespace Lib
{
	public class ProblemJson
	{
		public int height { get; set; }
		public int width { get; set; }
		public List<int> sourceSeeds { get; set; }
		public List<UnitJson> units { get; set; }
		public int id { get; set; }
		public List<CellJson> filled { get; set; }
		public int sourceLength { get; set; }
	}

	[TestFixture]
	public class ProblemJsonTest
	{
		[Test, UseReporter(typeof(DiffReporter))]
		public void Test()
		{
			var p = new ProblemJson
			{
				filled = new List<CellJson>() { new CellJson() { x = 1, y = 2 } },
				height = 10,
				width = 12,
				id = 42,
				sourceLength = 22,
				sourceSeeds = new List<int>() { 1, 2, 3 },
				units = new List<UnitJson>()
				{
					new UnitJson()
					{
						members = new List<CellJson>() {new CellJson() {x = 10, y = 11}},
						pivot = new CellJson(){x=10, y=11}
					}
				}
			};
			var s = JsonConvert.SerializeObject(p, Formatting.Indented);
			Approvals.Verify(s);
		}
	}
}