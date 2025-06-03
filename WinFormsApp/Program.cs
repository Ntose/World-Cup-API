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
			Thread.CurrentThread.CurrentUICulture = new CultureInfo("hr");
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


			Application.Run(new MainForm());
		}
	}
}
