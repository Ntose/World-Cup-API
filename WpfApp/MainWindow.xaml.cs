using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using DataLayer;
using DataLayer.Models;

namespace WpfApp
{
	public partial class MainWindow : Window, INotifyPropertyChanged
	{
		private DataManager dataManager;
		private Match currentMatch;
		private bool isInitializing = false;

		private List<Team> teams;
		public List<Team> Teams
		{
			get => teams;
			set
			{
				teams = value;
				OnPropertyChanged(nameof(Teams));
			}
		}

		private Team selectedTeam;
		public Team SelectedTeam
		{
			get => selectedTeam;
			set
			{
				if (selectedTeam != value)
				{
					selectedTeam = value;
					OnPropertyChanged(nameof(SelectedTeam));
					OnPropertyChanged(nameof(HasSelectedTeam));

					// Only update match result if not during initialization
					if (!isInitializing)
					{
						_ = UpdateMatchResult();
					}
				}
			}
		}

		private Team selectedOpponent;
		public Team SelectedOpponent
		{
			get => selectedOpponent;
			set
			{
				if (selectedOpponent != value)
				{
					selectedOpponent = value;
					OnPropertyChanged(nameof(SelectedOpponent));
					OnPropertyChanged(nameof(HasSelectedOpponent));

					// Only update match result if not during initialization
					if (!isInitializing)
					{
						_ = UpdateMatchResult();
					}
				}
			}
		}

		private List<Team> opponents;
		public List<Team> Opponents
		{
			get => opponents;
			set
			{
				opponents = value;
				OnPropertyChanged(nameof(Opponents));
			}
		}

		private bool isLoading;
		public bool IsLoading
		{
			get => isLoading;
			set
			{
				isLoading = value;
				OnPropertyChanged(nameof(IsLoading));
			}
		}

		// Add missing properties for binding
		public bool HasSelectedTeam => SelectedTeam != null;
		public bool HasSelectedOpponent => SelectedOpponent != null;

		// Add missing text properties for binding
		public string SettingsMenuText => "Settings";
		public string ChangeSettingsText => "Change Settings";
		public string ExitText => "Exit";
		public string SelectTeamText => "Select Team";
		public string SelectOpponentText => "Select Opponent";
		public string SelectedTeamInfoText => "Team Info";
		public string OpponentTeamInfoText => "Opponent Info";
		public string StatusText => "Ready";
		public string ChampionshipText => ConfigurationManager.SelectedChampionship ?? "World Cup";
		public string LoadingText => "Loading...";

		public MainWindow()
		{
			InitializeComponent();
			DataContext = this;

			// Add error handling for initialization
			Loaded += async (s, e) => {
				try
				{
					await InitializeAsync();
				}
				catch (Exception ex)
				{
					MessageBox.Show($"Error during initialization: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
					IsLoading = false;
				}
			};
		}

		public event PropertyChangedEventHandler PropertyChanged;

		protected void OnPropertyChanged(string name) =>
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

		private async Task InitializeAsync()
		{
			try
			{
				IsLoading = true;
				isInitializing = true;

				dataManager = new DataManager();

				var allTeams = await dataManager.GetTeamsAsync(ConfigurationManager.SelectedChampionship);
				Teams = allTeams.OrderBy(t => t.Country).ToList();

				SelectedTeam = Teams.FirstOrDefault(t => t.FifaCode == ConfigurationManager.SelectedTeam)
							   ?? Teams.FirstOrDefault();

				isInitializing = false;

				// Now update match result after initialization is complete
				if (SelectedTeam != null)
				{
					await UpdateMatchResult();
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show($"Error loading teams: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
				throw;
			}
			finally
			{
				IsLoading = false;
				isInitializing = false;
			}
		}

		private async Task UpdateMatchResult()
		{
			if (SelectedTeam == null || isInitializing) return;

			try
			{
				IsLoading = true;

				ConfigurationManager.SelectedTeam = SelectedTeam.FifaCode;

				var matches = await dataManager.GetMatchesAsync(ConfigurationManager.SelectedChampionship);

				// Fix the LINQ query - compare FifaCode with FifaCode, not Country with FifaCode
				Opponents = matches
					.Where(m => m.HomeTeam.FifaCode == SelectedTeam.FifaCode || m.AwayTeam.FifaCode == SelectedTeam.FifaCode)
					.Select(m => m.HomeTeam.FifaCode == SelectedTeam.FifaCode ? m.AwayTeam : m.HomeTeam)
					.Distinct()
					.OrderBy(t => t.Country)
					.ToList();

				if (Opponents.Count > 0 && SelectedOpponent == null)
					SelectedOpponent = Opponents[0];

				if (SelectedOpponent != null)
				{
					currentMatch = await dataManager.GetMatchBetweenTeamsAsync(
						ConfigurationManager.SelectedChampionship,
						SelectedTeam.FifaCode,
						SelectedOpponent.FifaCode);
				}

				DisplayMatchResult();

				// Add a small delay to ensure canvas is rendered before displaying lineups
				await Task.Delay(100);
				DisplayStartingLineups();
			}
			catch (Exception ex)
			{
				MessageBox.Show($"Error updating match result: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			}
			finally
			{
				IsLoading = false;
			}
		}

		private void DisplayMatchResult()
		{
			if (currentMatch == null)
			{
				lblResult.Text = string.Empty;
				return;
			}

			lblResult.Text = currentMatch.Status == "completed"
				? $"{currentMatch.HomeTeam.Country} {currentMatch.HomeTeamGoals} : {currentMatch.AwayTeamGoals} {currentMatch.AwayTeam.Country}"
				: $"{currentMatch.HomeTeam.Country} vs {currentMatch.AwayTeam.Country}";
		}

		private void DisplayStartingLineups()
		{
			fieldCanvas.Children.Clear();

			if (currentMatch?.HomeTeamStatistics?.StartingEleven == null ||
				currentMatch?.AwayTeamStatistics?.StartingEleven == null)
				return;

			// Force layout update to get actual canvas dimensions
			fieldCanvas.UpdateLayout();

			double canvasWidth = fieldCanvas.ActualWidth;
			double canvasHeight = fieldCanvas.ActualHeight;

			// If canvas still has no size, use default dimensions
			if (canvasWidth <= 0) canvasWidth = 800;
			if (canvasHeight <= 0) canvasHeight = 400;

			var homePositions = GenerateFormationPositions(currentMatch.HomeTeamStatistics.StartingEleven.Count, canvasWidth, canvasHeight, true);
			var awayPositions = GenerateFormationPositions(currentMatch.AwayTeamStatistics.StartingEleven.Count, canvasWidth, canvasHeight, false);

			PlacePlayersInPosition(currentMatch.HomeTeamStatistics.StartingEleven, homePositions, -50);
			PlacePlayersInPosition(currentMatch.AwayTeamStatistics.StartingEleven, awayPositions, 50);
		}

		private List<Point> GenerateFormationPositions(int playerCount, double canvasWidth, double canvasHeight, bool isHomeTeam)
		{
			var positions = new List<Point>();
			double spacingX = canvasWidth / (playerCount + 1);
			double startX = spacingX;

			double y = canvasHeight / 2;

			for (int i = 0; i < playerCount; i++)
			{
				double x = isHomeTeam ? startX + spacingX * i : canvasWidth - (startX + spacingX * i);
				positions.Add(new Point(x, y));
			}

			return positions;
		}

		private void PlacePlayersInPosition(List<Player> players, List<Point> positions, int yOffset)
		{
			for (int i = 0; i < players.Count && i < positions.Count; i++)
			{
				var player = players[i];
				var position = positions[i];

				// Create a simple representation instead of PlayerControl if it doesn't exist
				var playerControl = CreatePlayerControl(player);

				if (playerControl is FrameworkElement fe)
				{
					Canvas.SetLeft(playerControl, position.X - fe.Width / 2);
					Canvas.SetTop(playerControl, position.Y + yOffset - fe.Height / 2);

					fieldCanvas.Children.Add(playerControl);

					// Animate player drop-in
					var anim = new DoubleAnimation
					{
						From = -100,
						To = position.Y + yOffset - fe.Height / 2,
						Duration = TimeSpan.FromSeconds(0.5)
					};

					playerControl.BeginAnimation(Canvas.TopProperty, anim);
				}
				else
				{
					// Fallback positioning without animation
					Canvas.SetLeft(playerControl, position.X - 20);
					Canvas.SetTop(playerControl, position.Y + yOffset - 20);
					fieldCanvas.Children.Add(playerControl);
				}
			}
		}

		private UIElement CreatePlayerControl(Player player)
		{
			// For now, just use the fallback control since PlayerControl type is causing issues
			// You can replace this with proper PlayerControl creation once the type issues are resolved
			return CreateFallbackControl(player);

			/*
            // Uncomment this section once PlayerControl inheritance is fixed
            try
            {
                var playerControl = new PlayerControl(player)
                {
                    Width = 40,
                    Height = 40
                };

                playerControl.MouseLeftButtonDown += (s, e) =>
                {
                    try
                    {
                        var detailsWindow = new PlayerDetailsWindow(player);
                        detailsWindow.Owner = this;
                        detailsWindow.ShowDialog();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error showing player details: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                };

                return playerControl;
            }
            catch
            {
                return CreateFallbackControl(player);
            }
            */
		}

		private void PlayerControl_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			// This method will be called when PlayerControl is clicked
			// You can add your player details logic here if needed
		}

		private Button CreateFallbackControl(Player player)
		{
			var button = new Button
			{
				Content = player.Name,
				Width = 40,
				Height = 40,
				FontSize = 8
			};

			button.Click += (s, e) =>
			{
				MessageBox.Show($"Player: {player.Name}\nPosition: {player.Position}", "Player Info", MessageBoxButton.OK, MessageBoxImage.Information);
			};

			return button;
		}

		private void BtnTeamInfo_Click(object sender, RoutedEventArgs e)
		{
			if (SelectedTeam == null) return;

			try
			{
				var teamInfoWindow = new TeamInfoWindow(SelectedTeam);
				teamInfoWindow.Owner = this;
				teamInfoWindow.ShowDialog();
			}
			catch (Exception ex)
			{
				MessageBox.Show($"Error showing team info: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}

		private void BtnSettings_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				var settingsWindow = new SettingsWindow();
				settingsWindow.Owner = this;

				if (settingsWindow.ShowDialog() == true)
				{
					_ = InitializeAsync();
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show($"Error opening settings: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}

		private void CmbSelectedTeam_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (cmbSelectedTeam.SelectedItem is Team team)
			{
				SelectedTeam = team;
			}
		}

		private void CmbSelectedOpponent_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (cmbSelectedOpponent.SelectedItem is Team opponent)
			{
				SelectedOpponent = opponent;
			}
		}
	}
}