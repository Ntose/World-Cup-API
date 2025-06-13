using System;
using System.Collections.Generic;
using System.IO;

namespace WorldCupStats.WinFormsApp.Helpers
{
	public class AppSettings
	{
		public string Language { get; set; }
		public string Tournament { get; set; }
	}

	public static class ConfigManager
	{
		private static readonly string ConfigPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.txt");

		public static AppSettings LoadSettings()
		{
			if (!File.Exists(ConfigPath)) return null;

			var lines = File.ReadAllLines(ConfigPath);
			var settings = new Dictionary<string, string>();

			foreach (var line in lines)
			{
				var parts = line.Split('=');
				if (parts.Length == 2)
				{
					settings[parts[0]] = parts[1];
				}
			}

			if (settings.ContainsKey("Language") && settings.ContainsKey("Tournament"))
			{
				return new AppSettings
				{
					Language = settings["Language"],
					Tournament = settings["Tournament"]
				};
			}

			return null;
		}
		public static void SaveSettings(string language, string tournament)
		{
			var content = $"Language={language}{Environment.NewLine}Tournament={tournament}";
			File.WriteAllText(ConfigPath, content);
		}
	}
}
