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
			lbTeamSelect.Location = new Point(12, 41);
			lbTeamSelect.Name = "lbTeamSelect";
			lbTeamSelect.Size = new Size(168, 18);
			lbTeamSelect.TabIndex = 1;
			lbTeamSelect.Text = "Resources.TeamSelect";
			// 
			// comboTeams
			// 
			comboTeams.FormattingEnabled = true;
			comboTeams.Location = new Point(12, 62);
			comboTeams.Name = "comboTeams";
			comboTeams.Size = new Size(151, 26);
			comboTeams.TabIndex = 2;
			// 
			// btnConfirm
			// 
			btnConfirm.Location = new Point(12, 158);
			btnConfirm.Name = "btnConfirm";
			btnConfirm.Size = new Size(94, 29);
			btnConfirm.TabIndex = 3;
			btnConfirm.Text = "Confirm";
			btnConfirm.UseVisualStyleBackColor = true;
			btnConfirm.Click += btnConfirm_Click;
			// 
			// MainForm
			// 
			AutoScaleDimensions = new SizeF(8F, 18F);
			AutoScaleMode = AutoScaleMode.Font;
			ClientSize = new Size(800, 450);
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
	}
}
