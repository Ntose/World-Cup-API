using System.Windows;
using System.Windows.Input;

namespace WpfApp
{
	public partial class ConfirmCloseWindow : Window
	{
		public ConfirmCloseWindow()
		{
			InitializeComponent();
		}

		private void Window_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Enter)
			{
				// Confirm exit
				DialogResult = true;
			}
			else if (e.Key == Key.Escape)
			{
				// Cancel exit
				DialogResult = false;
			}
		}
	}
}
