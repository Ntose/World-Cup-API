using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using DataLayer;
using DataLayer.Models;

namespace WpfApp
{
	public partial class SettingsWindow : Window
	{
		private DataManager dataManager;
		private List<string> availableChampionships;

		public SettingsWindow()
		{
			InitializeComponent();
			LoadSettingsAsync();
		}

		private void InitializeComponent()
		{
			Title = "Settings";
			Width = 500;
			Height = 350;
			ResizeMode = ResizeMode.NoResize;
			WindowStartupLocation = WindowStartupLocation.CenterOwner;
			Background = new SolidColorBrush(Color.FromRgb(248, 249, 250));

			var mainGrid = new Grid();
			mainGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
			mainGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
			mainGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

			// Header
			var headerPanel = CreateHeaderPanel();
			Grid.SetRow(headerPanel, 0);

			// Settings content
			var settingsPanel = CreateSettingsPanel();
			Grid.SetRow(settingsPanel, 1);

			// Button panel
			var buttonPanel = CreateButtonPanel();
			Grid.SetRow(buttonPanel, 2);

			mainGrid.Children.Add(headerPanel);
			mainGrid.Children.Add(settingsPanel);
			mainGrid.Children.Add(buttonPanel);

			Content = mainGrid;
		}

		private StackPanel CreateHeaderPanel()
		{
			var headerPanel = new StackPanel
			{
				Orientation = Orientation.Vertical,
				Background = new SolidColorBrush(Colors.DarkSlateBlue),
				Margin = new Thickness(0, 0, 0, 20)
			};

			var titleLabel = new Label
			{
				Content = "Application Settings",
				FontSize = 24,
				FontWeight = FontWeights.Bold,
				Foreground = new SolidColorBrush(Colors.White),
				HorizontalAlignment = HorizontalAlignment.Center,
				Padding = new Thickness(20, 15, 20, 15)
			};

			var subtitleLabel = new Label
			{
				Content = "Configure your preferences",
				FontSize = 14,
				Foreground = new SolidColorBrush(Colors.LightGray),
				HorizontalAlignment = HorizontalAlignment.Center,
				Padding = new Thickness(20, 0, 20, 15)
			};

			headerPanel.Children.Add(titleLabel);
			headerPanel.Children.Add(subtitleLabel);

			return headerPanel;
		}

		private ScrollViewer CreateSettingsPanel()
		{
			var scrollViewer = new ScrollViewer
			{
				VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
				Margin = new Thickness(30, 0, 30, 20)
			};

			var settingsPanel = new StackPanel
			{
				Orientation = Orientation.Vertical
			};

			// Championship Selection Section
			settingsPanel.Children.Add(CreateSectionHeader("Championship Selection"));

			var championshipGrid = CreateSettingRow("Championship:", CreateChampionshipComboBox());
			settingsPanel.Children.Add(championshipGrid);

			settingsPanel.Children.Add(CreateSeparator());

			// Display Settings Section
			settingsPanel.Children.Add(CreateSectionHeader("Display Settings"));

			var animationsGrid = CreateSettingRow("Enable Animations:", CreateAnimationsCheckBox());
			settingsPanel.Children.Add(animationsGrid);

			var tooltipsGrid = CreateSettingRow("Show Tooltips:", CreateTooltipsCheckBox());
			settingsPanel.Children.Add(tooltipsGrid);

			scrollViewer.Content = settingsPanel;
			return scrollViewer;
		}

		private Label CreateSectionHeader(string title)
		{
			return new Label
			{
				Content = title,
				FontSize = 16,
				FontWeight = FontWeights.Bold,
				Foreground = new SolidColorBrush(Colors.DarkSlateBlue),
				Margin = new Thickness(0, 10, 0, 5),
				Padding = new Thickness(0, 5, 0, 5)
			};
		}

		private Grid CreateSettingRow(string labelText, FrameworkElement control)
		{
			var grid = new Grid
			{
				Margin = new Thickness(0, 8, 0, 8)
			};
			grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(180) });
			grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

			var label = new Label
			{
				Content = labelText,
				FontWeight = FontWeights.Medium,
				VerticalAlignment = VerticalAlignment.Center,
				Foreground = new SolidColorBrush(Colors.DarkSlateGray)
			};
			Grid.SetColumn(label, 0);

			Grid.SetColumn(control, 1);
			control.VerticalAlignment = VerticalAlignment.Center;

			grid.Children.Add(label);
			grid.Children.Add(control);

			return grid;
		}

		private ComboBox CreateChampionshipComboBox()
		{
			var comboBox = new ComboBox
			{
				Name = "cmbChampionship",
				MinWidth = 200,
				Height = 30,
				FontSize = 12,
				Background = new SolidColorBrush(Colors.White),
				BorderBrush = new SolidColorBrush(Colors.LightGray),
				IsEnabled = false // Will be enabled after loading
			};

			comboBox.SelectionChanged += CmbChampionship_SelectionChanged;
			return comboBox;
		}

		private CheckBox CreateAnimationsCheckBox()
		{
			var checkBox = new CheckBox
			{
				Name = "chkAnimations",
				IsChecked = true, // Default value
				VerticalAlignment = VerticalAlignment.Center
			};

			return checkBox;
		}

		private CheckBox CreateTooltipsCheckBox()
		{
			var checkBox = new CheckBox
			{
				Name = "chkTooltips",
				IsChecked = true, // Default value
				VerticalAlignment = VerticalAlignment.Center
			};

			return checkBox;
		}

		private Separator CreateSeparator()
		{
			return new Separator
			{
				Margin = new Thickness(0, 15, 0, 5),
				Background = new SolidColorBrush(Colors.LightGray)
			};
		}

		private StackPanel CreateButtonPanel()
		{
			var buttonPanel = new StackPanel
			{
				Orientation = Orientation.Horizontal,
				HorizontalAlignment = HorizontalAlignment.Right,
				Margin = new Thickness(30, 0, 30, 20)
			};

			var cancelButton = new Button
			{
				Content = "Cancel",
				Width = 100,
				Height = 35,
				Margin = new Thickness(0, 0, 10, 0),
				Background = new SolidColorBrush(Colors.LightGray),
				Foreground = new SolidColorBrush(Colors.Black),
				FontWeight = FontWeights.Medium
			};
			cancelButton.Click += CancelButton_Click;

			var saveButton = new Button
			{
				Content = "Save & Apply",
				Width = 120,
				Height = 35,
				Background = new SolidColorBrush(Colors.DarkSlateBlue),
				Foreground = new SolidColorBrush(Colors.White),
				FontWeight = FontWeights.Medium
			};
			saveButton.Click += SaveButton_Click;

			buttonPanel.Children.Add(cancelButton);
			buttonPanel.Children.Add(saveButton);

			return buttonPanel;
		}

		private async void LoadSettingsAsync()
		{
			try
			{
				dataManager = new DataManager();

				// Load available championships
				availableChampionships = await dataManager.GetAvailableChampionshipsAsync();

				var championshipComboBox = FindName("cmbChampionship") as ComboBox ??
					FindControlByName("cmbChampionship") as ComboBox;

				if (championshipComboBox != null)
				{
					championshipComboBox.ItemsSource = availableChampionships;
					championshipComboBox.IsEnabled = true;

					// Set current selection
					var currentChampionship = ConfigurationManager.SelectedChampionship;
					if (!string.IsNullOrEmpty(currentChampionship) && availableChampionships.Contains(currentChampionship))
					{
						championshipComboBox.SelectedItem = currentChampionship;
					}
					else if (availableChampionships.Count > 0)
					{
						championshipComboBox.SelectedIndex = 0;
					}
				}

				// Load other settings (you can extend this for additional settings)
				LoadDisplaySettings();
			}
			catch (Exception ex)
			{
				MessageBox.Show($"Error loading settings: {ex.Message}", "Error",
					MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}

		private void LoadDisplaySettings()
		{
			// Load saved display preferences (implement as needed)
			// For now, using default values
		}

		private FrameworkElement FindControlByName(string name)
		{
			return FindControlByName(this, name);
		}

		private FrameworkElement FindControlByName(DependencyObject parent, string name)
		{
			if (parent == null) return null;

			var childCount = System.Windows.Media.VisualTreeHelper.GetChildrenCount(parent);
			for (int i = 0; i < childCount; i++)
			{
				var child = System.Windows.Media.VisualTreeHelper.GetChild(parent, i);

				if (child is FrameworkElement fe && fe.Name == name)
					return fe;

				var result = FindControlByName(child, name);
				if (result != null)
					return result;
			}

			return null;
		}

		private void CmbChampionship_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			// Handle championship selection change if needed
		}

		private void SaveButton_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				// Save championship selection
				var championshipComboBox = FindControlByName("cmbChampionship") as ComboBox;
				if (championshipComboBox?.SelectedItem != null)
				{
					ConfigurationManager.SelectedChampionship = championshipComboBox.SelectedItem.ToString();
				}

				// Save other settings
				var animationsCheckBox = FindControlByName("chkAnimations") as CheckBox;
				var tooltipsCheckBox = FindControlByName("chkTooltips") as CheckBox;

				// You can extend this to save additional settings
				// ConfigurationManager.EnableAnimations = animationsCheckBox?.IsChecked ?? true;
				// ConfigurationManager.ShowTooltips = tooltipsCheckBox?.IsChecked ?? true;

				DialogResult = true;
				Close();
			}
			catch (Exception ex)
			{
				MessageBox.Show($"Error saving settings: {ex.Message}", "Error",
					MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}

		private void CancelButton_Click(object sender, RoutedEventArgs e)
		{
			DialogResult = false;
			Close();
		}
	}
}