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

		// Basic application settings.
		public static bool UseApiData { get; set; } = true;
		public static string SelectedChampionship { get; set; } = "men"; // "men" or "women"
		public static string SelectedLanguage { get; set; } = "en"; // "en" or "hr"
		public static string SelectedTeam { get; set; }        // e.g., FIFA code such as "ENG", "CRO", etc.
		public static string FavoriteTeam { get; set; }        // For backward compatibility; keeps in sync with SelectedTeam.
		public static List<string> FavoritePlayers { get; set; } = new List<string>();

		// WindowSize holds either a resolution (e.g., "1024x768", "1280x720") 
		// or the string "fullscreen" to denote a full screen display mode.
		public static string WindowSize { get; set; } = "1024x768";

		/// <summary>
		/// Loads the application configuration from disk.
		/// If the file does not exist, default values remain.
		/// </summary>


		public static void LoadConfiguration()
		{
			try
			{
				if (File.Exists(CONFIG_FILE))
				{
					var lines = File.ReadAllLines(CONFIG_FILE);

					foreach (var line in lines)
					{
						// Skip empty lines and comments (lines starting with '#')
						if (string.IsNullOrWhiteSpace(line) || line.StartsWith("#"))
							continue;

						// Split by '=' (if the value contains '=' the rest gets reassembled)
						var parts = line.Split('=');
						if (parts.Length < 2)
							continue;

						var key = parts[0].Trim();
						var value = string.Join("=", parts.Skip(1)).Trim(); // Handle values with '=' in them

						// Parse each configuration value based on key.
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
									// For backward compatibility, sync with SelectedTeam if needed
									if (string.IsNullOrEmpty(SelectedTeam))
										SelectedTeam = value;
								}
								break;

							case "favoriteplayers":
								if (!string.IsNullOrEmpty(value))
								{
									// Expecting a comma-separated list of favorite players
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
				// Log or handle the exception any way you prefer.
				Console.WriteLine($"Error loading configuration: {ex.Message}");
				// In this case, default values remain.
			}
		}


		/// <summary>
		/// Saves the current configuration to disk.
		/// </summary>
		public static void SaveConfiguration()
		{
			try
			{
				var configLines = new List<string>
				{
					"# World Cup Statistics Configuration File",
					"# Automatically generated - do not edit manually unless you know what you're doing",
					"",
					$"UseApiData={UseApiData}",
					$"SelectedChampionship={SelectedChampionship}",
					$"SelectedLanguage={SelectedLanguage}",
					$"SelectedTeam={SelectedTeam ?? ""}",
					$"FavoriteTeam={FavoriteTeam ?? ""}",
					$"FavoritePlayers={string.Join(",", FavoritePlayers ?? new List<string>())}",
                    // WindowSize can be a resolution or the literal "fullscreen".
                    $"WindowSize={WindowSize}",
					"",
					"# Valid values:",
					"# UseApiData: true/false",
					"# SelectedChampionship: men/women",
					"# SelectedLanguage: en/hr",
					"# SelectedTeam: FIFA code (e.g., ENG, CRO, etc.)",
					"# FavoriteTeam: FIFA code (e.g., ENG, CRO, etc.)",
					"# FavoritePlayers: comma-separated list of player names",
					"# WindowSize: either a resolution (e.g., 1024x768, 1280x720) or 'fullscreen'"
				};
				File.WriteAllLines(CONFIG_FILE, configLines);
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error saving configuration: {ex.Message}");
			}
		}

		/// <summary>
		/// Determines if the configuration is complete.
		/// In this case, we ensure that SelectedChampionship and SelectedLanguage are set.
		/// </summary>
		public static bool IsConfigurationComplete()
		{
			return !string.IsNullOrEmpty(SelectedChampionship) &&
				   !string.IsNullOrEmpty(SelectedLanguage);
		}

		/// <summary>
		/// Checks if a favorite team has been selected.
		/// </summary>
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
				SaveConfiguration(); // Auto-save when favorites change.
			}
		}

		public static void RemoveFavoritePlayer(string playerName)
		{
			if (string.IsNullOrEmpty(playerName))
				return;
			if (FavoritePlayers?.Contains(playerName) == true)
			{
				FavoritePlayers.Remove(playerName);
				SaveConfiguration(); // Auto-save when favorites change.
			}
		}

		/// <summary>
		/// Synchronizes the favorite team with the provided FIFA code.
		/// </summary>
		public static void SetFavoriteTeam(string teamFifaCode)
		{
			FavoriteTeam = teamFifaCode;
			SelectedTeam = teamFifaCode; // Keep both in sync.
			SaveConfiguration(); // Auto-save on change.
		}

		/// <summary>
		/// Saves a player’s favorite status.
		/// </summary>
		public static void SavePlayerFavorite(Player player)
		{
			if (player == null || string.IsNullOrEmpty(player.Name))
				return;
			if (player.IsFavorite)
				AddFavoritePlayer(player.Name);
			else
				RemoveFavoritePlayer(player.Name);
		}

		/// <summary>
		/// Gets the favorite team code from a dedicated file.
		/// This is maintained for backward compatibility.
		/// </summary>
		public static string FavoriteTeamCode
		{
			get
			{
				// Choose the file based on the championship.
				string filename = SelectedChampionship.ToLower() == "women"
					? "favorite_team_women.txt"
					: "favorite_team_men.txt";
				if (File.Exists(filename))
				{
					return File.ReadAllText(filename).Trim();
				}
				return null;
			}
		}

		/// <summary>
		/// Returns true if the window should be displayed in fullscreen mode.
		/// We determine this by checking if WindowSize equals "fullscreen" (case-insensitive).
		/// </summary>
		public static bool IsFullscreen()
		{
			return string.Equals(WindowSize, "fullscreen", StringComparison.OrdinalIgnoreCase);
		}
	}
}
