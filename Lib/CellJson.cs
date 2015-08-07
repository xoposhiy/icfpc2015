using System.Drawing;

namespace Lib
{
	public class CellJson
	{
		public int x { get; set; }
		public int y { get; set; }

		public Point ToPoint()
		{
			return new Point(x, y);
		}
	}
}