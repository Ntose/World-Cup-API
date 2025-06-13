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
using System.Windows.Input;
using System.ComponentModel;

namespace WpfApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // Store all teams so we can look up team info later.
        private List<Team> allTeams;


		public MainWindow()
        {
            InitializeComponent();
            LoadTeams();
			this.Closing += MainWindow_Closing;
			LoadMatchDataForSelectedTeam();
            HomeTeamComboBox.SelectionChanged += HomeTeamComboBox_SelectionChanged;
        }
		private void MainWindow_Closing(object sender, CancelEventArgs e)
		{
			var confirmWindow = new ConfirmCloseWindow();
			// Optionally, set the owner:
			confirmWindow.Owner = this;
			bool? result = confirmWindow.ShowDialog();
			if (result != true)
			{
				// User cancelled exit.
				e.Cancel = true;
			}
		}
		private async void LoadTeams()
        {
            var teams = await ApiService.GetTeamsAsync(ConfigurationManager.SelectedChampionship);
            allTeams = teams; // store for later lookup

            // Bind the full team objects to the ComboBox.
            HomeTeamComboBox.ItemsSource = teams.OrderBy(t => t.Country).ToList();
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

            Team selectedTeam = HomeTeamComboBox.SelectedItem as Team;
            if (selectedTeam == null)
            {
                MessageBox.Show("Invalid team selection.");
                return;
            }

            var matches = await ApiService.GetMatchesAsync(ConfigurationManager.SelectedChampionship);
            if (matches == null)
            {
                MessageBox.Show("No match data loaded.");
                return;
            }

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
            int homeGoals = random.Next(0, 4);
            int awayGoals = random.Next(0, 4);
            MatchResultTextBlock.Text = $"{homeGoals} - {awayGoals}";
        }

		private async void LoadMatchPlayers_Click(object sender, RoutedEventArgs e)
		{
			// ------------------------------
			// Load Home Team Players
			// ------------------------------
			if (HomeTeamComboBox.SelectedItem == null)
				return;

			Team homeTeam = HomeTeamComboBox.SelectedItem as Team;
			if (homeTeam == null)
				return;

			string homeFifaCode = homeTeam.FifaCode;
			var homePlayers = await GetHomeTeamPlayers(homeFifaCode);
			if (homePlayers == null)
				return;

			HomePlayersGrid.Children.Clear();
			foreach (var player in homePlayers)
			{
				// Create a player control (assumes a PlayerControl UserControl)
				var playerControl = new PlayerControl(player);
				// Set DataContext so that bindings work
				playerControl.DataContext = player;
				// Attach a click event so that selecting the player opens the PlayerInfoWindow
				playerControl.MouseLeftButtonUp += PlayerControl_MouseLeftButtonUp;
				HomePlayersGrid.Children.Add(playerControl);
			}

			// ------------------------------
			// Load Opposing Team Players
			// ------------------------------
			// Get the selected opposing team name from the OpposingTeamComboBox (items are assumed to be country names)
			string oppCountry = OpposingTeamComboBox.SelectedItem?.ToString();
			if (string.IsNullOrWhiteSpace(oppCountry))
				return;

			// Retrieve matches for the selected championship.
			var matches = await ApiService.GetMatchesAsync(ConfigurationManager.SelectedChampionship);
			if (matches == null)
			{
				MessageBox.Show("No match data loaded.");
				return;
			}

			// Find the match where the AwayTeam's Country equals the selected opposing team.
			var matchOpp = matches.FirstOrDefault(m =>
				string.Equals(m.AwayTeam?.Country, oppCountry, StringComparison.OrdinalIgnoreCase));
			if (matchOpp == null)
			{
				MessageBox.Show("No match found for the opposing team.");
				return;
			}

			// Extract the opposing team's FIFA code.
			string oppFifaCode = matchOpp.AwayTeam?.FifaCode;
			if (string.IsNullOrEmpty(oppFifaCode))
			{
				// Fallback: look up in the stored global team list.
				Team oppTeam = allTeams.FirstOrDefault(t =>
					string.Equals(t.Country, oppCountry, StringComparison.OrdinalIgnoreCase));
				if (oppTeam != null)
					oppFifaCode = oppTeam.FifaCode;
			}
			if (string.IsNullOrEmpty(oppFifaCode))
			{
				MessageBox.Show("Opposing team FIFA code not available.");
				return;
			}

			var oppPlayers = await GetHomeTeamPlayers(oppFifaCode);
			if (oppPlayers == null)
				return;

			OpponentPlayersGrid.Children.Clear();
			foreach (var player in oppPlayers)
			{
				var playerControl = new PlayerControl(player);
				playerControl.DataContext = player;
				playerControl.MouseLeftButtonUp += PlayerControl_MouseLeftButtonUp;
				OpponentPlayersGrid.Children.Add(playerControl);
			}
		}

		private void PlayerControl_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			// Cast the sender to your PlayerControl
			var control = sender as PlayerControl;
			if (control == null) return;

			// Retrieve the player data from the DataContext
			var selectedPlayer = control.DataContext as Player;
			if (selectedPlayer == null) return;

			// Create and show the PlayerInfoWindow, passing the selected player's data
			var playerInfoWindow = new PlayerInfoWindow(selectedPlayer);
			playerInfoWindow.Owner = this;
			playerInfoWindow.Show();
		}


		// New event handler for showing Home Team Info.
		private void HomeTeamInfoButton_Click(object sender, RoutedEventArgs e)
        {
            if (HomeTeamComboBox.SelectedItem is Team homeTeam)
            {
                // For demo purposes, using dummy statistics.
                var teamInfo = new TeamInfo
                {
                    Name = homeTeam.Country,
                    FifaCode = homeTeam.FifaCode,
                    Games = homeTeam.GamesPlayed,
                    Wins = homeTeam.Wins,
                    Losses = homeTeam.Losses,
                    Draws = homeTeam.Draws,
                    GoalsFor = homeTeam.GoalsFor,
                    GoalsAgainst = homeTeam.GoalsAgainst
                };
                var infoWindow = new TeamInfoWindow(teamInfo);
                infoWindow.Owner = this;
                infoWindow.Show();
            }
        }
		private void SettingsPanel_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			// Open the configuration window modally.
			var configWindow = new ConfigWindow();
			bool? result = configWindow.ShowDialog();
			if (result == true)
			{
				// User confirmed new settings—reload configuration and update the window display.
				ConfigurationManager.LoadConfiguration();

				if (ConfigurationManager.IsFullscreen())
				{
					this.WindowState = WindowState.Maximized;
					this.WindowStyle = WindowStyle.None;
				}
				else
				{
					// Parse WindowSize assuming the format "WIDTHxHEIGHT"
					var parts = ConfigurationManager.WindowSize.Split('x');
					if (parts.Length == 2 &&
						int.TryParse(parts[0], out int width) &&
						int.TryParse(parts[1], out int height))
					{
						this.Width = width;
						this.Height = height;
					}
					this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
					this.WindowStyle = WindowStyle.SingleBorderWindow;
					this.WindowState = WindowState.Normal;
				}

				// Optionally, update additional UI elements or language settings here.
			}
		}

		// New event handler for showing Opponent Team Info.
		private void OpponentTeamInfoButton_Click(object sender, RoutedEventArgs e)
        {
            if (OpposingTeamComboBox.SelectedItem != null)
            {
                string oppCountry = OpposingTeamComboBox.SelectedItem.ToString();
                var oppTeam = allTeams.FirstOrDefault(t =>
                    string.Equals(t.Country, oppCountry, StringComparison.OrdinalIgnoreCase));
                if (oppTeam != null)
                {
                    var teamInfo = new TeamInfo
                    {
                        Name = oppTeam.Country,
                        FifaCode = oppTeam.FifaCode,
                        Games = oppTeam.GamesPlayed,
                        Wins = oppTeam.Wins,
                        Losses = oppTeam.Losses,
                        Draws = oppTeam.Draws,
                        GoalsFor = oppTeam.GoalsFor,
                        GoalsAgainst = oppTeam.GoalsAgainst
                    };
                    var infoWindow = new TeamInfoWindow(teamInfo);
                    infoWindow.Owner = this;
                    infoWindow.Show();
                }
            }
        }

        // Manually parse the JSON inside of MainWindow.xaml.cs.
        private async Task<List<Player>> GetHomeTeamPlayers(string fifaCode)
        {
            var settings = ConfigManager.LoadSettings();
            if (settings == null)
            {
                // Default settings when the file doesn't exist.
                settings = new WorldCupStats.WinFormsApp.Helpers.AppSettings
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
                    return players.OrderBy(p => p.ShirtNumber).ToList();
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
