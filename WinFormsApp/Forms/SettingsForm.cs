using System;
using System.Windows.Forms;
using WorldCupStats.WinFormsApp.Helpers;

namespace WorldCupStats.WinFormsApp.Forms
{
	public partial class SettingsForm : Form
	{
		public SettingsForm()
		{
			InitializeComponent();
			comboLanguage.SelectedIndex = 0;
			comboTournament.SelectedIndex = 0;
		}

		private void btnOK_Click(object sender, EventArgs e)
		{
			string lang = comboLanguage.SelectedItem.ToString() == "English" ? "en" : "hr";
			string tour = comboTournament.SelectedItem.ToString() == "Men" ? "men" : "women";

			ConfigManager.SaveSettings(lang, tour);
			DialogResult = DialogResult.OK;
			Close();
		}

		private void InitializeComponent()
		{
			comboLanguage = new ComboBox();
			comboTournament = new ComboBox();
			btnOk = new Button();
			btnCancel = new Button();
			SuspendLayout();
			// 
			// comboLanguage
			// 
			comboLanguage.FormattingEnabled = true;
			comboLanguage.Items.AddRange(new object[] { "English", "Croatian" });
			comboLanguage.Location = new Point(119, 12);
			comboLanguage.Name = "comboLanguage";
			comboLanguage.Size = new Size(151, 26);
			comboLanguage.TabIndex = 0;
			// 
			// comboTournament
			// 
			comboTournament.FormattingEnabled = true;
			comboTournament.Items.AddRange(new object[] { "Men", "Women" });
			comboTournament.Location = new Point(119, 44);
			comboTournament.Name = "comboTournament";
			comboTournament.Size = new Size(151, 26);
			comboTournament.TabIndex = 1;
			// 
			// btnOk
			// 
			btnOk.Location = new Point(12, 212);
			btnOk.Name = "btnOk";
			btnOk.Size = new Size(94, 29);
			btnOk.TabIndex = 2;
			btnOk.Text = "Apply";
			btnOk.UseVisualStyleBackColor = true;
			// 
			// btnCancel
			// 
			btnCancel.Location = new Point(176, 212);
			btnCancel.Name = "btnCancel";
			btnCancel.Size = new Size(94, 29);
			btnCancel.TabIndex = 3;
			btnCancel.Text = "Cancel";
			btnCancel.UseVisualStyleBackColor = true;
			// 
			// SettingsForm
			// 
			ClientSize = new Size(282, 253);
			Controls.Add(btnCancel);
			Controls.Add(btnOk);
			Controls.Add(comboTournament);
			Controls.Add(comboLanguage);
			Name = "SettingsForm";
			ResumeLayout(false);
		}

		private void btnCancel_Click(object sender, EventArgs e)
		{
			DialogResult = DialogResult.Cancel;
			Close();
		}

		private ComboBox comboLanguage;
		private Button btnOk;
		private Button btnCancel;
		private ComboBox comboTournament;
	}
}
