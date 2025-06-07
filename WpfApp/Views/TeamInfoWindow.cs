using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using DataLayer.Models;

namespace WpfApp
{
	public partial class TeamInfoWindow : Window
	{
		private Team team;

		public TeamInfoWindow(Team team)
		{
			this.team = team ?? throw new ArgumentNullException(nameof(team));
			InitializeComponent();
			LoadTeamData();
		}

		private void InitializeComponent()
		{
			Title = "Team Information";
			Width = 600;
			Height = 700;
			ResizeMode = ResizeMode.CanResize;
			WindowStartupLocation = WindowStartupLocation.CenterOwner;
			Background = new SolidColorBrush(Color.FromRgb(248, 249, 250));

			var mainGrid = new Grid();
			mainGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
			mainGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
			mainGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

			// Header section
			var headerPanel = CreateHeaderPanel();
			Grid.SetRow(headerPanel, 0);

			// Content section with tabs
			var tabControl = CreateTabControl();
			Grid.SetRow(tabControl, 1);

			// Button section
			var buttonPanel = CreateButtonPanel();
			Grid.SetRow(buttonPanel, 2);

			mainGrid.Children.Add(headerPanel);
			mainGrid.Children.Add(tabControl);
			mainGrid.Children.Add(buttonPanel);

			Content = mainGrid;
		}

		private StackPanel CreateHeaderPanel()
		{
			var headerPanel = new StackPanel
			{
				Orientation = Orientation.Vertical,
				Margin = new Thickness(20),
				Background = new SolidColorBrush(Colors.Navy)
			};

			var countryLabel = new Label
			{
				Content = "Team Name",
				FontSize = 28,
				FontWeight = FontWeights.Bold,
				Foreground = new SolidColorBrush(Colors.White),
				HorizontalAlignment = HorizontalAlignment.Center,
				Padding = new Thickness(15),
				Tag = "CountryLabel"
			};

			var fifaCodeLabel = new Label
			{
				Content = "FIFA Code",
				FontSize = 16,
				Foreground = new SolidColorBrush(Colors.LightGray),
				HorizontalAlignment = HorizontalAlignment.Center,
				Padding = new Thickness(5),
				Tag = "FifaCodeLabel"
			};

			headerPanel.Children.Add(countryLabel);
			headerPanel.Children.Add(fifaCodeLabel);

			return headerPanel;
		}

		private TabControl CreateTabControl()
		{
			var tabControl = new TabControl
			{
				Margin = new Thickness(20, 0, 20, 0)
			};

			// Team Details Tab
			var detailsTab = new TabItem
			{
				Header = "Team Details"
			};

			var detailsContent = new ScrollViewer
			{
				VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
				Padding = new Thickness(10)
			};

			var detailsPanel = new StackPanel
			{
				Orientation = Orientation.Vertical,
				Tag = "DetailsPanel"
			};

			detailsPanel.Children.Add(CreateInfoSection("Basic Information"));
			detailsPanel.Children.Add(CreateDetailRow("Country:", ""));
			detailsPanel.Children.Add(CreateDetailRow("FIFA Code:", ""));

			detailsContent.Content = detailsPanel;
			detailsTab.Content = detailsContent;

			// Statistics Tab (if available)
			var statsTab = new TabItem
			{
				Header = "Statistics"
			};

			var statsContent = new ScrollViewer
			{
				VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
				Padding = new Thickness(10)
			};

			var statsPanel = new StackPanel
			{
				Orientation = Orientation.Vertical,
				Tag = "StatsPanel"
			};

			statsPanel.Children.Add(CreateInfoSection("Team Statistics"));
			statsPanel.Children.Add
				(CreateDetailRow("Games Played:", team.GamesPlayed.ToString()));
			statsPanel.Children.Add(CreateDetailRow("Wins:", team.Wins.ToString()));
			statsPanel.Children.Add(CreateDetailRow("Draws:", team.Draws.ToString()));
			statsPanel.Children.Add(CreateDetailRow("Losses:", team.Losses.ToString()));
			statsPanel.Children.Add(CreateDetailRow("Goals For:", team.GoalsFor.ToString()));
			statsPanel.Children.Add(CreateDetailRow("Goals Against:", team.GoalsAgainst.ToString()));
			statsPanel.Children.Add(CreateDetailRow("Goal Differential:", team.GoalDifferential.ToString()));
			statsPanel.Children.Add(CreateDetailRow("Points:", team.Points.ToString()));
			statsContent.Content = statsPanel;
			statsTab.Content = statsContent;

			tabControl.Items.Add(detailsTab);
			tabControl.Items.Add(statsTab);

			return tabControl;
		}

		private Label CreateInfoSection(string title)
		{
			return new Label
			{
				Content = title,
				FontSize = 18,
				FontWeight = FontWeights.Bold,
				Foreground = new SolidColorBrush(Colors.Navy),
				Margin = new Thickness(0, 10, 0, 5),
				Background = new SolidColorBrush(Color.FromRgb(230, 235, 240)),
				Padding = new Thickness(10, 5, 10, 5)
			};
		}

		private Grid CreateDetailRow(string label, string value)
		{
			var grid = new Grid
			{
				Margin = new Thickness(10, 5, 10, 5)
			};
			grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(150) });
			grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

			var labelControl = new Label
			{
				Content = label,
				FontWeight = FontWeights.SemiBold,
				VerticalAlignment = VerticalAlignment.Center,
				Foreground = new SolidColorBrush(Colors.DarkSlateGray)
			};
			Grid.SetColumn(labelControl, 0);

			var valueControl = new Label
			{
				Content = value,
				VerticalAlignment = VerticalAlignment.Center,
				Tag = label, // Store label for identification
				Background = new SolidColorBrush(Colors.White),
				Padding = new Thickness(8, 4, 8, 4),
				BorderBrush = new SolidColorBrush(Colors.LightGray),
				BorderThickness = new Thickness(1)
			};
			Grid.SetColumn(valueControl, 1);

			grid.Children.Add(labelControl);
			grid.Children.Add(valueControl);

			return grid;
		}

		private StackPanel CreateButtonPanel()
		{
			var buttonPanel = new StackPanel
			{
				Orientation = Orientation.Horizontal,
				HorizontalAlignment = HorizontalAlignment.Right,
				Margin = new Thickness(20, 10, 20, 20)
			};

			var closeButton = new Button
			{
				Content = "Close",
				Width = 100,
				Height = 35,
				Background = new SolidColorBrush(Colors.Navy),
				Foreground = new SolidColorBrush(Colors.White),
				FontWeight = FontWeights.SemiBold,
				Margin = new Thickness(5, 0, 0, 0)
			};
			closeButton.Click += CloseButton_Click;

			buttonPanel.Children.Add(closeButton);

			return buttonPanel;
		}

		private void LoadTeamData()
		{
			if (team == null) return;

			// Update header
			UpdateHeaderLabels();

			// Update details panel
			UpdateDetailsPanel();

			// Set window title
			Title = $"Team Information - {team.Country ?? "Unknown Team"}";
		}

		private void UpdateHeaderLabels()
		{
			var headerPanel = ((Grid)Content).Children[0] as StackPanel;
			if (headerPanel != null)
			{
				foreach (Label label in headerPanel.Children.OfType<Label>())
				{
					switch (label.Tag?.ToString())
					{
						case "CountryLabel":
							label.Content = team.Country ?? "Unknown Team";
							break;
						case "FifaCodeLabel":
							label.Content = $"FIFA Code: {team.FifaCode ?? "N/A"}";
							break;
					}
				}
			}
		}

		private void UpdateDetailsPanel()
		{
			var tabControl = ((Grid)Content).Children[1] as TabControl;
			if (tabControl?.Items[0] is TabItem detailsTab)
			{
				var scrollViewer = detailsTab.Content as ScrollViewer;
				var detailsPanel = (scrollViewer?.Content as StackPanel);

				if (detailsPanel != null)
				{
					foreach (var child in detailsPanel.Children)
					{
						if (child is Grid row && row.Children.Count >= 2 && row.Children[1] is Label valueLabel)
						{
							var labelText = valueLabel.Tag?.ToString();
							switch (labelText)
							{
								case "Country:":
									valueLabel.Content = team.Country ?? "Unknown";
									break;
								case "FIFA Code:":
									valueLabel.Content = team.FifaCode ?? "N/A";
									break;
							}
						}
					}
				}
			}
		}

		private void CloseButton_Click(object sender, RoutedEventArgs e)
		{
			Close();
		}
	}
}