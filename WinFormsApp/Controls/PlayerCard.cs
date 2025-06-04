using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using DataLayer.Models;

namespace WinFormsApp.Controls
{
	public partial class PlayerCard : UserControl
	{
		public Player PlayerData { get; private set; }

		private Label lblName;
		private Label lblNumber;
		private Label lblPosition;
		private PictureBox picCaptain;
		private PictureBox picFavorite;
		private PictureBox picPlayerImage;

		public PlayerCard(Player player, bool isFavorite = false)
		{
			PlayerData = player;
			InitializeComponent();

			// Logic/config here
			lblName.Text = player.Name;
			lblNumber.Text = $"#{player.ShirtNumber}";
			lblPosition.Text = player.Position;

			picCaptain.Visible = player.Captain;
			picFavorite.Visible = isFavorite;

			// Load images
			using (var ms = new MemoryStream(Resources.Resources.DefaultPlayer))
				picPlayerImage.Image = Image.FromStream(ms);

			using (var ms = new MemoryStream(Resources.Resources.Star))
				picCaptain.Image = Image.FromStream(ms);

			using (var ms = new MemoryStream(Resources.Resources.Heart))
				picFavorite.Image = Image.FromStream(ms);

			// Control properties
			BorderStyle = BorderStyle.FixedSingle;
			Size = new Size(230, 70);

			// Events
			MouseDown += (s, e) => DoDragDrop(this, DragDropEffects.Move);
		}


		private void InitializeComponent()
		{
			lblName = new Label();
			lblNumber = new Label();
			lblPosition = new Label();
			picCaptain = new PictureBox();
			picFavorite = new PictureBox();
			picPlayerImage = new PictureBox();
			((System.ComponentModel.ISupportInitialize)picCaptain).BeginInit();
			((System.ComponentModel.ISupportInitialize)picFavorite).BeginInit();
			((System.ComponentModel.ISupportInitialize)picPlayerImage).BeginInit();
			SuspendLayout();
			// 
			// lblName
			// 
			lblName.Location = new Point(70, 5);
			lblName.Name = "lblName";
			lblName.Size = new Size(150, 20);
			lblName.TabIndex = 0;
			// 
			// lblNumber
			// 
			lblNumber.Location = new Point(70, 25);
			lblNumber.Name = "lblNumber";
			lblNumber.Size = new Size(50, 20);
			lblNumber.TabIndex = 1;
			lblNumber.Click += lblNumber_Click;
			// 
			// lblPosition
			// 
			lblPosition.Location = new Point(70, 45);
			lblPosition.Name = "lblPosition";
			lblPosition.Size = new Size(100, 20);
			lblPosition.TabIndex = 2;
			// 
			// picCaptain
			// 
			picCaptain.Location = new Point(130, 25);
			picCaptain.Name = "picCaptain";
			picCaptain.Size = new Size(20, 20);
			picCaptain.SizeMode = PictureBoxSizeMode.StretchImage;
			picCaptain.TabIndex = 4;
			picCaptain.TabStop = false;
			picCaptain.Visible = false;
			// 
			// picFavorite
			// 
			picFavorite.Location = new Point(155, 25);
			picFavorite.Name = "picFavorite";
			picFavorite.Size = new Size(20, 20);
			picFavorite.SizeMode = PictureBoxSizeMode.StretchImage;
			picFavorite.TabIndex = 5;
			picFavorite.TabStop = false;
			picFavorite.Visible = false;
			// 
			// picPlayerImage
			// 
			picPlayerImage.Location = new Point(5, 5);
			picPlayerImage.Name = "picPlayerImage";
			picPlayerImage.Size = new Size(60, 60);
			picPlayerImage.SizeMode = PictureBoxSizeMode.StretchImage;
			picPlayerImage.TabIndex = 3;
			picPlayerImage.TabStop = false;
			// 
			// PlayerCard
			// 
			BorderStyle = BorderStyle.FixedSingle;
			Controls.Add(lblName);
			Controls.Add(lblNumber);
			Controls.Add(lblPosition);
			Controls.Add(picPlayerImage);
			Controls.Add(picCaptain);
			Controls.Add(picFavorite);
			Name = "PlayerCard";
			Size = new Size(230, 70);
			((System.ComponentModel.ISupportInitialize)picCaptain).EndInit();
			((System.ComponentModel.ISupportInitialize)picFavorite).EndInit();
			((System.ComponentModel.ISupportInitialize)picPlayerImage).EndInit();
			ResumeLayout(false);
		}

		private void lblNumber_Click(object sender, EventArgs e)
		{

		}
	}
}
