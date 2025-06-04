using DataLayer;
using System.IO;
using System.Windows;
using WorldCupStats.WinFormsApp.Helpers;

namespace WpfApp
{
	public partial class App : Application
	{
		protected override void OnStartup(StartupEventArgs e)
		{
			base.OnStartup(e);

			var settings = ConfigManager.LoadSettings();
			bool missingConfig = settings == null || !File.Exists("display_mode.txt");

			if (missingConfig)
			{
				var startupWindow = new StartupSettingsWindow();
				bool? result = startupWindow.ShowDialog();

				if (result != true)
				{
					Shutdown(); // user cancelled
					return;
				}

				settings = ConfigManager.LoadSettings(); // reload now that it's saved
			}

			string resolution = File.ReadAllText("display_mode.txt");

			var main = new MainWindow();

			if (resolution == "Fullscreen")
			{
				main.WindowState = WindowState.Maximized;
				main.WindowStyle = WindowStyle.None;
			}
			else
			{
				var dims = resolution.Split('x');
				if (dims.Length == 2 &&
					int.TryParse(dims[0].Trim(), out int width) &&
					int.TryParse(dims[1].Trim(), out int height))
				{
					main.Width = width;
					main.Height = height;
				}
			}

			main.Show();
		}
	}
}
