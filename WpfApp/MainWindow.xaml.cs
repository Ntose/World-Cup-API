using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text; // Added for potential future use, e.g., logging
using System.Threading.Tasks; // Added for Task if any new async operations are introduced
using System.Windows;
using System.Windows.Controls; // Required for SelectionChangedEventArgs
using DataLayer.Models;
using DataLayer;

namespace WpfApp
{
	public partial class MainWindow : Window
	{
		private readonly DataManager _dataManager = new DataManager();
		private List<Team> _teams;
		private List<Match> _matches;
		private string _selectedGender;
		private string _selectedLanguage; // Assuming language might be part of settings

		// Path for storing the favorite team
		private readonly string FavoriteTeamFilePath = Path.Combine(
			Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
			"WorldCupApp", // Suggest using a dedicated app folder
			"favorite_team.txt");

		// Path for settings
		private readonly string SettingsFilePath = Path.Combine(
			Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
			"WorldCupApp",
			"settings.txt");


		public MainWindow()
		{
			InitializeComponent();
			EnsureAppSettingsDirectory(); // Ensure directory for settings files exists
			LoadInitialDataAsync();
		}

		private void EnsureAppSettingsDirectory()
		{
			// Ensure the directory for settings and favorite team files exists
			// This prevents errors if the directory isn't manually created
			var appDataDir = Path.GetDirectoryName(SettingsFilePath);
			if (appDataDir != null && !Directory.Exists(appDataDir))
			{
				Directory.CreateDirectory(appDataDir);
			}
		}

		private async void LoadInitialDataAsync()
		{
			try
			{
				// Robustly read settings
				if (!File.Exists(SettingsFilePath))
				{
					// Handle missing settings file, perhaps by prompting user or using defaults
					// For now, we can create a default one or show an error.
					// Example: File.WriteAllText(SettingsFilePath, "women;en"); // Default to women, English
					MessageBox.Show($"Settings file not found at {SettingsFilePath}. Please create it (e.g., 'women;en') or set up initial configuration.", "Settings Missing", MessageBoxButton.OK, MessageBoxImage.Warning);
					return; // Exit if settings are crucial and missing
				}

				var settingsContent = File.ReadAllText(SettingsFilePath);
				var settings = settingsContent.Split(';');
				if (settings.Length < 1 || string.IsNullOrWhiteSpace(settings[0]))
				{
					MessageBox.Show("Gender not specified or invalid in settings.txt.", "Configuration Error", MessageBoxButton.OK, MessageBoxImage.Error);
					return;
				}
				_selectedGender = settings[0].Trim().ToLower();
				// _selectedLanguage = settings.Length > 1 ? settings[1].Trim().ToLower() : "en"; // Example: Handle language

				// Performance Note:
				// Your DataManager.GetTeamsAsync internally calls GetMatchesAsync.
				// Then you call _dataManager.GetMatchesAsync again.
				// This means match data is fetched twice from the API during startup.
				// Consider refactoring DataManager or how you call it to fetch matches only once.
				// For example:
				// _matches = await _dataManager.GetMatchesAsync(_selectedGender);
				// _teams = _dataManager.ExtractTeamsFromMatches(_matches); // (You'd need to add ExtractTeamsFromMatches to DataManager)

				_teams = await _dataManager.GetTeamsAsync(_selectedGender);
				_matches = await _dataManager.GetMatchesAsync(_selectedGender);

				if (_teams == null || !_teams.Any())
				{
					MessageBox.Show($"No teams found for the selected gender: '{_selectedGender}'. Check API or settings.", "Data Error", MessageBoxButton.OK, MessageBoxImage.Warning);
					FavoriteTeamComboBox.ItemsSource = null; // Clear combobox
					return;
				}
				if (_matches == null) // Matches could legitimately be empty if no matches played, but null is an issue
				{
					MessageBox.Show($"Match data could not be loaded for the selected gender: '{_selectedGender}'.", "Data Error", MessageBoxButton.OK, MessageBoxImage.Warning);
					_matches = new List<Match>(); // Initialize to empty list to prevent null reference errors later
				}


				var sortedTeams = _teams.OrderBy(t => t.Country).ToList();

				FavoriteTeamComboBox.ItemsSource = sortedTeams;
				FavoriteTeamComboBox.DisplayMemberPath = "Country"; // Assumes Team class has a 'Country' property

				// Pre-select favorite team by FIFA code
				if (File.Exists(FavoriteTeamFilePath))
				{
					var favoriteCode = File.ReadAllText(FavoriteTeamFilePath).Trim().ToUpperInvariant();
					if (!string.IsNullOrEmpty(favoriteCode))
					{
						var favoriteTeam = sortedTeams.FirstOrDefault(t => t.FifaCode.ToUpperInvariant() == favoriteCode);
						if (favoriteTeam != null)
						{
							FavoriteTeamComboBox.SelectedItem = favoriteTeam;
						}
						else
						{
							// Favorite team code from file not found in the current list of teams
							// Maybe clear favorite_team.txt or notify user?
							// For now, it will just not pre-select.
						}
					}
				}
			}
			catch (IOException ex)
			{
				MessageBox.Show($"File access error during initial load: {ex.Message}\nMake sure settings.txt and favorite_team.txt are accessible.", "File Error", MessageBoxButton.OK, MessageBoxImage.Error);
			}
			catch (System.Net.Http.HttpRequestException ex)
			{
				MessageBox.Show($"Network error fetching data: {ex.Message}. Please check your internet connection and the API endpoint.", "Network Error", MessageBoxButton.OK, MessageBoxImage.Error);
			}
			catch (Exception ex) // Catch-all for other unexpected errors
			{
				MessageBox.Show($"An unexpected error occurred during initial data load: {ex.Message}", "Initialization Error", MessageBoxButton.OK, MessageBoxImage.Error);
				// Log ex.ToString() for detailed debugging
			}
		}

		// Changed from async void to void as no await is used.
		private void FavoriteTeamComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			var team1 = FavoriteTeamComboBox.SelectedItem as Team;
			if (team1 == null)
			{
				MatchResultTextBlock.Text = "";
				OpponentTeamComboBox.ItemsSource = null;
				OpponentTeamComboBox.SelectedItem = null;
				// Save the cleared favorite team selection
				try
				{
					if (File.Exists(FavoriteTeamFilePath)) File.Delete(FavoriteTeamFilePath);
				}
				catch (IOException ex)
				{
					// Handle error (e.g., log it, show a non-critical message)
					System.Diagnostics.Debug.WriteLine($"Could not clear favorite team file: {ex.Message}");
				}
				return;
			}

			// Save the selected favorite team's FIFA code
			try
			{
				File.WriteAllText(FavoriteTeamFilePath, team1.FifaCode);
			}
			catch (IOException ex)
			{
				// Handle error (e.g., log it, show a non-critical message)
				MessageBox.Show($"Could not save favorite team: {ex.Message}", "File Error", MessageBoxButton.OK, MessageBoxImage.Warning);
			}


			// Find opponent codes from the already loaded _matches list
			var opponentCodes = _matches
				.Where(m => m.HomeTeam.FifaCode == team1.FifaCode || m.AwayTeam.FifaCode == team1.FifaCode)
				.Select(m => m.HomeTeam.FifaCode == team1.FifaCode ? m.AwayTeam.FifaCode : m.HomeTeam.FifaCode)
				.Distinct()
				.ToList();

			var opponents = _teams
				.Where(t => opponentCodes.Contains(t.FifaCode))
				.OrderBy(t => t.Country)
				.ToList();

			OpponentTeamComboBox.ItemsSource = opponents;
			OpponentTeamComboBox.DisplayMemberPath = "Country"; // Assumes Team class has a 'Country' property
			OpponentTeamComboBox.SelectedIndex = -1; // Clear previous opponent selection
			MatchResultTextBlock.Text = ""; // Reset result text
		}

		// --- CRITICAL FIX HERE ---
		// This method no longer needs to be async because we are querying data already in memory.
		// Calling _dataManager.GetMatchBetweenTeamsAsync here was inefficient as it would
		// re-fetch ALL matches from the internet every time an opponent was selected.
		private void OpponentTeamComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			var team1 = FavoriteTeamComboBox.SelectedItem as Team; // Favorite team
			var team2 = OpponentTeamComboBox.SelectedItem as Team; // Opponent team

			if (team1 == null || team2 == null) // Don't proceed if either team isn't selected
			{
				MatchResultTextBlock.Text = ""; // Clear result if selection is incomplete
				return;
			}

			// It's possible for team1 and team2 to be the same if the logic in FavoriteTeamComboBox_SelectionChanged
			// doesn't exclude the favorite team from the opponent list.
			// Current logic seems to correctly list only *other* teams as opponents.
			// if (team1.FifaCode == team2.FifaCode) return; // This check is technically redundant if opponent list is always different.

			// --- THE FIX ---
			// Instead of calling: await _dataManager.GetMatchBetweenTeamsAsync(...)
			// Query the _matches list that is already loaded into memory.
			var match = _matches.FirstOrDefault(m =>
				(m.HomeTeam.FifaCode == team1.FifaCode && m.AwayTeam.FifaCode == team2.FifaCode) ||
				(m.HomeTeam.FifaCode == team2.FifaCode && m.AwayTeam.FifaCode == team1.FifaCode));

			if (match == null)
			{
				MatchResultTextBlock.Text = "No match found between these teams.";
				return;
			}

			int score1, score2;

			// Determine which score belongs to team1 (the favorite team)
			if (match.HomeTeam.FifaCode == team1.FifaCode)
			{
				score1 = match.HomeTeamGoals;
				score2 = match.AwayTeamGoals;
			}
			else // team1 must have been the AwayTeam in this match
			{
				score1 = match.AwayTeamGoals;
				score2 = match.HomeTeamGoals;
			}

			MatchResultTextBlock.Text = $"{team1.Country} {score1} : {score2} {team2.Country}";
		}
	}
}