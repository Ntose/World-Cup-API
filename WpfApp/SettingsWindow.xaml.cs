using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace WpfApp
{
	public partial class SettingsWindow : Window
	{
		public string Championship { get; private set; }
		public string Language { get; private set; }
		public string Resolution { get; private set; }

		private readonly string settingsFilePath = "settings.txt";

		public SettingsWindow()
		{
			InitializeComponent();
		}

		private void Confirm_Click(object sender, RoutedEventArgs e)
		{
			Championship = ((ComboBoxItem)ChampionshipComboBox.SelectedItem)?.Content.ToString();
			Language = ((ComboBoxItem)LanguageComboBox.SelectedItem)?.Content.ToString();
			Resolution = ((ComboBoxItem)ResolutionComboBox.SelectedItem)?.Content.ToString();

			File.WriteAllText(settingsFilePath, $"{Championship};{Language};{Resolution}");
			DialogResult = true;
		}

		private void Cancel_Click(object sender, RoutedEventArgs e)
		{
			Application.Current.Shutdown();
		}
	}
}
