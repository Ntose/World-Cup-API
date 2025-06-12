using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Newtonsoft.Json;
using DataLayer.Models;
using DataLayer;
using WorldCupStats.WinFormsApp.Helpers;

namespace WpfApp
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
			// Load team and match data on start
			LoadTeams();
			LoadMatchDataForSelectedTeam();
			HomeTeamComboBox.SelectionChanged += HomeTeamComboBox_SelectionChanged;
		}

		private async void LoadTeams()
		{
			var teams = await ApiService.GetTeamsAsync(ConfigurationManager.SelectedChampionship);

			// Bind the full team objects to the ComboBox.
			HomeTeamComboBox.ItemsSource = teams.OrderBy(team => team.Country).ToList();
			HomeTeamComboBox.DisplayMemberPath = "Country";

			try
			{
				if (teams.Any(t => t.Country == ConfigurationManager.SelectedTeam))
				{
					var favTeam = teams.FirstOrDefault(t => t.Country == ConfigurationManager.FavoriteTeam);
					HomeTeamComboBox.SelectedItem = favTeam ?? teams.First();
				}
				else
				{
					HomeTeamComboBox.SelectedIndex = 0;
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show($"Error selecting default team: {ex.Message}",
								"Selection Error",
								MessageBoxButton.OK,
								MessageBoxImage.Warning);
			}
		}

		private async void LoadMatchDataForSelectedTeam()
		{
			// Retrieve all matches for the selected championship.
			var matches = await ApiService.GetMatchesAsync(ConfigurationManager.SelectedChampionship);

			// Locate the match based on the favorite team’s FIFA code.
			var match = matches.FirstOrDefault(m =>
				string.Equals(m.HomeTeam?.FifaCode, ConfigurationManager.FavoriteTeamCode, StringComparison.OrdinalIgnoreCase));

			if (match != null)
			{
				// Populate the OpposingTeamComboBox with the away team.
				OpposingTeamComboBox.ItemsSource = new List<string> { match.AwayTeam.Country };
				OpposingTeamComboBox.SelectedIndex = 0;
			}
			else
			{
				MessageBox.Show("No match available for the selected team");
			}
		}

		private async void HomeTeamComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (HomeTeamComboBox.SelectedItem == null)
				return;

			// Cast the selected item to a Team object.
			Team selectedTeam = HomeTeamComboBox.SelectedItem as Team;
			if (selectedTeam == null)
			{
				MessageBox.Show("Invalid team selection.");
				return;
			}

			// Retrieve all matches for the championship.
			var matches = await ApiService.GetMatchesAsync(ConfigurationManager.SelectedChampionship);
			if (matches == null)
			{
				MessageBox.Show("No match data loaded.");
				return;
			}

			// Get the first match where the home team's country matches the selected team.
			var match = matches.FirstOrDefault(m =>
				string.Equals(m.HomeTeam?.Country, selectedTeam.Country, StringComparison.OrdinalIgnoreCase));

			if (match != null)
			{
				var opponentTeams = matches
					.Where(m => string.Equals(m.HomeTeam?.Country, selectedTeam.Country, StringComparison.OrdinalIgnoreCase))
					.Select(m => m.AwayTeam?.Country)
					.Where(awayCountry => !string.IsNullOrEmpty(awayCountry))
					.Distinct()
					.OrderBy(awayCountry => awayCountry)
					.ToList();

				OpposingTeamComboBox.ItemsSource = opponentTeams;
				if (opponentTeams.Any())
					OpposingTeamComboBox.SelectedIndex = 0;

				// Random simulation for match result.
				Random random = new Random();
				int homeTeamGoals = random.Next(0, 4);
				int awayTeamGoals = random.Next(0, 4);
				MatchResultTextBlock.Text = $"{homeTeamGoals} - {awayTeamGoals}";
			}
			else
			{
				OpposingTeamComboBox.ItemsSource = null;
				MatchResultTextBlock.Text = "Match Result Error";
			}
		}

		private async void OpposingTeamComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (OpposingTeamComboBox.SelectedItem == null)
				return;

			string selectedOpposingTeam = OpposingTeamComboBox.SelectedItem.ToString();
			var matches = await ApiService.GetMatchesAsync(ConfigurationManager.SelectedChampionship);
			if (matches == null)
			{
				MessageBox.Show("No match data loaded.");
				return;
			}

			var match = matches.FirstOrDefault(m =>
				string.Equals(m.AwayTeam?.Country, selectedOpposingTeam, StringComparison.OrdinalIgnoreCase));

			if (match == null)
			{
				MessageBox.Show("Selected match not found.");
				return;
			}

			Random random = new Random();
			int homeTeamGoals = random.Next(0, 4);
			int awayTeamGoals = random.Next(0, 4);
			MatchResultTextBlock.Text = $"{homeTeamGoals} - {awayTeamGoals}";
		}

		private async void LoadMatchPlayers_Click(object sender, RoutedEventArgs e)
		{
			if (HomeTeamComboBox.SelectedItem == null)
				return;

			Team selectedTeam = HomeTeamComboBox.SelectedItem as Team;
			if (selectedTeam == null)
				return;

			string fifaCode = selectedTeam.FifaCode;
			var players = await GetHomeTeamPlayers(fifaCode);
			if (players == null)
				return;

			HomePlayersGrid.Children.Clear(); // Clear old player controls.

			foreach (var player in players)
			{
				var playerControl = new PlayerControl(player);
				HomePlayersGrid.Children.Add(playerControl);
			}
		}

		// Updated GetHomeTeamPlayers method: if settings are not loaded, default to English and Men's tournament.
		private async Task<List<Player>> GetHomeTeamPlayers(string fifaCode)
		{
			var settings = ConfigManager.LoadSettings();
			if (settings == null)
			{
				// Default to English and men's tournament if the config file doesn't exist.
				settings = new AppSettings
				{
					Language = "en",
					Tournament = "men"  // Adjust this value if the API expects a different tournament string.
				};
			}

			try
			{
				string url = $"https://worldcup-vua.nullbit.hr/{settings.Tournament}/matches/country?fifa_code={fifaCode}";
				using (HttpClient client = new HttpClient())
				{
					string json = await client.GetStringAsync(url);
					var matches = JsonConvert.DeserializeObject<List<DataLayer.Models.Match>>(json);

					if (matches == null || !matches.Any())
					{
						MessageBox.Show("No matches were returned from the API.");
						return null;
					}

					// Optional: debug the returned matches.
					System.Diagnostics.Debug.WriteLine($"Returned matches count: {matches.Count}");
					foreach (var m in matches)
					{
						System.Diagnostics.Debug.WriteLine($"Match: Home Team - {m.HomeTeam?.FifaCode ?? "N/A"}, Away Team - {m.AwayTeam?.FifaCode ?? "N/A"}");
					}

					// Use case-insensitive comparison
					var match = matches.FirstOrDefault(m =>
						string.Equals(m.HomeTeam?.FifaCode, fifaCode, StringComparison.OrdinalIgnoreCase) ||
						string.Equals(m.AwayTeam?.FifaCode, fifaCode, StringComparison.OrdinalIgnoreCase));

					if (match == null)
					{
						MessageBox.Show("No match found for this team in the returned data.");
						return null;
					}

					// Return the starting eleven based on whether the team is home or away.
					if (string.Equals(match.HomeTeam?.FifaCode, fifaCode, StringComparison.OrdinalIgnoreCase))
						return match.HomeTeamStatistics?.StartingEleven;
					else
						return match.AwayTeamStatistics?.StartingEleven;
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show($"Error fetching players: {ex.Message}");
				return null;
			}
		}


		private void MainWindow_Loaded(object sender, RoutedEventArgs e)
		{
			// For testing: add a dummy player control.
			var dummyPlayer = new Player
			{
				Name = "Test Player",
				ShirtNumber = 10,
				Position = "Midfield",
				Captain = true,
				ImagePath = "" // Use default image when empty.
			};

			var playerControl = new PlayerControl(dummyPlayer);
			HomePlayersGrid.Children.Clear();
			HomePlayersGrid.Children.Add(playerControl);
		}
	}
}
