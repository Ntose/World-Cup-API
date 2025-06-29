﻿using System.IO;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using DataLayer.Models;
using WpfApp.Properties;
using DataLayer;
using System;

namespace WpfApp
{
	public partial class PlayerControl : UserControl
	{
		public PlayerControl(Player player)
		{
			InitializeComponent();
			PlayerName.Text = player.Name;
			PlayerNumber.Text = $"#{player.ShirtNumber}";

			if (!string.IsNullOrEmpty(player.ImagePath) && File.Exists(player.ImagePath))
			{
				PlayerImage.Source = new BitmapImage(new System.Uri(player.ImagePath));
			}
			else
			{
				// fallback (ensure this exists in Resources)
				PlayerImage.Resources["DefaultPlayerImage"] = new BitmapImage(new System.Uri("pack://application:,,,/Resources/DefaultPlayer.jpg"));

			}
		}
	}
}
