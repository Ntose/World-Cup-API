using System;
using System.Globalization;
using System.Threading;
using System.Windows.Forms;
using WorldCupStats.WinFormsApp.Helpers;
using WorldCupStats.WinFormsApp.Forms;
using WinFormsApp;

namespace WorldCupStats.WinFormsApp
{
	static class Program
	{
		[STAThread]
		static void Main()
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);

			// Load settings
			var settings = ConfigManager.LoadSettings();

			if (settings == null)
			{
				using (var settingsForm = new SettingsForm())
				{
					if (settingsForm.ShowDialog() == DialogResult.OK)
					{
						settings = ConfigManager.LoadSettings();
					}
					else
					{
						return;
					}
				}
			}
			Thread.CurrentThread.CurrentUICulture = new CultureInfo(settings.Language);


			Application.Run(new MainForm());
		}
	}
}
