using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Newtonsoft.Json.Linq; // For manual JSON parsing
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
		// Store the full list of teams for use later (e.g. when retrieving opposing team codes).
		private List<Team> allTeams;

		public MainWindow()
		{
			InitializeComponent();
			// Load teams and match data on startup.
			LoadTeams();
			LoadMatchDataForSelectedTeam();
			HomeTeamComboBox.SelectionChanged += HomeTeamComboBox_SelectionChanged;
		}

		private async void LoadTeams()
		{
			var teams = await ApiService.GetTeamsAsync(ConfigurationManager.SelectedChampionship);
			allTeams = teams; // store for later lookup

			// Bind the full team objects to the ComboBox so that we can later access properties like FifaCode.
			HomeTeamComboBox.ItemsSource = teams.OrderBy(t => t.Country).ToList();
			HomeTeamComboBox.DisplayMemberPath = "Country";

			try
			{
				// If the configuration holds a favorite team, select it; otherwise select the first team.
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
				MessageBox.Show("Error selecting default team: " + ex.Message,
								"Selection Error", MessageBoxButton.OK, MessageBoxImage.Warning);
			}
		}

		private async void LoadMatchDataForSelectedTeam()
		{
			// Retrieve all matches for the championship.
			var matches = await ApiService.GetMatchesAsync(ConfigurationManager.SelectedChampionship);

			// Find a match based on the favorite team’s FIFA code.
			var match = matches.FirstOrDefault(m =>
				string.Equals(m.HomeTeam?.FifaCode, ConfigurationManager.FavoriteTeamCode, StringComparison.OrdinalIgnoreCase));

			if (match != null)
			{
				// Populate the opposing team ComboBox with the away team's country.
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

			// Retrieve match data.
			var matches = await ApiService.GetMatchesAsync(ConfigurationManager.SelectedChampionship);
			if (matches == null)
			{
				MessageBox.Show("No match data loaded.");
				return;
			}

			// Get the first match where the selected team's country appears as the home team.
			var match = matches.FirstOrDefault(m =>
				string.Equals(m.HomeTeam?.Country, selectedTeam.Country, StringComparison.OrdinalIgnoreCase));

			if (match != null)
			{
				var opponentTeams = matches
					.Where(m => string.Equals(m.HomeTeam?.Country, selectedTeam.Country, StringComparison.OrdinalIgnoreCase))
					.Select(m => m.AwayTeam?.Country)
					.Where(country => !string.IsNullOrEmpty(country))
					.Distinct()
					.OrderBy(country => country)
					.ToList();

				OpposingTeamComboBox.ItemsSource = opponentTeams;
				if (opponentTeams.Any())
					OpposingTeamComboBox.SelectedIndex = 0;

				// Simulate a random match result.
				Random random = new Random();
				int homeGoals = random.Next(0, 4);
				int awayGoals = random.Next(0, 4);
				MatchResultTextBlock.Text = $"{homeGoals} - {awayGoals}";
			}
			else
			{
				OpposingTeamComboBox.ItemsSource = null;
				MatchResultTextBlock.Text = "Match Result Error";
			}
		}

		private async void OpposingTeamComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			// This event updates any match result simulation whenever the opposing team selection changes.
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

			// Simulate random match result.
			Random random = new Random();
			int homeGoals = random.Next(0, 4);
			int awayGoals = random.Next(0, 4);
			MatchResultTextBlock.Text = $"{homeGoals} - {awayGoals}";
		}

		private async void LoadMatchPlayers_Click(object sender, RoutedEventArgs e)
		{
			// Load players for the home team.
			if (HomeTeamComboBox.SelectedItem == null)
				return;

			Team selectedTeam = HomeTeamComboBox.SelectedItem as Team;
			if (selectedTeam == null)
				return;

			string fifaCode = selectedTeam.FifaCode;
			var players = await GetHomeTeamPlayers(fifaCode);
			if (players == null)
				return;

			HomePlayersGrid.Children.Clear();
			foreach (var player in players)
			{
				var playerControl = new PlayerControl(player);
				HomePlayersGrid.Children.Add(playerControl);
			}

			//////////////////////////////////////
			// Now load players for the opposing team.
			// Because OpposingTeamComboBox is bound to a list of strings (country names), we first get the country.
			string oppCountry = OpposingTeamComboBox.SelectedItem?.ToString();
			if (string.IsNullOrEmpty(oppCountry))
				return;

			// Retrieve matches again.
			var matchesOpp = await ApiService.GetMatchesAsync(ConfigurationManager.SelectedChampionship);
			if (matchesOpp == null)
			{
				MessageBox.Show("No match data loaded.");
				return;
			}

			var matchOpp = matchesOpp.FirstOrDefault(m =>
				string.Equals(m.AwayTeam?.Country, oppCountry, StringComparison.OrdinalIgnoreCase));
			if (matchOpp == null)
			{
				MessageBox.Show("No match found for the opposing team.");
				return;
			}

			// Attempt to retrieve the away team's FIFA code.
			fifaCode = matchOpp.AwayTeam?.FifaCode;
			if (string.IsNullOrEmpty(fifaCode))
			{
				// As a fallback, try looking it up in the stored teams list.
				Team oppTeam = allTeams.FirstOrDefault(t =>
					string.Equals(t.Country, oppCountry, StringComparison.OrdinalIgnoreCase));
				if (oppTeam != null)
					fifaCode = oppTeam.FifaCode;
			}
			if (string.IsNullOrEmpty(fifaCode))
			{
				MessageBox.Show("Opposing team FIFA code not available.");
				return;
			}

			players = await GetHomeTeamPlayers(fifaCode);
			if (players == null)
				return;

			OpponentPlayersGrid.Children.Clear();
			foreach (var player in players)
			{
				var playerControl = new PlayerControl(player);
				OpponentPlayersGrid.Children.Add(playerControl);
			}
		}

		// Manually parse the JSON inside of MainWindow.xaml.cs.
		private async Task<List<Player>> GetHomeTeamPlayers(string fifaCode)
		{
			var settings = ConfigManager.LoadSettings();
			if (settings == null)
			{
				// Default settings when the file doesn't exist.
				settings = new AppSettings
				{
					Language = "en",
					Tournament = "men"
				};
			}

			try
			{
				string url = $"https://worldcup-vua.nullbit.hr/{settings.Tournament}/matches/country?fifa_code={fifaCode}";
				System.Diagnostics.Debug.WriteLine("Constructed URL: " + url);

				using (HttpClient client = new HttpClient())
				{
					string json = await client.GetStringAsync(url);
					System.Diagnostics.Debug.WriteLine("Raw JSON: " + json);

					// Parse JSON using JArray for manual processing.
					JArray jMatches = JArray.Parse(json);
					if (jMatches.Count == 0)
					{
						MessageBox.Show("No matches were returned from the API.");
						return null;
					}

					JObject matchFound = null;
					foreach (JObject jMatch in jMatches)
					{
						JObject jHomeTeam = jMatch["home_team"] as JObject;
						JObject jAwayTeam = jMatch["away_team"] as JObject;
						string homeTeamCode = jHomeTeam?["code"]?.ToString() ?? jHomeTeam?["fifa_code"]?.ToString();
						string awayTeamCode = jAwayTeam?["code"]?.ToString() ?? jAwayTeam?["fifa_code"]?.ToString();

						if (!string.IsNullOrEmpty(homeTeamCode))
							homeTeamCode = homeTeamCode.ToUpperInvariant();
						if (!string.IsNullOrEmpty(awayTeamCode))
							awayTeamCode = awayTeamCode.ToUpperInvariant();

						if (string.Equals(homeTeamCode, fifaCode, StringComparison.OrdinalIgnoreCase) ||
							string.Equals(awayTeamCode, fifaCode, StringComparison.OrdinalIgnoreCase))
						{
							matchFound = jMatch;
							break;
						}
					}

					if (matchFound == null)
					{
						MessageBox.Show("No match found for this team in the returned data.");
						return null;
					}

					JObject foundHomeTeam = matchFound["home_team"] as JObject;
					string foundHomeCode = foundHomeTeam?["code"]?.ToString() ?? foundHomeTeam?["fifa_code"]?.ToString();
					bool isHomeTeam = string.Equals(foundHomeCode, fifaCode, StringComparison.OrdinalIgnoreCase);

					JObject jTeamStatistics = isHomeTeam ?
						(matchFound["home_team_statistics"] as JObject) :
						(matchFound["away_team_statistics"] as JObject);

					if (jTeamStatistics == null)
					{
						MessageBox.Show("Team statistics not available.");
						return null;
					}

					JArray jStartingEleven = jTeamStatistics["starting_eleven"] as JArray;
					if (jStartingEleven == null)
					{
						MessageBox.Show("Starting eleven not available.");
						return null;
					}

					List<Player> players = new List<Player>();
					foreach (JObject jPlayer in jStartingEleven)
					{
						Player p = new Player
						{
							Name = jPlayer["name"]?.ToString(),
							ShirtNumber = jPlayer["shirt_number"] != null ? (int)jPlayer["shirt_number"] : 0,
							Position = jPlayer["position"]?.ToString(),
							Captain = jPlayer["captain"] != null ? (bool)jPlayer["captain"] : false,
							ImagePath = jPlayer["image_path"]?.ToString() ?? ""
						};
						players.Add(p);
					}
					return players;
				}
			}
			catch (HttpRequestException hre)
			{
				MessageBox.Show($"HTTP error: {hre.Message}", "HTTP Request Error", MessageBoxButton.OK, MessageBoxImage.Error);
				return null;
			}
			catch (Exception ex)
			{
				MessageBox.Show($"Error fetching players: {ex.Message}");
				return null;
			}
		}

		private void MainWindow_Loaded(object sender, RoutedEventArgs e)
		{
			// For testing purposes, add a dummy player control.
			var dummyPlayer = new Player
			{
				Name = "Test Player",
				ShirtNumber = 10,
				Position = "Midfield",
				Captain = true,
				ImagePath = ""
			};
			var playerControl = new PlayerControl(dummyPlayer);
			HomePlayersGrid.Children.Clear();
			HomePlayersGrid.Children.Add(playerControl);
		}
	}
}
