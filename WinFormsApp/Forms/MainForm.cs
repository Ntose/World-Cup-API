using DataLayer;
using DataLayer.Models;
using Newtonsoft.Json;
using System.Windows.Forms.Design;
using WinFormsApp.Controls;
using WorldCupStats.WinFormsApp.Forms;
using WorldCupStats.WinFormsApp.Helpers;

namespace WinFormsApp
{
	public partial class MainForm : Form
	{
		public MainForm()
		{
			InitializeComponent();
			pnlAllPlayers.DragEnter += (s, e) => e.Effect = DragDropEffects.Move;
			pnlFavoritePlayers.DragEnter += (s, e) => e.Effect = DragDropEffects.Move;

			pnlFavoritePlayers.DragDrop += (s, e) =>
			{
				var card = e.Data.GetData(typeof(PlayerCard)) as PlayerCard;
				if (card != null && !pnlFavoritePlayers.Controls.Contains(card) && pnlFavoritePlayers.Controls.Count < 3)
				{
					pnlAllPlayers.Controls.Remove(card);
					pnlFavoritePlayers.Controls.Add(card);
				}
			};

			pnlAllPlayers.DragDrop += (s, e) =>
			{
				var card = e.Data.GetData(typeof(PlayerCard)) as PlayerCard;
				if (card != null && !pnlAllPlayers.Controls.Contains(card))
				{
					pnlFavoritePlayers.Controls.Remove(card);
					pnlAllPlayers.Controls.Add(card);
				}
			};

			this.Text = Resources.Resources.MainTitle;
			lbTeamSelect.Text = Resources.Resources.TeamSelect;
			settingToolStripMenuItem.Text = Resources.Resources.SettingsTitle;
			closeToolStripMenuItem.Text = Resources.Resources.Close;
			btnConfirm.Text = Resources.Resources.btnApply;
		}

		private void settingToolStripMenuItem_Click(object sender, EventArgs e)
		{
			using (var settingsForm = new SettingsForm())
			{
				if (settingsForm.ShowDialog() == DialogResult.OK)
				{
					// Restart to apply new settings
					Application.Restart();
					Environment.Exit(0);
				}
			}
		}

		private void closeToolStripMenuItem_Click(object sender, EventArgs e)
		{
			this.Close();
		}

		private async void MainForm_Load(object sender, EventArgs e)
		{
			var settings = ConfigManager.LoadSettings();
			if (settings == null) return;

			try
			{
				var teams = await ApiService.GetTeamsAsync(settings.Tournament);
				comboTeams.Items.Clear();

				foreach (var team in teams.OrderBy(t => t.Country))
				{
					comboTeams.Items.Add(team.ToString());
				}

				if (File.Exists("favorite_team.txt"))
				{
					var saved = File.ReadAllText("favorite_team.txt");
					comboTeams.SelectedItem = saved;

					await LoadFavoritePlayers(); // Load players only if favorite is known
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show("Failed to load teams: " + ex.Message);
			}

			// UI Texts
			this.Text = Resources.Resources.MainTitle;
			lbTeamSelect.Text = Resources.Resources.TeamSelect;
			settingToolStripMenuItem.Text = Resources.Resources.SettingsTitle;
			closeToolStripMenuItem.Text = Resources.Resources.Close;
			btnConfirm.Text = Resources.Resources.btnApply;
			lbAllPlayers.Text = Resources.Resources.AllPlayers;
			lbFavoritePlayers.Text = Resources.Resources.FavoritePlayers;
		}


		private async void btnConfirm_Click(object sender, EventArgs e)
		{
			if (comboTeams.SelectedItem != null)
			{
				var selectedTeam = comboTeams.SelectedItem.ToString();
				File.WriteAllText("favorite_team.txt", selectedTeam);

				MessageBox.Show($"{Resources.Resources.FavoriteTeam} {selectedTeam}");

				await LoadFavoritePlayers(); //load players after setting favorite
			}
			else
			{
				MessageBox.Show(Resources.Resources.PickFavoriteTeam);
			}
		}

		private async Task LoadFavoritePlayers()
		{
			var settings = ConfigManager.LoadSettings();
			string favoriteTeam = File.Exists("favorite_team.txt")
				? File.ReadAllText("favorite_team.txt").Trim()
				: null;

			if (string.IsNullOrEmpty(favoriteTeam))
			{
				MessageBox.Show("Favorite team not found.");
				return;
			}

			string fifaCode = favoriteTeam.Substring(favoriteTeam.IndexOf('(') + 1, 3);
			string url = $"https://worldcup-vua.nullbit.hr/{settings.Tournament}/matches/country?fifa_code={fifaCode}";

			using var client = new HttpClient();
			string json = await client.GetStringAsync(url);
			var matches = JsonConvert.DeserializeObject<List<Match>>(json);
			var firstMatch = matches.FirstOrDefault();

			if (firstMatch == null)
			{
				MessageBox.Show("No matches found.");
				return;
			}

			var startingEleven = firstMatch.HomeTeamStartingEleven ?? new List<Player>();
			var substitutes = firstMatch.HomeTeamSubstitutes ?? new List<Player>();

			var players = startingEleven.Concat(substitutes).ToList();


			var favoriteNames = File.Exists("favorite_players.txt")
				? File.ReadAllLines("favorite_players.txt").ToList()
				: new List<string>();

			pnlAllPlayers.Controls.Clear(); // Clear previous
			pnlFavoritePlayers.Controls.Clear();

			foreach (var player in players)
			{
				var card = new PlayerCard(player, favoriteNames.Contains(player.Name));
				pnlAllPlayers.Controls.Add(card);
			}
		}

		private void comboTeams_SelectedIndexChanged(object sender, EventArgs e)
		{

		}
	}
}
