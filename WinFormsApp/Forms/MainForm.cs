using DataLayer;
using System.Windows.Forms.Design;
using WorldCupStats.WinFormsApp.Forms;
using WorldCupStats.WinFormsApp.Helpers;

namespace WinFormsApp
{
	public partial class MainForm : Form
	{
		public MainForm()
		{
			InitializeComponent();
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
				var teams = await ApiService.GetTeamsAsync(settings.Tournament); // "men" or "women"
				comboTeams.Items.Clear();

				foreach (var team in teams)
				{
					comboTeams.Items.Add(team.ToString());
				}

				// Load previously saved favorite team
				if (File.Exists("favorite_team.txt"))
				{
					var saved = File.ReadAllText("favorite_team.txt");
					comboTeams.SelectedItem = saved;
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show("Failed to load teams: " + ex.Message);
			}
		}

		private void btnConfirm_Click(object sender, EventArgs e)
		{
			if (comboTeams.SelectedItem != null)
			{
				var selectedTeam = comboTeams.SelectedItem.ToString();
				File.WriteAllText("favorite_team.txt", selectedTeam);

				MessageBox.Show($"{Resources.Resources.FavoriteTeam} {selectedTeam}");
			}
			else
			{
				MessageBox.Show(Resources.Resources.PickFavoriteTeam);
			}
		}
	}
}
