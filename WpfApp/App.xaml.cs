using System.IO;
using System.Windows;

namespace WpfApp
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            if (!File.Exists("settings.txt"))
            {
                var settingsWindow = new SettingsWindow();
                bool? result = settingsWindow.ShowDialog();

                if (result != true)
                {
                    Shutdown();
                    return;
                }
            }

            var mainWindow = new MainWindow();
            mainWindow.Show();
        }
    }
}
