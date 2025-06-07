using DataLayer;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using WpfApp;
using WpfApp.ViewModels;

namespace WpfApp
{
	public partial class MainWindow : Window
	{
		private MainViewModel viewModel;

		public MainWindow()
		{
			InitializeComponent();
			viewModel = new MainViewModel();
			DataContext = viewModel;

			Loaded += async (s, e) =>
			{
				ConfigurationManager.LoadConfiguration();

				await viewModel.LoadTeams(); // Now awaitable!

				await LoadFavoriteTeamAsync(); // Will work properly now
			};

		}

		private void TestComboBoxSelection()
		{
			try
			{
				Console.WriteLine($"Teams loaded: {viewModel.Teams?.Count ?? 0}");

				// Test 1: Select by index using SelectedIndex
				if (cmbSelectedTeam.Items.Count > 0)
				{
					Console.WriteLine("Testing SelectedIndex = 0");
					cmbSelectedTeam.SelectedIndex = 0;
					Console.WriteLine($"Selected team after index: {viewModel.SelectedTeam?.Country}");
				}

				// Test 2: Select by setting ViewModel property directly
				if (viewModel.Teams?.Count > 1)
				{
					Console.WriteLine("Testing ViewModel selection (index 1)");
					viewModel.SelectedTeam = viewModel.Teams[1];
					Console.WriteLine($"Selected team after viewmodel: {viewModel.SelectedTeam?.Country}");
				}

				// Test 3: Select by FIFA code (similar to favorite team loading)
				var testTeam = viewModel.Teams?.FirstOrDefault(t => t.FifaCode != null);
				if (testTeam != null)
				{
					Console.WriteLine($"Testing selection by FIFA code: {testTeam.FifaCode}");
					viewModel.SelectedTeam = testTeam;
					Console.WriteLine($"Selected team after FIFA code: {viewModel.SelectedTeam?.Country}");
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Test error: {ex.Message}");
			}
		}

		private async Task LoadFavoriteTeamAsync()
		{
			try
			{
				// Use ConfigurationManager to get the favorite team code
				string fifaCode = ConfigurationManager.FavoriteTeamCode;

				if (!string.IsNullOrEmpty(fifaCode))
				{
					// Find the team with matching FIFA code and set it as selected
					var favoriteTeam = viewModel.Teams?.FirstOrDefault(t => t.FifaCode == fifaCode);
					if (favoriteTeam != null)
					{
						viewModel.SelectedTeam = favoriteTeam;

						// Load opponents for the selected team
						viewModel.LoadOpponentsForSelectedTeam();
					}
					else
					{
						MessageBox.Show($"Favorite team with FIFA code '{fifaCode}' not found in available teams.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
					}
				}
				// If no favorite team is set, that's normal - don't show any message
			}
			catch (Exception ex)
			{
				MessageBox.Show($"Error loading favorite team: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}

		private void CmbSelectedTeam_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (viewModel.SelectedTeam != null)
			{
				viewModel.LoadOpponentsForSelectedTeam();
			}
		}

		private void CmbSelectedOpponent_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			viewModel.UpdateMatchResult();
		}

		private void ChangeSettings_Click(object sender, RoutedEventArgs e)
		{
			var settingsWindow = new SettingsWindow();
			settingsWindow.ShowDialog();
			// Reload localized texts if necessary
			viewModel.ReloadTexts();

			// Reload configuration and favorite team in case championship changed
			ConfigurationManager.LoadConfiguration();
			_ = LoadFavoriteTeamAsync();
		}

		private void Exit_Click(object sender, RoutedEventArgs e)
		{
			Application.Current.Shutdown();
		}

		private void BtnSelectedTeamInfo_Click(object sender, RoutedEventArgs e)
		{
			if (viewModel.SelectedTeam != null)
			{
				var infoWindow = new TeamInfoWindow(viewModel.SelectedTeam);
				infoWindow.ShowDialog();
			}
		}

		private void BtnOpponentTeamInfo_Click(object sender, RoutedEventArgs e)
		{
			if (viewModel.SelectedOpponent != null)
			{
				var infoWindow = new TeamInfoWindow(viewModel.SelectedOpponent);
				infoWindow.ShowDialog();
			}
		}

		// Test methods for debugging ComboBox selection
		private void TestSelectByIndex(int index)
		{
			try
			{
				if (viewModel.Teams != null && index >= 0 && index < viewModel.Teams.Count)
				{
					Console.WriteLine($"Testing selection of index {index}");

					// Method 1: Direct ComboBox selection
					cmbSelectedTeam.SelectedIndex = index;

					// Method 2: ViewModel selection
					// viewModel.SelectedTeam = viewModel.Teams[index];

					Console.WriteLine($"Selected: {viewModel.SelectedTeam?.Country} ({viewModel.SelectedTeam?.FifaCode})");
				}
				else
				{
					Console.WriteLine($"Invalid index {index}. Available teams: {viewModel.Teams?.Count ?? 0}");
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error testing selection: {ex.Message}");
			}
		}

		private void TestSelectByFifaCode(string fifaCode)
		{
			try
			{
				var team = viewModel.Teams?.FirstOrDefault(t =>
					string.Equals(t.FifaCode, fifaCode, StringComparison.OrdinalIgnoreCase));

				if (team != null)
				{
					Console.WriteLine($"Found team with FIFA code {fifaCode}: {team.Country}");
					viewModel.SelectedTeam = team;
					Console.WriteLine($"Selection successful: {viewModel.SelectedTeam?.Country}");
				}
				else
				{
					Console.WriteLine($"Team with FIFA code '{fifaCode}' not found");
					Console.WriteLine("Available FIFA codes:");
					viewModel.Teams?.Take(5).ToList().ForEach(t =>
						Console.WriteLine($"  - {t.FifaCode}: {t.Country}"));
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error testing FIFA code selection: {ex.Message}");
			}
		}
	}
}