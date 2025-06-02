using DataLayer;
using System.Windows.Forms;
using System;
using WinFormsApp;

public partial class InitialSettingsForm : Form
{
	private ComboBox championshipComboBox;
	private ComboBox languageComboBox;
	private Button okButton;
	private Button cancelButton;
	private Label championshipLabel;
	private Label languageLabel;

	public string SelectedChampionship { get; private set; }
	public string SelectedLanguage { get; private set; }

	public InitialSettingsForm()
	{
		InitializeComponent();
		SetupControls();
	}

	private void InitializeComponent()
	{
		this.championshipLabel = new System.Windows.Forms.Label();
		this.championshipComboBox = new System.Windows.Forms.ComboBox();
		this.languageLabel = new System.Windows.Forms.Label();
		this.languageComboBox = new System.Windows.Forms.ComboBox();
		this.okButton = new System.Windows.Forms.Button();
		this.cancelButton = new System.Windows.Forms.Button();
		this.SuspendLayout();
		// 
		// championshipLabel
		// 
		this.championshipLabel.Location = new System.Drawing.Point(20, 20);
		this.championshipLabel.Name = "championshipLabel";
		this.championshipLabel.Size = new System.Drawing.Size(100, 23);
		this.championshipLabel.TabIndex = 0;
		this.championshipLabel.Text = "Championship:";
		// 
		// championshipComboBox
		// 
		this.championshipComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
		this.championshipComboBox.Location = new System.Drawing.Point(130, 20);
		this.championshipComboBox.Name = "championshipComboBox";
		this.championshipComboBox.Size = new System.Drawing.Size(150, 24);
		this.championshipComboBox.TabIndex = 1;
		// 
		// languageLabel
		// 
		this.languageLabel.Location = new System.Drawing.Point(20, 60);
		this.languageLabel.Name = "languageLabel";
		this.languageLabel.Size = new System.Drawing.Size(100, 23);
		this.languageLabel.TabIndex = 2;
		this.languageLabel.Text = "Language:";
		// 
		// languageComboBox
		// 
		this.languageComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
		this.languageComboBox.Location = new System.Drawing.Point(130, 60);
		this.languageComboBox.Name = "languageComboBox";
		this.languageComboBox.Size = new System.Drawing.Size(150, 24);
		this.languageComboBox.TabIndex = 3;
		// 
		// okButton
		// 
		this.okButton.Location = new System.Drawing.Point(130, 110);
		this.okButton.Name = "okButton";
		this.okButton.Size = new System.Drawing.Size(75, 23);
		this.okButton.TabIndex = 4;
		this.okButton.Text = "OK";
		this.okButton.UseVisualStyleBackColor = true;
		this.okButton.Click += new System.EventHandler(this.OkButton_Click);
		// 
		// cancelButton
		// 
		this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
		this.cancelButton.Location = new System.Drawing.Point(210, 110);
		this.cancelButton.Name = "cancelButton";
		this.cancelButton.Size = new System.Drawing.Size(75, 23);
		this.cancelButton.TabIndex = 5;
		this.cancelButton.Text = "Cancel";
		this.cancelButton.UseVisualStyleBackColor = true;
		// 
		// InitialSettingsForm
		// 
		this.AcceptButton = this.okButton;
		this.CancelButton = this.cancelButton;
		this.ClientSize = new System.Drawing.Size(332, 153);
		this.Controls.Add(this.championshipLabel);
		this.Controls.Add(this.championshipComboBox);
		this.Controls.Add(this.languageLabel);
		this.Controls.Add(this.languageComboBox);
		this.Controls.Add(this.okButton);
		this.Controls.Add(this.cancelButton);
		this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
		this.MaximizeBox = false;
		this.MinimizeBox = false;
		this.Name = "InitialSettingsForm";
		this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
		this.Text = "Initial Settings";
		this.Load += new System.EventHandler(this.InitialSettingsForm_Load);
		this.ResumeLayout(false);
	}

	private void SetupControls()
	{
		// Setup championship ComboBox
		championshipComboBox.Items.Add(new ComboBoxItem("Men's World Cup", "men"));
		championshipComboBox.Items.Add(new ComboBoxItem("Women's World Cup", "women"));
		championshipComboBox.DisplayMember = "Text";
		championshipComboBox.ValueMember = "Value";

		// Setup language ComboBox
		languageComboBox.Items.Add(new ComboBoxItem("Croatian", "hr"));
		languageComboBox.Items.Add(new ComboBoxItem("English", "en"));
		languageComboBox.DisplayMember = "Text";
		languageComboBox.ValueMember = "Value";

		// Load current settings
		LoadCurrentSettings();
	}

	private void LoadCurrentSettings()
	{
		// Load configuration to get current values
		ConfigurationManager.LoadConfiguration();

		// Set championship selection
		for (int i = 0; i < championshipComboBox.Items.Count; i++)
		{
			var item = (ComboBoxItem)championshipComboBox.Items[i];
			if (item.Value == ConfigurationManager.SelectedChampionship)
			{
				championshipComboBox.SelectedIndex = i;
				break;
			}
		}

		// Set language selection
		for (int i = 0; i < languageComboBox.Items.Count; i++)
		{
			var item = (ComboBoxItem)languageComboBox.Items[i];
			if (item.Value == ConfigurationManager.SelectedLanguage)
			{
				languageComboBox.SelectedIndex = i;
				break;
			}
		}

		// Set defaults if nothing was selected
		if (championshipComboBox.SelectedIndex == -1)
			championshipComboBox.SelectedIndex = 0; // Default to Men

		if (languageComboBox.SelectedIndex == -1)
			languageComboBox.SelectedIndex = 1; // Default to English
	}

	private void OkButton_Click(object sender, EventArgs e)
	{
		if (championshipComboBox.SelectedItem == null || languageComboBox.SelectedItem == null)
		{
			MessageBox.Show("Please select both championship and language.", "Validation Error",
						  MessageBoxButtons.OK, MessageBoxIcon.Warning);
			return;
		}

		// Get selected values
		SelectedChampionship = ((ComboBoxItem)championshipComboBox.SelectedItem).Value;
		SelectedLanguage = ((ComboBoxItem)languageComboBox.SelectedItem).Value;

		// Save to configuration
		ConfigurationManager.SelectedChampionship = SelectedChampionship;
		ConfigurationManager.SelectedLanguage = SelectedLanguage;
		ConfigurationManager.SaveConfiguration();

		DialogResult = DialogResult.OK;
		this.Close();
	}

	private void InitialSettingsForm_Load(object sender, EventArgs e)
	{
		// Form load event - could be used for additional initialization if needed
	}

	// Helper class for ComboBox items
	private class ComboBoxItem
	{
		public string Text { get; set; }
		public string Value { get; set; }

		public ComboBoxItem(string text, string value)
		{
			Text = text;
			Value = value;
		}

		public override string ToString()
		{
			return Text;
		}
	}
}