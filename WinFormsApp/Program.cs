using System;
using System.Windows.Forms;
using DataLayer;
//using WinFormsApp.WinFormsApp; // Make sure to include the DataLayer namespace

namespace WinFormsApp
{
	static class Program
	{
		[STAThread]
		static void Main()
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);

			// Load configuration from the file
			ConfigurationManager.LoadConfiguration();

			// If the configuration file doesn't exist or is incomplete, show the initial settings form [cite: 26]
			if (string.IsNullOrEmpty(ConfigurationManager.SelectedChampionship) || string.IsNullOrEmpty(ConfigurationManager.SelectedLanguage))
			{
				// Show the initial settings form as a dialog
				var initialSettingsForm = new InitialSettingsForm();
				if (initialSettingsForm.ShowDialog() != DialogResult.OK)
				{
					// If the user cancels, exit the application
					return;
				}
			}

			// Once settings are confirmed, run the main form
			//Application.Run(new MainForm());
		}
	}
}