using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json;

namespace Lib
{
	public class SubmitionClient
	{
		private readonly int teamId;
		private readonly string apiKey;
		public static readonly SubmitionClient Default = new SubmitionClient(37, "0u0hbMTthhsUHWOZwAngrgZBZZM5J/OuXaexUvIsP0k=");

		public SubmitionClient(int teamId, string apiKey)
		{
			this.teamId = teamId;
			this.apiKey = apiKey;
		}

		public void PostSubmissions(params SubmitionJson[] submitions)
		{
			PostSubmissions(JsonConvert.SerializeObject(submitions));
		}

		public void PostSubmissions(string payload)
		{
			UseClient(client =>
			{
				var content = new StringContent(payload, Encoding.UTF8, "application/json");
				var result = client.PostAsync("", content).Result;
				var ans = result.Content.ReadAsStringAsync().Result;
				if (!result.IsSuccessStatusCode || ans != "created")
					throw new Exception("Error: " + ans + "\r\n" + result);
				return true;
			});
		}

		public SubmissionResultJson[] GetSubmissions()
		{
			return UseClient(client =>
			{
				var s = client.GetStringAsync("").Result;
				return JsonConvert.DeserializeObject<SubmissionResultJson[]>(s);
			});
		}

		private T UseClient<T>(Func<HttpClient, T> use)
		{
			using (var client = new HttpClient())
			{
				client.BaseAddress = new Uri("https://davar.icfpcontest.org/teams/" + teamId + "/solutions");
				var byteArray = Encoding.ASCII.GetBytes(":" + apiKey);
				client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
				return use(client);
			}
		}
	}
}