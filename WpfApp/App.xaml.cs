using System.IO;
using System.Windows;
using DataLayer; // For ConfigurationManager etc.
				 // Below alias makes sure we use the DataLayer.Models version
using AppSettings = DataLayer.Models.AppSettings;

namespace WpfApp
{
	public partial class App : Application
	{
		protected override void OnStartup(StartupEventArgs e)
		{
			base.OnStartup(e);

			// Check if configuration file exists. 
			// (Make sure to use the same filename as your ConfigurationManager, 
			// e.g., "config.txt" if that’s what is being used.)
			if (!System.IO.File.Exists("config.txt"))
			{
				// Configuration file not found – show the configuration window.
				var configWindow = new ConfigWindow();
				bool? result = configWindow.ShowDialog();
				if (result != true)
				{
					// If the user cancels the configuration, exit the app.
					Shutdown();
					return;
				}
			}

			// Load configuration (this will update static fields in ConfigurationManager).
			ConfigurationManager.LoadConfiguration();

			// Now create the MainWindow based on settings.
			var mainWindow = new MainWindow();
			if (ConfigurationManager.IsFullscreen())
			{
				mainWindow.WindowState = WindowState.Maximized;
				mainWindow.WindowStyle = WindowStyle.None;
			}
			else
			{
				// Assume WindowSize is in the format "1024x768"
				var parts = ConfigurationManager.WindowSize.Split('x');
				if (parts.Length == 2 &&
					int.TryParse(parts[0], out int width) &&
					int.TryParse(parts[1], out int height))
				{
					mainWindow.Width = width;
					mainWindow.Height = height;
				}
				mainWindow.WindowStartupLocation = WindowStartupLocation.CenterScreen;
			}

			mainWindow.Show();
		}


	}
}
