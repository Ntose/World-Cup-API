using System.Windows;
using DataLayer.Models;
using DataLayer;
using System.Linq;
using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Text.RegularExpressions;

namespace WpfApp
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private async void LoadTeams()
		{
			var teams = await ApiService.GetTeamsAsync(ConfigurationManager.SelectedChampionship);
			HomeTeamComboBox.ItemsSource = teams
			.OrderBy(team => team.Country) // Sort teams alphabetically by Country
			.Select(team => $"{team.Country}")
			.ToList();
			try
			{
				if (HomeTeamComboBox.Items.Contains(ConfigurationManager.SelectedTeam))
				{
					HomeTeamComboBox.SelectedItem = ConfigurationManager.FavoriteTeam;
				}
				else
				{
					HomeTeamComboBox.SelectedIndex = 0;
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show($"Error selecting default team: {ex.Message}", "Selection Error", MessageBoxButton.OK, MessageBoxImage.Warning);
			}

		}
		private async void LoadMatchDataForSelectedTeam()
		{
			// Retrieve all matches for the selected championship.
			var matches = await ApiService.GetMatchesAsync(ConfigurationManager.SelectedChampionship);

			// For example, select the first match where the home team is the one configured.
			// (Make sure your ConfigurationManager holds a property like SelectedTeamFifaCode that you can compare.)
			var match = matches.FirstOrDefault(m =>
			string.Equals(m.HomeTeam?.FifaCode, ConfigurationManager.FavoriteTeamCode, StringComparison.OrdinalIgnoreCase));


			// If a match is found, extract the away team and populate the ComboBox.
			if (match != null)
			{
				OpposingTeamComboBox.ItemsSource = new List<string>
		{
			$"{match.AwayTeam.Country}"
		};
				OpposingTeamComboBox.SelectedIndex = 0;
			}
			else
			{
				MessageBox.Show("No match available for the selected team");
			}
		}




		private async void HomeTeamComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			// Ensure a selection exists.
			if (HomeTeamComboBox.SelectedItem == null)
				return;

			// Get the selected home team name.
			string selectedHomeTeam = HomeTeamComboBox.SelectedItem.ToString();

			// Retrieve all matches for the championship.
			var matches = await ApiService.GetMatchesAsync(ConfigurationManager.SelectedChampionship);
			if (matches == null)
			{
				MessageBox.Show("No match data loaded.");
				return;
			}

			// Filter to select the first match with the selected home team.
			var match = matches.FirstOrDefault(m =>
				string.Equals(m.HomeTeam?.Country, selectedHomeTeam, StringComparison.OrdinalIgnoreCase));

			if (match != null)
			{
				// Update the opponent teams combobox.
				var opponentTeams = matches
					.Where(m =>
						string.Equals(m.HomeTeam?.Country, selectedHomeTeam, StringComparison.OrdinalIgnoreCase))
					.Select(m => m.AwayTeam?.Country)
					.Where(awayCountry => !string.IsNullOrEmpty(awayCountry))
					.Distinct()
					.OrderBy(awayCountry => awayCountry)
					.ToList();

				OpposingTeamComboBox.ItemsSource = opponentTeams;
				if (opponentTeams.Any())
				{
					OpposingTeamComboBox.SelectedIndex = 0;
				}

				// Update the match result display with the actual goal numbers.
				// This assumes the match object includes HomeTeamGoals and AwayTeamGoals properties.
				int homeGoals = match.HomeTeamGoals;
				int awayGoals = match.AwayTeamGoals;
				MatchResultTextBlock.Text = $"Match Result: {homeGoals} - {awayGoals}";
			}
			else
			{
				// Clear opponent combo box and reset match result if no match is found.
				OpposingTeamComboBox.ItemsSource = null;
				MatchResultTextBlock.Text = "Match Result Error";
			}
		}



		public MainWindow()
		{
			InitializeComponent();
			LoadTeams();
			LoadMatchDataForSelectedTeam();
			HomeTeamComboBox.SelectionChanged += HomeTeamComboBox_SelectionChanged;
		}


	}
}
