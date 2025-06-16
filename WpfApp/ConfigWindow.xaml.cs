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
				// Read championship and language from the combo boxes.
				string championship =
					(TournamentComboBox.SelectedItem as ComboBoxItem)?.Content.ToString().ToLowerInvariant() ?? "men";
				string language =
					(LanguageComboBox.SelectedItem as ComboBoxItem)?.Tag?.ToString() ?? "en";

				// Determine the window size.
				// If fullscreen radio is checked, store "fullscreen".
				// Otherwise, store the resolution string from the resolution combo.
				string windowSize = "fullscreen";
				if (WindowedRadioButton != null && WindowedRadioButton.IsChecked == true)
				{
					windowSize = (ResolutionComboBox?.SelectedItem as ComboBoxItem)?.Tag?.ToString() ?? "1024x768";
				}

				// Save configuration as three lines in config.txt.
				string[] lines = new string[]
				{
			$"SelectedChampionship={championship}",
			$"SelectedLanguage={language}",
			$"WindowSize={windowSize}"
				};

				File.WriteAllLines("config.txt", lines);

				MessageBox.Show("Configuration saved successfully.", "Configuration", MessageBoxButton.OK, MessageBoxImage.Information);

				// Restart the MainWindow (your method for restarting the window)
				RestartMainWindow();

				DialogResult = true;
				Close();
			}
			catch (Exception ex)
			{
				MessageBox.Show("Error saving configuration: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}


		private void RestartMainWindow()
		{
			// Get the current MainWindow instance
			var currentMainWindow = Application.Current.MainWindow as MainWindow;
			if (currentMainWindow != null)
			{
				currentMainWindow.Close(); // Close the existing MainWindow
			}

			// Create a new MainWindow instance
			MainWindow newMainWindow = new MainWindow();
			Application.Current.MainWindow = newMainWindow; // Set it as the new main window
			newMainWindow.Show(); // Show the new MainWindow
		}


		private void CancelButton_Click(object sender, RoutedEventArgs e)
		{
			DialogResult = false;
			Close();
		}
	}
}
