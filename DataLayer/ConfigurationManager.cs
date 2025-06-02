using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DataLayer.Models;

namespace DataLayer
{
	public static class ConfigurationManager
	{
		private const string CONFIG_FILE = "config.txt";

		public static bool UseApiData { get; set; } = true;
		public static string SelectedChampionship { get; set; } = "men";
		public static string SelectedLanguage { get; set; } = "en";
		public static string SelectedTeam { get; set; } // Fixed property name
		public static string FavoriteTeam { get; set; } // Keep this for backward compatibility
		public static List<string> FavoritePlayers { get; set; } = new List<string>();
		public static string WindowSize { get; set; } = "1024x768";

		public static void LoadConfiguration()
		{
			try
			{
				if (File.Exists(CONFIG_FILE))
				{
					var lines = File.ReadAllLines(CONFIG_FILE);

					foreach (var line in lines)
					{
						// Skip empty lines and comments
						if (string.IsNullOrWhiteSpace(line) || line.StartsWith("#"))
							continue;

						// Split by '=' and ensure we have key and value
						var parts = line.Split('=');
						if (parts.Length < 2)
							continue;

						var key = parts[0].Trim();
						var value = string.Join("=", parts.Skip(1)).Trim(); // Handle values with '=' in them

						// Parse each configuration value
						switch (key.ToLowerInvariant())
						{
							case "useapidata":
								if (bool.TryParse(value, out bool useApi))
									UseApiData = useApi;
								break;

							case "selectedchampionship":
								if (value == "men" || value == "women")
									SelectedChampionship = value;
								break;

							case "selectedlanguage":
								if (value == "en" || value == "hr")
									SelectedLanguage = value;
								break;

							case "selectedteam":
								if (!string.IsNullOrEmpty(value))
									SelectedTeam = value;
								break;

							case "favoriteteam":
								if (!string.IsNullOrEmpty(value))
								{
									FavoriteTeam = value;
									// Sync with SelectedTeam for backward compatibility
									if (string.IsNullOrEmpty(SelectedTeam))
										SelectedTeam = value;
								}
								break;

							case "favoriteplayers":
								if (!string.IsNullOrEmpty(value))
								{
									// Parse comma-separated list of favorite players
									FavoritePlayers = value.Split(',')
										.Select(p => p.Trim())
										.Where(p => !string.IsNullOrEmpty(p))
										.ToList();
								}
								break;

							case "windowsize":
								if (!string.IsNullOrEmpty(value))
									WindowSize = value;
								break;
						}
					}
				}
			}
			catch (Exception ex)
			{
				// Log error or handle gracefully - don't crash the application
				Console.WriteLine($"Error loading configuration: {ex.Message}");
				// Keep default values
			}
		}

		public static void SaveConfiguration()
		{
			try
			{
				var configLines = new List<string>
				{
					"# World Cup Statistics Configuration File",
					"# Generated automatically - do not edit manually unless you know what you're doing",
					"",
					$"UseApiData={UseApiData}",
					$"SelectedChampionship={SelectedChampionship}",
					$"SelectedLanguage={SelectedLanguage}",
					$"SelectedTeam={SelectedTeam ?? ""}",
					$"FavoriteTeam={FavoriteTeam ?? ""}",
					$"FavoritePlayers={string.Join(",", FavoritePlayers ?? new List<string>())}",
					$"WindowSize={WindowSize}",
					"",
					"# Valid values:",
					"# UseApiData: true/false",
					"# SelectedChampionship: men/women",
					"# SelectedLanguage: en/hr",
					"# SelectedTeam: FIFA code (e.g., ENG, CRO, etc.)",
					"# FavoriteTeam: FIFA code (e.g., ENG, CRO, etc.)",
					"# FavoritePlayers: comma-separated list of player names",
					"# WindowSize: WIDTHxHEIGHT (e.g., 1024x768, 1280x720, fullscreen)"
				};

				File.WriteAllLines(CONFIG_FILE, configLines);
			}
			catch (Exception ex)
			{
				// Log error or handle gracefully
				Console.WriteLine($"Error saving configuration: {ex.Message}");
			}
		}

		// Helper methods for specific configuration checks
		public static bool IsConfigurationComplete()
		{
			return !string.IsNullOrEmpty(SelectedChampionship) &&
				   !string.IsNullOrEmpty(SelectedLanguage);
		}

		public static bool HasFavoriteTeamSelected()
		{
			return !string.IsNullOrEmpty(FavoriteTeam) || !string.IsNullOrEmpty(SelectedTeam);
		}

		public static void AddFavoritePlayer(string playerName)
		{
			if (string.IsNullOrEmpty(playerName))
				return;

			if (FavoritePlayers == null)
				FavoritePlayers = new List<string>();

			if (!FavoritePlayers.Contains(playerName))
			{
				FavoritePlayers.Add(playerName);
				SaveConfiguration(); // Auto-save when favorites change
			}
		}

		public static void RemoveFavoritePlayer(string playerName)
		{
			if (string.IsNullOrEmpty(playerName))
				return;

			if (FavoritePlayers?.Contains(playerName) == true)
			{
				FavoritePlayers.Remove(playerName);
				SaveConfiguration(); // Auto-save when favorites change
			}
		}

		public static void SetFavoriteTeam(string teamFifaCode)
		{
			FavoriteTeam = teamFifaCode;
			SelectedTeam = teamFifaCode; // Keep both in sync
			SaveConfiguration(); // Auto-save when favorite team changes
		}

		// Method to save player favorite status
		public static void SavePlayerFavorite(Player player)
		{
			if (player == null || string.IsNullOrEmpty(player.Name))
				return;

			if (player.IsFavorite)
				AddFavoritePlayer(player.Name);
			else
				RemoveFavoritePlayer(player.Name);
		}
	}
}