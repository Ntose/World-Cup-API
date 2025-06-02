using DataLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataLayer
{
	public class DataManager
	{
		private readonly ApiService apiService;
		private readonly JsonDataService jsonDataService;

		public DataManager()
		{
			apiService = new ApiService();
			jsonDataService = new JsonDataService();
		}

		public async Task<List<Team>> GetTeamsAsync(string championship)
		{
			if (ConfigurationManager.UseApiData)
				return await apiService.GetTeamsAsync(championship);
			else
				return jsonDataService.GetTeamsFromJson(championship);
		}

		public async Task<List<Match>> GetMatchesAsync(string championship)
		{
			if (ConfigurationManager.UseApiData)
				return await apiService.GetMatchesAsync(championship);
			else
				return jsonDataService.GetMatchesFromJson(championship);
		}

		// This method was missing from the original implementation
		public async Task<List<Match>> GetTeamMatchesAsync(string championship, string fifaCode)
		{
			if (ConfigurationManager.UseApiData)
				return await apiService.GetTeamMatchesAsync(championship, fifaCode);
			else
				return jsonDataService.GetTeamMatchesFromJson(championship, fifaCode);
		}

		// Helper method to get all players from a team's first match
		public async Task<List<Player>> GetTeamPlayersAsync(string championship, string fifaCode)
		{
			var teamMatches = await GetTeamMatchesAsync(championship, fifaCode);
			var firstMatch = teamMatches.FirstOrDefault();

			if (firstMatch == null)
				return new List<Player>();

			var allPlayers = new List<Player>();

			// Determine if the selected team was home or away in this match
			if (firstMatch.HomeTeam?.FifaCode == fifaCode)
			{
				allPlayers.AddRange(firstMatch.HomeTeamStartingEleven ?? new List<Player>());
				allPlayers.AddRange(firstMatch.HomeTeamSubstitutes ?? new List<Player>());
			}
			else if (firstMatch.AwayTeam?.FifaCode == fifaCode)
			{
				allPlayers.AddRange(firstMatch.AwayTeamStartingEleven ?? new List<Player>());
				allPlayers.AddRange(firstMatch.AwayTeamSubstitutes ?? new List<Player>());
			}

			return allPlayers.Distinct().ToList(); // Remove duplicates if any
		}

		// Helper method to get match between two specific teams
		public async Task<Match> GetMatchBetweenTeamsAsync(string championship, string team1FifaCode, string team2FifaCode)
		{
			var allMatches = await GetMatchesAsync(championship);

			return allMatches.FirstOrDefault(match =>
				(match.HomeTeam?.FifaCode == team1FifaCode && match.AwayTeam?.FifaCode == team2FifaCode) ||
				(match.HomeTeam?.FifaCode == team2FifaCode && match.AwayTeam?.FifaCode == team1FifaCode)
			);
		}

		// Helper method to get team statistics
		public async Task<Team> GetTeamByFifaCodeAsync(string championship, string fifaCode)
		{
			var teams = await GetTeamsAsync(championship);
			return teams.FirstOrDefault(t => t.FifaCode == fifaCode);
		}

		// Method to check data availability
		public bool IsDataSourceAvailable(string championship)
		{
			if (ConfigurationManager.UseApiData)
			{
				// For API, we assume it's available (actual check would require a test call)
				return true;
			}
			else
			{
				return jsonDataService.JsonFilesExist(championship);
			}
		}

		// Method to cache API data to JSON for offline use
		public async Task CacheDataAsync(string championship)
		{
			if (ConfigurationManager.UseApiData)
			{
				await jsonDataService.CacheApiDataToJsonAsync(championship);
			}
		}
	}
}