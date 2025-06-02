using DataLayer.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DataLayer
{
	public class JsonDataService
	{
		private const string JSON_FOLDER = "JsonData";

		// JSON file names based on the project specification
		private const string MEN_TEAMS_FILE = "men_teams_results.json";
		private const string WOMEN_TEAMS_FILE = "women_teams_results.json";
		private const string MEN_MATCHES_FILE = "men_matches.json";
		private const string WOMEN_MATCHES_FILE = "women_matches.json";

		public List<Team> GetTeamsFromJson(string championship)
		{
			try
			{
				string fileName = championship.ToLower() == "men" ? MEN_TEAMS_FILE : WOMEN_TEAMS_FILE;
				string filePath = Path.Combine(JSON_FOLDER, fileName);

				if (!File.Exists(filePath))
				{
					throw new FileNotFoundException($"JSON file not found: {filePath}");
				}

				string jsonContent = File.ReadAllText(filePath);
				var teams = JsonConvert.DeserializeObject<List<Team>>(jsonContent);

				return teams ?? new List<Team>();
			}
			catch (Exception ex)
			{
				// Log the error and return empty list to prevent app crash
				Console.WriteLine($"Error reading teams from JSON: {ex.Message}");
				return new List<Team>();
			}
		}

		public List<Match> GetMatchesFromJson(string championship)
		{
			try
			{
				string fileName = championship.ToLower() == "men" ? MEN_MATCHES_FILE : WOMEN_MATCHES_FILE;
				string filePath = Path.Combine(JSON_FOLDER, fileName);

				if (!File.Exists(filePath))
				{
					throw new FileNotFoundException($"JSON file not found: {filePath}");
				}

				string jsonContent = File.ReadAllText(filePath);
				var matches = JsonConvert.DeserializeObject<List<Match>>(jsonContent);

				return matches ?? new List<Match>();
			}
			catch (Exception ex)
			{
				// Log the error and return empty list to prevent app crash
				Console.WriteLine($"Error reading matches from JSON: {ex.Message}");
				return new List<Match>();
			}
		}

		public List<Match> GetTeamMatchesFromJson(string championship, string fifaCode)
		{
			try
			{
				var allMatches = GetMatchesFromJson(championship);

				// Filter matches where the team played (either as home or away team)
				var teamMatches = allMatches.Where(match =>
					match.HomeTeam?.FifaCode == fifaCode ||
					match.AwayTeam?.FifaCode == fifaCode
				).ToList();

				return teamMatches;
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error filtering team matches from JSON: {ex.Message}");
				return new List<Match>();
			}
		}

		// Helper method to check if JSON files exist
		public bool JsonFilesExist(string championship)
		{
			string teamsFile = championship.ToLower() == "men" ? MEN_TEAMS_FILE : WOMEN_TEAMS_FILE;
			string matchesFile = championship.ToLower() == "men" ? MEN_MATCHES_FILE : WOMEN_MATCHES_FILE;

			string teamsPath = Path.Combine(JSON_FOLDER, teamsFile);
			string matchesPath = Path.Combine(JSON_FOLDER, matchesFile);

			return File.Exists(teamsPath) && File.Exists(matchesPath);
		}

		// Method to create JSON folder if it doesn't exist
		public void EnsureJsonFolderExists()
		{
			if (!Directory.Exists(JSON_FOLDER))
			{
				Directory.CreateDirectory(JSON_FOLDER);
			}
		}

		// Optional: Method to download and save JSON files from API (for caching)
		public async Task CacheApiDataToJsonAsync(string championship)
		{
			try
			{
				EnsureJsonFolderExists();

				var apiService = new ApiService();

				// Download and save teams data
				var teams = await apiService.GetTeamsAsync(championship);
				string teamsFileName = championship.ToLower() == "men" ? MEN_TEAMS_FILE : WOMEN_TEAMS_FILE;
				string teamsPath = Path.Combine(JSON_FOLDER, teamsFileName);
				string teamsJson = JsonConvert.SerializeObject(teams, Formatting.Indented);
				File.WriteAllText(teamsPath, teamsJson);

				// Download and save matches data
				var matches = await apiService.GetMatchesAsync(championship);
				string matchesFileName = championship.ToLower() == "men" ? MEN_MATCHES_FILE : WOMEN_MATCHES_FILE;
				string matchesPath = Path.Combine(JSON_FOLDER, matchesFileName);
				string matchesJson = JsonConvert.SerializeObject(matches, Formatting.Indented);
				File.WriteAllText(matchesPath, matchesJson);

				Console.WriteLine($"Successfully cached {championship} championship data to JSON files.");
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error caching API data to JSON: {ex.Message}");
			}
		}
	}
}