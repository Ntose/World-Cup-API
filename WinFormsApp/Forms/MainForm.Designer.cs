namespace WinFormsApp
{
	partial class MainForm
	{
		/// <summary>
		///  Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		///  Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		///  Required method for Designer support - do not modify
		///  the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			menuStrip1 = new MenuStrip();
			settingToolStripMenuItem = new ToolStripMenuItem();
			closeToolStripMenuItem = new ToolStripMenuItem();
			lbTeamSelect = new Label();
			comboTeams = new ComboBox();
			btnConfirm = new Button();
			pnlFavoritePlayers = new Panel();
			pnlAllPlayers = new Panel();
			lbAllPlayers = new Label();
			lbFavoritePlayers = new Label();
			menuStrip1.SuspendLayout();
			SuspendLayout();
			// 
			// menuStrip1
			// 
			menuStrip1.ImageScalingSize = new Size(20, 20);
			menuStrip1.Items.AddRange(new ToolStripItem[] { settingToolStripMenuItem, closeToolStripMenuItem });
			menuStrip1.Location = new Point(0, 0);
			menuStrip1.Name = "menuStrip1";
			menuStrip1.Size = new Size(800, 26);
			menuStrip1.TabIndex = 0;
			menuStrip1.Text = "menuStrip1";
			// 
			// settingToolStripMenuItem
			// 
			settingToolStripMenuItem.Name = "settingToolStripMenuItem";
			settingToolStripMenuItem.Size = new Size(86, 22);
			settingToolStripMenuItem.Text = "Settings";
			settingToolStripMenuItem.Click += settingToolStripMenuItem_Click;
			// 
			// closeToolStripMenuItem
			// 
			closeToolStripMenuItem.Name = "closeToolStripMenuItem";
			closeToolStripMenuItem.Size = new Size(62, 22);
			closeToolStripMenuItem.Text = "Close";
			closeToolStripMenuItem.Click += closeToolStripMenuItem_Click;
			// 
			// lbTeamSelect
			// 
			lbTeamSelect.AutoSize = true;
			lbTeamSelect.Location = new Point(12, 44);
			lbTeamSelect.Name = "lbTeamSelect";
			lbTeamSelect.Size = new Size(168, 18);
			lbTeamSelect.TabIndex = 1;
			lbTeamSelect.Text = "Resources.TeamSelect";
			// 
			// comboTeams
			// 
			comboTeams.FormattingEnabled = true;
			comboTeams.Location = new Point(12, 76);
			comboTeams.Name = "comboTeams";
			comboTeams.Size = new Size(151, 26);
			comboTeams.TabIndex = 2;
			comboTeams.SelectedIndexChanged += comboTeams_SelectedIndexChanged;
			// 
			// btnConfirm
			// 
			btnConfirm.Location = new Point(169, 76);
			btnConfirm.Name = "btnConfirm";
			btnConfirm.Size = new Size(94, 29);
			btnConfirm.TabIndex = 3;
			btnConfirm.Text = "Confirm";
			btnConfirm.UseVisualStyleBackColor = true;
			btnConfirm.Click += btnConfirm_Click;
			// 
			// pnlFavoritePlayers
			// 
			pnlFavoritePlayers.AllowDrop = true;
			pnlFavoritePlayers.Location = new Point(420, 164);
			pnlFavoritePlayers.Name = "pnlFavoritePlayers";
			pnlFavoritePlayers.Size = new Size(368, 265);
			pnlFavoritePlayers.TabIndex = 4;
			// 
			// pnlAllPlayers
			// 
			pnlAllPlayers.AllowDrop = true;
			pnlAllPlayers.Location = new Point(12, 164);
			pnlAllPlayers.Name = "pnlAllPlayers";
			pnlAllPlayers.Size = new Size(366, 265);
			pnlAllPlayers.TabIndex = 5;
			// 
			// lbAllPlayers
			// 
			lbAllPlayers.AutoSize = true;
			lbAllPlayers.Location = new Point(12, 120);
			lbAllPlayers.Name = "lbAllPlayers";
			lbAllPlayers.Size = new Size(168, 18);
			lbAllPlayers.TabIndex = 6;
			lbAllPlayers.Text = "Resources.AllPlayers";
			// 
			// lbFavoritePlayers
			// 
			lbFavoritePlayers.AutoSize = true;
			lbFavoritePlayers.Location = new Point(420, 120);
			lbFavoritePlayers.Name = "lbFavoritePlayers";
			lbFavoritePlayers.Size = new Size(208, 18);
			lbFavoritePlayers.TabIndex = 7;
			lbFavoritePlayers.Text = "Resources.FavoritePlayers";
			// 
			// MainForm
			// 
			AutoScaleDimensions = new SizeF(8F, 18F);
			AutoScaleMode = AutoScaleMode.Font;
			ClientSize = new Size(800, 450);
			Controls.Add(lbFavoritePlayers);
			Controls.Add(lbAllPlayers);
			Controls.Add(pnlAllPlayers);
			Controls.Add(pnlFavoritePlayers);
			Controls.Add(btnConfirm);
			Controls.Add(comboTeams);
			Controls.Add(lbTeamSelect);
			Controls.Add(menuStrip1);
			MainMenuStrip = menuStrip1;
			Name = "MainForm";
			Text = "Form1";
			Load += MainForm_Load;
			menuStrip1.ResumeLayout(false);
			menuStrip1.PerformLayout();
			ResumeLayout(false);
			PerformLayout();
		}

		#endregion

		private MenuStrip menuStrip1;
		private ToolStripMenuItem settingToolStripMenuItem;
		private ToolStripMenuItem closeToolStripMenuItem;
		private Label lbTeamSelect;
		private ComboBox comboTeams;
		private Button btnConfirm;
		private Panel pnlFavoritePlayers;
		private Panel pnlAllPlayers;
		private Label lbAllPlayers;
		private Label lbFavoritePlayers;
	}
}
