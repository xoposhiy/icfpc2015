using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NUnit.Framework;

namespace Lib
{
	public class SubmitionJson
	{
		public int problemId { get; set; }

		public int seed { get; set; }

		public string tag { get; set; }

		public string solution { get; set; }
	}

	public class SubmitionClient
	{
		private readonly int teamId;
		private readonly string apiKey;

		public SubmitionClient(int teamId, string apiKey)
		{
			this.teamId = teamId;
			this.apiKey = apiKey;
		}

		public async Task PostSubmition(SubmitionJson submitionJson)
		{
			using (var client = new HttpClient())
			{
				client.BaseAddress = new Uri("https://davar.icfpcontest.org/teams/" + teamId + "/solutions");
				var byteArray = Encoding.ASCII.GetBytes(":" + apiKey);
				client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
				var payload = JsonConvert.SerializeObject(new[] {submitionJson});
				var content = new StringContent(payload, Encoding.UTF8, "application/json");

				var result = await client.PostAsync("", content);
				var ans = await result.Content.ReadAsStringAsync();
				if (!result.IsSuccessStatusCode || ans != "created")
					throw new Exception("Error: " + ans + "\r\n" + result);
			}
		}
	}

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

	[TestFixture]
	public class SubmitionJsonTest
	{

		[Test]
		public async void SendOne()
		{
			var client = new SubmitionClient(37, "0u0hbMTthhsUHWOZwAngrgZBZZM5J/OuXaexUvIsP0k=");
			await client.PostSubmition(new SubmitionJson
			{
				problemId = 0,
				seed = 0,
				solution = "Ph'nglui mglw'nafh Cthulhu R'lyeh wgah'nagl fhtagn!Ei!Ei!"
			});

		}
		[Test]
		public async void Test()
		{
			var client = new SubmitionClient(37, "0u0hbMTthhsUHWOZwAngrgZBZZM5J/OuXaexUvIsP0k=");
			foreach (var prob in Problems.LoadProblems())
			{
				foreach (var seed in prob.sourceSeeds)
				{
					var submition = new SubmitionJson
					{
						problemId = prob.id,
						seed = seed,
						solution = "Ei!Ph'nglui mglw'nafh Cthulhu R'lyeh wgah'nagl fhtagn!Ei!Ei!Ph'nglui mglw'nafh Cthulhu R'lyeh wgah'nagl fhtagn!"
					};
					await client.PostSubmition(submition);
					Console.WriteLine(prob.id + " " + seed + " sent");
				}
			}
		}
	}
}