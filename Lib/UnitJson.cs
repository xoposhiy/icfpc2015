using System.Collections.Generic;
using System.Linq;
using Lib.Models;

namespace Lib
{
	public class UnitJson
	{
		public List<CellJson> members { get; set; }
		public CellJson pivot { get; set; }

		public Unit ToUnit()
		{
			return new Unit(members.Select(c => c.ToPoint()).ToList(), pivot.ToPoint());
		}

	}
}