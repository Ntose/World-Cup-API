using DataLayer;
using DataLayer.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WorldCupStats.WinFormsApp.Helpers;

namespace WinFormsApp.Forms
{

	public partial class RankingForm : Form
	{
		private DataGridView dgvPlayers;
		private DataGridView dgvRanking;
		public RankingForm()
		{
			InitializeComponent();
			SetupGrid();
			this.Text = "Player Rankings";
			this.Load += async (s, e) => await LoadPlayerRankingAsync();
		}
		private void SetupGrid()
		{

			dgvPlayers = new DataGridView
			{
				Location = new Point(10, 10),
				Size = new Size(800, 400),
				ReadOnly = true,
				AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
				AllowUserToAddRows = false
			};

			Controls.Add(dgvPlayers);
			ComboBox comboRanking = new ComboBox
			{
				Location = new Point(10, 420),
				Size = new Size(200, 28),
				DropDownStyle = ComboBoxStyle.DropDownList
			};

			comboRanking.Items.AddRange(new string[]
			{
			"Goals",
			"Yellow Cards",
			"Appearances"
			});
			comboRanking.SelectedIndex = 0; // default = Goals

			comboRanking.SelectedIndexChanged += async (s, e) => await LoadPlayerRankingAsync(comboRanking.SelectedItem.ToString());

			Controls.Add(comboRanking);
		}

		private async Task LoadPlayerRankingAsync(string sortBy = "Goals")
		{
			var settings = ConfigManager.LoadSettings();
			if (settings == null) return;

			// Get the favorite team
			string teamFile = settings.Tournament == "men"
				? "favorite_team_men.txt"
				: "favorite_team_women.txt";

			if (!File.Exists(teamFile)) return;

			string favoriteTeam = File.ReadAllText(teamFile).Trim();
			int start = favoriteTeam.IndexOf('(');
			int end = favoriteTeam.IndexOf(')');
			if (start == -1 || end == -1) return;

			string fifaCode = favoriteTeam.Substring(start + 1, 3);
			string url = $"https://worldcup-vua.nullbit.hr/{settings.Tournament}/matches/country?fifa_code={fifaCode}";

			using var client = new HttpClient();
			string json = await client.GetStringAsync(url);
			var matches = JsonConvert.DeserializeObject<List<Match>>(json);

			var playerStats = new Dictionary<string, (int appearances, int goals, int yellowCards)>();

			foreach (var match in matches)
			{
				var players = match.HomeTeamStatistics?.StartingEleven
					.Concat(match.HomeTeamStatistics?.Substitutes ?? new List<Player>()) ?? new List<Player>();

				foreach (var player in players)
				{
					if (!playerStats.ContainsKey(player.Name))
						playerStats[player.Name] = (0, 0, 0);

					var stat = playerStats[player.Name];
					playerStats[player.Name] = (stat.appearances + 1, stat.goals, stat.yellowCards);
				}

				var events = match.HomeTeamEvents; // You can also include AwayTeamEvents if needed

				foreach (var ev in events)
				{
					if (!playerStats.ContainsKey(ev.Player))
						continue;

					var stat = playerStats[ev.Player];

					if (ev.TypeOfEvent == "goal" || ev.TypeOfEvent == "goal-penalty")
						playerStats[ev.Player] = (stat.appearances, stat.goals + 1, stat.yellowCards);
					else if (ev.TypeOfEvent == "yellow-card")
						playerStats[ev.Player] = (stat.appearances, stat.goals, stat.yellowCards + 1);
				}
			}
			IEnumerable<object> rows = playerStats.Select(ps => new
			{
				Player = ps.Key,
				Appearances = ps.Value.appearances,
				Goals = ps.Value.goals,
				YellowCards = ps.Value.yellowCards
			});

			rows = sortBy switch
			{
				"Goals" => rows.OrderByDescending(p => ((dynamic)p).Goals),
				"Yellow Cards" => rows.OrderByDescending(p => ((dynamic)p).YellowCards),
				"Appearances" => rows.OrderByDescending(p => ((dynamic)p).Appearances),
				_ => rows
			};

			dgvPlayers.DataSource = rows.ToList();


		}
	}
}
