using System;
using System.Windows;
using System.Windows.Media.Animation;

namespace WpfApp
{
	public partial class TeamInfoWindow : Window
	{
		public TeamInfoWindow(TeamInfo teamInfo)
		{
			InitializeComponent();
			this.DataContext = teamInfo;
		}
		private void CloseButton_Click(object sender, RoutedEventArgs e)
		{
			this.Close();
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			// Create a fade-in animation from opacity 0 to 1 over 0.5 seconds.
			DoubleAnimation fadeAnimation = new DoubleAnimation(0, 1, new Duration(TimeSpan.FromSeconds(0.5)));
			this.BeginAnimation(Window.OpacityProperty, fadeAnimation);
		}
	}
}
