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
			btnSaveFavoritePlayers.Text = Resources.Resources.Save;
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
				var settings = ConfigManager.LoadSettings();
				var tournament = settings.Tournament; // "men" or "women"
				string fileName = tournament == "men" ? "favorite_team_men.txt" : "favorite_team_women.txt";
				File.WriteAllText(fileName, selectedTeam);

				MessageBox.Show($"{Resources.Resources.FavoriteTeam} {selectedTeam}");
				await LoadFavoritePlayers(); // reload players based on saved favorite
			}
			else
			{
				MessageBox.Show(Resources.Resources.PickFavoriteTeam);
			}
		}
		private void SaveFavoritePlayers()
		{
			var settings = ConfigManager.LoadSettings();
			if (settings == null) return;

			string fileName = $"favorite_players_{settings.Tournament}_{GetCurrentFifaCode()}.txt";


			var favoriteNames = pnlFavoritePlayers.Controls
				.OfType<PlayerCard>()
				.Select(card => card.PlayerData.Name)
				.ToList();

			File.WriteAllLines(fileName, favoriteNames);
		}

		private string GetCurrentFifaCode()
		{
			if (!File.Exists("favorite_team_" + ConfigManager.LoadSettings().Tournament + ".txt"))
				return "";

			string teamLine = File.ReadAllText("favorite_team_" + ConfigManager.LoadSettings().Tournament + ".txt");
			int start = teamLine.IndexOf('(');
			int end = teamLine.IndexOf(')');
			if (start == -1 || end == -1) return "";

			return teamLine.Substring(start + 1, 3); // e.g. FRA
		}


		private async Task LoadFavoritePlayers()
		{
			var settings = ConfigManager.LoadSettings();
			string fileName = settings.Tournament == "men"
				? "favorite_team_men.txt"
				: "favorite_team_women.txt";
			string favoriteTeam = File.ReadAllText(fileName).Trim();
			// Extract FIFA code
			int codeStart = favoriteTeam.IndexOf('(');
			int codeEnd = favoriteTeam.IndexOf(')');
			if (codeStart == -1 || codeEnd == -1 || codeEnd - codeStart != 4)
			{
				MessageBox.Show("Invalid team format.");
				return;
			}
			
			string fifaCode = favoriteTeam.Substring(codeStart + 1, 3);
			if (settings == null) return;

			
			string playerFile = $"favorite_players_{settings.Tournament}_{fifaCode}.txt";


			var favoriteNames = File.Exists(playerFile)
				? File.ReadAllLines(playerFile).ToList()
				: new List<string>();


			if (!File.Exists(fileName))
			{
				MessageBox.Show(Resources.Resources.FavoriteTeamNotFound);
				return;
			}


			


			string url = $"https://worldcup-vua.nullbit.hr/{settings.Tournament}/matches/country?fifa_code={fifaCode}";

			try
			{
				using var client = new HttpClient();
				string json = await client.GetStringAsync(url);
				var matches = JsonConvert.DeserializeObject<List<Match>>(json);
				var firstMatch = matches.FirstOrDefault();

				if (firstMatch == null)
				{
					MessageBox.Show("No matches found.");
					return;
				}

				var teamStats = firstMatch.HomeTeamStatistics;
				var players = (teamStats?.StartingEleven ?? new List<Player>())
					.Concat(teamStats?.Substitutes ?? new List<Player>())
					.ToList();

				pnlAllPlayers.Controls.Clear();
				pnlFavoritePlayers.Controls.Clear();

				foreach (var player in players)
				{
					var card = new PlayerCard(player, favoriteNames.Contains(player.Name));
					pnlAllPlayers.Controls.Add(card);
				}

				// Pre-select team in ComboBox
				comboTeams.SelectedItem = favoriteTeam;
			}
			catch (Exception ex)
			{
				MessageBox.Show($"Error loading players: {ex.Message}");
			}
		}




		private void comboTeams_SelectedIndexChanged(object sender, EventArgs e)
		{

		}

		private void btnSaveFavoritePlayers_Click(object sender, EventArgs e)
		{
			SaveFavoritePlayers();
			MessageBox.Show(Resources.Resources.FavoritePlayersSaved);
			Application.Restart();
		}
	}
}
