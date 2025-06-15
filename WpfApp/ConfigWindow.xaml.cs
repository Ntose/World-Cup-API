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
				string championship = "men";
				if (TournamentComboBox.SelectedItem is ComboBoxItem tItem)
				{
					// Expecting "Men" or "Women"
					championship = tItem.Content.ToString().ToLowerInvariant();
				}

				// Read Language selection.
				string language = "en";
				if (LanguageComboBox.SelectedItem is ComboBoxItem langItem)
				{
					// Assuming Tag has the code, e.g. "en" or "hr"
					language = langItem.Tag.ToString();
				}

				// Read Display Mode and resolution.
				string windowSize = "fullscreen"; // default for fullscreen, for example
				if (FullscreenRadioButton != null && FullscreenRadioButton.IsChecked == true)
				{
					windowSize = "fullscreen";
				}
				else if (WindowedRadioButton != null && WindowedRadioButton.IsChecked == true)
				{
					// In windowed mode, read the resolution from ResolutionComboBox.
					if (ResolutionComboBox?.SelectedItem is ComboBoxItem resItem)
					{
						string resTag = resItem.Tag?.ToString() ?? "1024x768";
						// We assume resTag is already in the format "WIDTHxHEIGHT"
						windowSize = resTag;
					}
					else
					{
						windowSize = "1024x768";
					}
				}

				// Create settings and save them.
				// Note: We're not saving "DisplayMode", "Width", and "Height" separately now.
				var settings = new DataLayer.Models.AppSettings
				{
					Tournament = championship,    // This will be read as SelectedChampionship
					Language = language,          // This will be read as SelectedLanguage
					DisplayMode = windowSize      // We'll treat DisplayMode property as holding the window size info.
				};

				// Save settings to file ("config.txt" in the working directory).
				string filePath = "config.txt";
				string[] lines = new string[]
				{
			$"SelectedChampionship={settings.Tournament}",
			$"SelectedLanguage={settings.Language}",
			$"WindowSize={settings.DisplayMode}"
				};

				File.WriteAllLines(filePath, lines);

				MessageBox.Show("Configuration saved successfully.", "Configuration", MessageBoxButton.OK, MessageBoxImage.Information);

				DialogResult = true;
				var MainWindow = Application.Current.MainWindow as MainWindow;
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
