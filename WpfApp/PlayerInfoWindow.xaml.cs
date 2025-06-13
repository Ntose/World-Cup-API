using System;
using System.Windows;
using System.Windows.Media.Animation;

namespace WpfApp
{
	public partial class PlayerInfoWindow : Window
	{
		public PlayerInfoWindow(object playerInfo)
		{
			InitializeComponent();
			// Set the DataContext so that bindings (Name, Picture, etc.) work
			DataContext = playerInfo;
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			// Create scale animations for the zoom-in effect (0.3 seconds duration)
			DoubleAnimation scaleXAnim = new DoubleAnimation(0.5, 1.0, TimeSpan.FromSeconds(0.3));
			DoubleAnimation scaleYAnim = new DoubleAnimation(0.5, 1.0, TimeSpan.FromSeconds(0.3));

			scaleTransform.BeginAnimation(System.Windows.Media.ScaleTransform.ScaleXProperty, scaleXAnim);
			scaleTransform.BeginAnimation(System.Windows.Media.ScaleTransform.ScaleYProperty, scaleYAnim);
		}

		private void CloseButton_Click(object sender, RoutedEventArgs e)
		{
			this.Close();
		}
	}
}
