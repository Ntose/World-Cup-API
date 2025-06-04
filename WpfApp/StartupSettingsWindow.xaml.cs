using System.IO;
using System.Windows;
using System.Windows.Controls;
using DataLayer;
using WorldCupStats.WinFormsApp.Helpers; // or your actual namespace

namespace WpfApp
{
	public partial class StartupSettingsWindow : Window
	{
		public StartupSettingsWindow()
		{
			InitializeComponent();
		}

		private void Confirm_Click(object sender, RoutedEventArgs e)
		{
			string language = ((ComboBoxItem)comboLanguage.SelectedItem).Content.ToString();
			string tournament = ((ComboBoxItem)comboTournament.SelectedItem).Content.ToString();
			string resolution = ((ComboBoxItem)comboResolution.SelectedItem).Content.ToString();

			// Normalize
			language = language == "Croatian" ? "hr" : "en";
			tournament = tournament == "Men" ? "men" : "women";

			ConfigManager.SaveSettings(language, tournament);
			File.WriteAllText("display_mode.txt", resolution);

			DialogResult = true;
			Close();
		}

	}
}
