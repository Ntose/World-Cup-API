using System.Collections.Generic;
using System;

namespace DataLayer.Models
{
	public class AppSettings
	{
		public string Tournament { get; set; }    // e.g. "men" or "women"
		public string Language { get; set; }        // e.g. "en" or "hr"
		public string DisplayMode { get; set; }     // "Fullscreen" or "Windowed"
		public int Width { get; set; }              // used if windowed
		public int Height { get; set; }             // used if windowed

		public override string ToString()
		{
			// For WindowSize below, if DisplayMode equals "Fullscreen", output "fullscreen".
			// Otherwise, combine the width and height into WIDTHxHEIGHT
			string windowSizeOutput;
			if (string.Equals(DisplayMode, "Fullscreen", StringComparison.OrdinalIgnoreCase))
			{
				windowSizeOutput = "fullscreen";
			}
			else
			{
				windowSizeOutput = $"{Width}x{Height}";
			}

			// Since the configuration file written by ConfigurationManager includes keys for
			// UseApiData, SelectedChampionship, SelectedLanguage, SelectedTeam, FavoriteTeam, FavoritePlayers,
			// and WindowSize, we produce those keys here.
			// Note: for SelectedTeam, FavoriteTeam, and FavoritePlayers we don't have values in AppSettings,
			// so they are written as empty.
			bool useApiData = true;  // You can modify this if necessary

			var lines = new List<string>
			{
				"# World Cup Statistics Configuration File",
				"# Automatically generated - do not edit manually unless you know what you're doing",
				"",
				$"UseApiData={useApiData}",
				$"SelectedChampionship={Tournament}",
				$"SelectedLanguage={Language}",
				$"SelectedTeam=",
				$"FavoriteTeam=",
				$"FavoritePlayers=",
				$"WindowSize={windowSizeOutput}",
				"",
				"# Valid values:",
				"# UseApiData: true/false",
				"# SelectedChampionship: men/women",
				"# SelectedLanguage: en/hr",
				"# SelectedTeam: FIFA code (e.g., ENG, CRO, etc.)",
				"# FavoriteTeam: FIFA code (e.g., ENG, CRO, etc.)",
				"# FavoritePlayers: comma-separated list of player names",
				"# WindowSize: WIDTHxHEIGHT (e.g., 1024x768, 1280x720) or 'fullscreen'"
			};

			return string.Join(Environment.NewLine, lines);
		}
	}
}
