using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using DataLayer.Models;

namespace WpfApp
{
	public partial class PlayerDetailsWindow : Window
	{
		private Player player;

		public PlayerDetailsWindow(Player player)
		{
			this.player = player ?? throw new ArgumentNullException(nameof(player));
			InitializeComponent();
			LoadPlayerData();
		}

		private void InitializeComponent()
		{
			Title = "Player Details";
			Width = 400;
			Height = 500;
			ResizeMode = ResizeMode.NoResize;
			WindowStartupLocation = WindowStartupLocation.CenterOwner;
			Background = new SolidColorBrush(Color.FromRgb(240, 240, 240));

			var mainGrid = new Grid();
			mainGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
			mainGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
			mainGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

			// Header section
			var headerPanel = new StackPanel
			{
				Orientation = Orientation.Vertical,
				Margin = new Thickness(20, 20, 20, 10),
				Background = new SolidColorBrush(Colors.DodgerBlue)
			};

			var nameLabel = new Label
			{
				Content = "Player Name",
				FontSize = 24,
				FontWeight = FontWeights.Bold,
				Foreground = new SolidColorBrush(Colors.White),
				HorizontalAlignment = HorizontalAlignment.Center,
				Padding = new Thickness(10)
			};

			headerPanel.Children.Add(nameLabel);
			Grid.SetRow(headerPanel, 0);

			// Details section
			var detailsScrollViewer = new ScrollViewer
			{
				Margin = new Thickness(20, 10, 20, 10),
				VerticalScrollBarVisibility = ScrollBarVisibility.Auto
			};

			var detailsPanel = new StackPanel
			{
				Orientation = Orientation.Vertical
			};

			// Add detail fields
			detailsPanel.Children.Add(CreateDetailRow("Position:", ""));
			detailsPanel.Children.Add(CreateDetailRow("Jersey Number:", ""));
			detailsPanel.Children.Add(CreateDetailRow("Captain:", ""));

			detailsScrollViewer.Content = detailsPanel;
			Grid.SetRow(detailsScrollViewer, 1);

			// Button section
			var buttonPanel = new StackPanel
			{
				Orientation = Orientation.Horizontal,
				HorizontalAlignment = HorizontalAlignment.Right,
				Margin = new Thickness(20, 10, 20, 20)
			};

			var closeButton = new Button
			{
				Content = "Close",
				Width = 80,
				Height = 30,
				Margin = new Thickness(5, 0, 0, 0)
			};
			closeButton.Click += CloseButton_Click;

			buttonPanel.Children.Add(closeButton);
			Grid.SetRow(buttonPanel, 2);

			mainGrid.Children.Add(headerPanel);
			mainGrid.Children.Add(detailsScrollViewer);
			mainGrid.Children.Add(buttonPanel);

			Content = mainGrid;

			// Store references for data loading
			Tag = new
			{
				NameLabel = nameLabel,
				DetailsPanel = detailsPanel
			};
		}

		private Grid CreateDetailRow(string label, string value)
		{
			var grid = new Grid
			{
				Margin = new Thickness(0, 5, 0, 5)
			};
			grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(120) });
			grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

			var labelControl = new Label
			{
				Content = label,
				FontWeight = FontWeights.Bold,
				VerticalAlignment = VerticalAlignment.Center
			};
			Grid.SetColumn(labelControl, 0);

			var valueControl = new Label
			{
				Content = value,
				VerticalAlignment = VerticalAlignment.Center,
				Tag = label // Store label for identification
			};
			Grid.SetColumn(valueControl, 1);

			grid.Children.Add(labelControl);
			grid.Children.Add(valueControl);

			return grid;
		}

		private void LoadPlayerData()
		{
			if (player == null) return;

			var components = Tag as dynamic;
			if (components?.NameLabel != null)
			{
				((Label)components.NameLabel).Content = player.Name ?? "Unknown Player";
			}

			if (components?.DetailsPanel != null)
			{
				var detailsPanel = (StackPanel)components.DetailsPanel;

				foreach (Grid row in detailsPanel.Children)
				{
					if (row.Children.Count >= 2 && row.Children[1] is Label valueLabel)
					{
						var labelText = valueLabel.Tag?.ToString();
						switch (labelText)
						{
							case "Position:":
								valueLabel.Content = player.Position ?? "Unknown";
								break;
							case "Jersey Number:":
								valueLabel.Content = player.ShirtNumber.ToString() ?? "N/A";
								break;
							case "Captain:":
								valueLabel.Content = player.Captain ? "Yes" : "No";
								break;
						}
					}
				}
			}

			// Set window title
			Title = $"Player Details - {player.Name ?? "Unknown"}";
		}

		private void CloseButton_Click(object sender, RoutedEventArgs e)
		{
			Close();
		}
	}
}