using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using DataLayer.Models;
using DataLayer;

namespace WpfApp
{
	public partial class ConfigWindow : Window
	{
		public ConfigWindow()
		{
			InitializeComponent();
		}

		// This event fires for any key presses in the window.
		private void ConfigWindow_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Enter)
			{
				// Treat the Enter key as OK/Confirm.
				OkButton_Click(sender, e);
			}
			else if (e.Key == Key.Escape)
			{
				// Treat the Esc key as Cancel.
				CancelButton_Click(sender, e);
			}
		}

		private void DisplayMode_Checked(object sender, RoutedEventArgs e)
		{
			if (WindowedRadioButton != null && WindowedRadioButton.IsChecked == true)
			{
				if (ResolutionPanel != null)
					ResolutionPanel.Visibility = Visibility.Visible;
			}
			else
			{
				if (ResolutionPanel != null)
					ResolutionPanel.Visibility = Visibility.Collapsed;
			}
		}


		private void OkButton_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				// Read Tournament selection.
				string tournament = "men";
				if (TournamentComboBox.SelectedItem is ComboBoxItem tItem)
				{
					// For example, we assume the content is "Men" or "Women"
					tournament = tItem.Content.ToString().ToLowerInvariant();
				}

				// Read Language selection.
				string language = "en";
				if (LanguageComboBox.SelectedItem is ComboBoxItem langItem)
				{
					// We assume the Tag contains the language code (e.g., "en", "hr")
					language = langItem.Tag.ToString();
				}

				// Read Display Mode and Resolution.
				string displayMode = "Fullscreen";
				int width = 0, height = 0;
				if (FullscreenRadioButton != null && FullscreenRadioButton.IsChecked == true)
				{
					displayMode = "Fullscreen";
				}
				else if (WindowedRadioButton != null && WindowedRadioButton.IsChecked == true)
				{
					displayMode = "Windowed";
					// Read the selected resolution.
					if (ResolutionComboBox?.SelectedItem is ComboBoxItem resItem)
					{
						string resTag = resItem.Tag?.ToString() ?? "1024x768";
						var parts = resTag.Split('x');
						if (parts.Length == 2 &&
							int.TryParse(parts[0], out width) &&
							int.TryParse(parts[1], out height))
						{
							// Valid resolution acquired.
						}
						else
						{
							// Fallback to 1024x768 if parsing fails.
							width = 1024;
							height = 768;
						}
					}
				}

				// Create the settings instance.
				var settings = new DataLayer.Models.AppSettings
				{
					Tournament = tournament,
					Language = language,
					DisplayMode = displayMode,
					Width = width,
					Height = height
				};

				// Save settings to file ("config.txt" in the working directory).
				string filePath = "config.txt";
				string[] lines = new string[]
				{
			$"Tournament={settings.Tournament}",
			$"Language={settings.Language}",
			$"DisplayMode={settings.DisplayMode}",
			$"Width={settings.Width}",
			$"Height={settings.Height}"
				};

				File.WriteAllLines(filePath, lines);

				// For debugging purposes, show a confirmation message.
				MessageBox.Show("Configuration saved successfully.", "Configuration", MessageBoxButton.OK, MessageBoxImage.Information);

				DialogResult = true;
				Close();
			}
			catch (Exception ex)
			{
				MessageBox.Show("Error saving configuration: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}


		private void CancelButton_Click(object sender, RoutedEventArgs e)
		{
			DialogResult = false;
			Close();
		}
	}
}
