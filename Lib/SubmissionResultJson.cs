using Newtonsoft.Json;

namespace Lib
{
	public class SubmissionResultJson
	{
		public int? powerScore { get; set; }
		public int seed { get; set; }
		public string tag { get; set; }
		public string createdAt { get; set; }
		public int? score { get; set; }
		public int authorId { get; set; }
		public int teamId { get; set; }
		public int problemId { get; set; }
		public string solution { get; set; }

		public override string ToString()
		{
			return JsonConvert.SerializeObject(this, Formatting.Indented);
		}
	}
}