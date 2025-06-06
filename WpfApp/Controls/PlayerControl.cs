using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using DataLayer.Models;

namespace WpfApp
{
	public class PlayerControl : UserControl
	{
		private Player player;

		public Player Player
		{
			get => player;
			set
			{
				player = value;
				UpdateDisplay();
			}
		}

		public PlayerControl()
		{
			InitializeComponent();
		}

		public PlayerControl(Player player) : this()
		{
			this.Player = player;
		}

		private void InitializeComponent()
		{
			Width = 40;
			Height = 40;
			Cursor = Cursors.Hand;

			// Create the visual representation
			var grid = new Grid();

			// Background circle
			var circle = new Ellipse
			{
				Fill = new SolidColorBrush(Colors.DodgerBlue),
				Stroke = new SolidColorBrush(Colors.White),
				StrokeThickness = 2,
				Width = 36,
				Height = 36
			};

			// Player name/number text
			var textBlock = new TextBlock
			{
				Text = "?",
				Foreground = new SolidColorBrush(Colors.White),
				FontSize = 10,
				FontWeight = FontWeights.Bold,
				HorizontalAlignment = HorizontalAlignment.Center,
				VerticalAlignment = VerticalAlignment.Center,
				TextAlignment = TextAlignment.Center
			};

			grid.Children.Add(circle);
			grid.Children.Add(textBlock);

			Content = grid;

			// Add click handler
			MouseLeftButtonDown += OnMouseLeftButtonDown;
		}

		private void UpdateDisplay()
		{
			if (Player == null || !(Content is Grid grid) || grid.Children.Count < 2)
				return;

			var textBlock = grid.Children[1] as TextBlock;
			if (textBlock != null)
			{
				textBlock.Text = !string.IsNullOrEmpty(Player.ShirtNumber.ToString())
					? Player.ShirtNumber.ToString()
					: (!string.IsNullOrEmpty(Player.Name) ? Player.Name.Substring(0, 1).ToUpper() : "?");
			}

			ToolTip = $"{Player.Name}\nPosition: {Player.Position}";
		}

		private void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			if (Player != null)
			{
				try
				{
					var detailsWindow = new PlayerDetailsWindow(Player);

					// Use Window.GetWindow to find parent window (any type)
					var parentWindow = Window.GetWindow(this);

					detailsWindow.ShowDialog();
				}
				catch (Exception ex)
				{
					MessageBox.Show($"Error showing player details: {ex.Message}", "Error",
						MessageBoxButton.OK, MessageBoxImage.Error);
				}
			}
		}
	}
}
