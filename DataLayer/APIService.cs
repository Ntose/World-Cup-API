using DataLayer.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer
{
	public class ApiService
	{
		private static readonly HttpClient httpClient = new HttpClient();
		private const string BASE_URL = "https://worldcup-vua.nullbit.hr";

		public static async Task<List<Team>> GetTeamsAsync(string championship)
		{
			string url = $"{BASE_URL}/{championship}/teams/results";
			string jsonResponse = await httpClient.GetStringAsync(url);
			return JsonConvert.DeserializeObject<List<Team>>(jsonResponse);
		}

		public static async Task<List<Match>> GetMatchesAsync(string championship)
		{
			string url = $"{BASE_URL}/{championship}/matches";
			string jsonResponse = await httpClient.GetStringAsync(url);
			return JsonConvert.DeserializeObject<List<Match>>(jsonResponse);
		}

		public async Task<List<Match>> GetTeamMatchesAsync(string championship, string fifaCode)
		{
			string url = $"{BASE_URL}/{championship}/matches/country?fifa_code={fifaCode}";
			string jsonResponse = await httpClient.GetStringAsync(url);
			return JsonConvert.DeserializeObject<List<Match>>(jsonResponse);
		}
	}
}
