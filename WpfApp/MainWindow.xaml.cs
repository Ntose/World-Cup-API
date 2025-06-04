using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Windows;
using System.Windows.Controls;
using DataLayer.Models;
using Newtonsoft.Json;
using WorldCupStats.WinFormsApp.Helpers;

namespace WpfApp
{
	public partial class MainWindow : Window
	{
		private List<Match> matches;
		private string selectedFifaCode;

		public MainWindow()
		{
			InitializeComponent();
			Loaded += MainWindow_Loaded;
		}

		private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
		{
			var settings = ConfigManager.LoadSettings();
			if (settings == null)
			{
				MessageBox.Show("No config found. Please run the Windows Forms app and select your team first.");
				return;
			}

			// Load team from file
			string teamFile = settings.Tournament == "men" ? "favorite_team_men.txt" : "favorite_team_women.txt";
			if (!File.Exists(teamFile))
			{
				MessageBox.Show("Favorite team not set.");
				return;
			}

			string teamLine = File.ReadAllText(teamFile).Trim();
			int start = teamLine.IndexOf('(');
			int end = teamLine.IndexOf(')');
			if (start == -1 || end == -1) return;

			selectedFifaCode = teamLine.Substring(start + 1, 3);

			// Load matches
			string url = $"https://worldcup-vua.nullbit.hr/{settings.Tournament}/matches/country?fifa_code={selectedFifaCode}";

			try
			{
				string json;
				using (var client = new HttpClient())
				{
					json = await client.GetStringAsync(url);
				}
				matches = JsonConvert.DeserializeObject<List<Match>>(json);
			}
			catch (Exception ex)
			{
				MessageBox.Show("Error loading matches: " + ex.Message);
				return;
			}

			// Populate your team ComboBox
			comboYourTeam.ItemsSource = new List<string> { teamLine };
			comboYourTeam.SelectedIndex = 0;

			// Populate opponent ComboBox
			var opponents = matches.Select(m =>
				m.AwayTeam.FifaCode == selectedFifaCode ? $"{m.HomeTeam.Country} ({m.HomeTeam.FifaCode})"
														: $"{m.AwayTeam.Country} ({m.AwayTeam.FifaCode})")
				.Distinct()
				.OrderBy(name => name)
				.ToList();

			comboOpponent.ItemsSource = opponents;
		}

		private void comboOpponent_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (comboYourTeam.SelectedItem == null || comboOpponent.SelectedItem == null) return;

			string opponentLine = comboOpponent.SelectedItem.ToString();
			int start = opponentLine.IndexOf('(');
			int end = opponentLine.IndexOf(')');
			if (start == -1 || end == -1) return;

			string opponentCode = opponentLine.Substring(start + 1, 3);

			var match = matches.FirstOrDefault(m =>
				(m.HomeTeam.FifaCode == selectedFifaCode && m.AwayTeam.FifaCode == opponentCode) ||
				(m.AwayTeam.FifaCode == selectedFifaCode && m.HomeTeam.FifaCode == opponentCode));

			if (match == null)
			{
				txtResult.Text = "Match not found.";
				return;
			}

			txtResult.Text = $"{match.HomeTeamGoals} : {match.AwayTeamGoals}";
		}

		private void BtnYourTeamInfo_Click(object sender, RoutedEventArgs e)
		{
			// To be implemented: animated team stats popup
		}

		private void BtnOpponentInfo_Click(object sender, RoutedEventArgs e)
		{
			// To be implemented: animated team stats popup
		}
	}
}
