using System;
using System.Windows.Forms;
using WinFormsApp.Resources;
using WorldCupStats.WinFormsApp.Helpers;

namespace WorldCupStats.WinFormsApp.Forms
{
	public partial class SettingsForm : Form
	{

		private ComboBox comboLanguage;
		private Button btnOk;
		private Button btnCancel;
		private Label label1;
		private Label label2;
		private ComboBox comboTournament;

		public SettingsForm()
		{
			InitializeComponent();
		}

		private void btnOK_Click(object sender, EventArgs e)
		{
			if (comboLanguage.SelectedItem == null || comboTournament.SelectedItem == null)
			{
				MessageBox.Show(Resources.modalSettingsError);
				return;
			}

			string tournament = comboTournament.SelectedItem.ToString() == Resources.TeamMen ? "men" : "women";
			string language = comboLanguage.SelectedItem.ToString() == Resources.LanguageCroatian ? "hr" : "en";

			// Save settings
			ConfigManager.SaveSettings(language, tournament);

			this.DialogResult = DialogResult.OK;
			this.Close();
		}

		private void InitializeComponent()
		{
			comboLanguage = new ComboBox();
			comboTournament = new ComboBox();
			btnOk = new Button();
			btnCancel = new Button();
			label1 = new Label();
			label2 = new Label();
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
			btnOk.Click += btnOk_Click_1;
			// 
			// btnCancel
			// 
			btnCancel.Location = new Point(176, 212);
			btnCancel.Name = "btnCancel";
			btnCancel.Size = new Size(94, 29);
			btnCancel.TabIndex = 3;
			btnCancel.Text = "Cancel";
			btnCancel.UseVisualStyleBackColor = true;
			btnCancel.Click += btnCancel_Click_1;
			// 
			// label1
			// 
			label1.AutoSize = true;
			label1.Location = new Point(12, 15);
			label1.Name = "label1";
			label1.Size = new Size(80, 18);
			label1.TabIndex = 4;
			label1.Text = "Language:";
			label1.Click += label1_Click;
			// 
			// label2
			// 
			label2.AutoSize = true;
			label2.Location = new Point(12, 47);
			label2.Name = "label2";
			label2.Size = new Size(56, 18);
			label2.TabIndex = 5;
			label2.Text = "Teams:";
			// 
			// SettingsForm
			// 
			ClientSize = new Size(282, 253);
			Controls.Add(label2);
			Controls.Add(label1);
			Controls.Add(btnCancel);
			Controls.Add(btnOk);
			Controls.Add(comboTournament);
			Controls.Add(comboLanguage);
			Name = "SettingsForm";
			Load += SettingsForm_Load;
			ResumeLayout(false);
			PerformLayout();
		}

		private void btnCancel_Click(object sender, EventArgs e)
		{
			//DialogResult = DialogResult.Cancel;
			//Close();
		}

		private void label1_Click(object sender, EventArgs e)
		{

		}

		private void SettingsForm_Load(object sender, EventArgs e)
		{
			label1.Text = Resources.LanguagePicker;
			label2.Text = Resources.TeamPicker;
			comboLanguage.Items[0] = Resources.LanguageEnglish;
			comboLanguage.Items[1] = Resources.LanguageCroatian;
			comboTournament.Items[0] = Resources.TeamMen;
			comboTournament.Items[1] = Resources.TeamWomen;
			btnOk.Text = Resources.btnApply;
			btnCancel.Text = Resources.btnCancel;
			AppSettings appSettings = ConfigManager.LoadSettings();
			comboLanguage.SelectedItem = appSettings.Language == "en" ? "English" : comboLanguage.Items[1];
			if (appSettings.Tournament == "men") { }
				comboTournament.SelectedItem = comboTournament.Items[0];
			
			else
				comboTournament.SelectedItem = comboTournament.Items[1];
			this.Text = Resources.SettingsTitle;
			this.AcceptButton = btnOk;
			//this.CancelButton = btnCancel;

		}

		private void btnOk_Click_1(object sender, EventArgs e)
		{
			if (comboLanguage.SelectedItem == null || comboTournament.SelectedItem == null)
			{
				MessageBox.Show(Resources.modalSettingsError);
				return;
			}

			string tournament = comboTournament.SelectedItem.ToString() == Resources.TeamMen ? "men" : "women";
			string language = comboLanguage.SelectedItem.ToString() == Resources.LanguageCroatian ? "hr" : "en";

			// Save settings
			ConfigManager.SaveSettings(language, tournament);
				
			this.DialogResult = DialogResult.OK;
			this.Close();
		}

		private void btnCancel_Click_1(object sender, EventArgs e)
		{
			this.DialogResult = DialogResult.Cancel;
			this.Close();
		}

		private void label3_Click(object sender, EventArgs e)
		{

		}
	}
}
