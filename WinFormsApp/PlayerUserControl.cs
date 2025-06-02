using DataLayer.Models;
using System.Windows.Forms;
using System.Drawing;
using System;
using System.IO;

public partial class PlayerUserControl : UserControl
{
	private PictureBox playerPictureBox;
	private Label playerNameLabel;
	private Label playerNumberLabel;
	private Label playerPositionLabel;
	private PictureBox captainIndicator;
	private PictureBox favoriteIndicator;
	private ContextMenuStrip contextMenu;
	private ToolStripMenuItem favoriteMenuItem;
	private ToolStripMenuItem unfavoriteMenuItem;

	public Player Player { get; set; }
	public bool IsSelected { get; set; }

	// Events
	public event EventHandler<Player> PlayerClicked;
	public event EventHandler<Player> FavoriteToggled;

	public PlayerUserControl(Player player)
	{
		InitializeComponent();
		Player = player;
		SetupPlayerDisplay();
		SetupContextMenu();
		SetupDragDrop();
	}

	private void InitializeComponent()
	{
		// Initialize all controls
		this.playerPictureBox = new PictureBox();
		this.playerNameLabel = new Label();
		this.playerNumberLabel = new Label();
		this.playerPositionLabel = new Label();
		this.captainIndicator = new PictureBox();
		this.favoriteIndicator = new PictureBox();
		this.contextMenu = new ContextMenuStrip();
		this.favoriteMenuItem = new ToolStripMenuItem();
		this.unfavoriteMenuItem = new ToolStripMenuItem();

		this.SuspendLayout();

		// 
		// PlayerUserControl
		// 
		this.BackColor = Color.White;
		this.BorderStyle = BorderStyle.FixedSingle;
		this.Size = new Size(200, 120);
		this.Cursor = Cursors.Hand;
		this.Click += PlayerUserControl_Click;

		// 
		// playerPictureBox
		// 
		this.playerPictureBox.Location = new Point(10, 10);
		this.playerPictureBox.Size = new Size(60, 60);
		this.playerPictureBox.SizeMode = PictureBoxSizeMode.StretchImage;
		this.playerPictureBox.BorderStyle = BorderStyle.FixedSingle;
		this.playerPictureBox.Click += PlayerUserControl_Click;
		this.Controls.Add(this.playerPictureBox);

		// 
		// playerNameLabel
		// 
		this.playerNameLabel.Location = new Point(80, 10);
		this.playerNameLabel.Size = new Size(100, 20);
		this.playerNameLabel.Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Bold);
		this.playerNameLabel.Click += PlayerUserControl_Click;
		this.Controls.Add(this.playerNameLabel);

		// 
		// playerNumberLabel
		// 
		this.playerNumberLabel.Location = new Point(80, 35);
		this.playerNumberLabel.Size = new Size(50, 20);
		this.playerNumberLabel.Font = new Font("Microsoft Sans Serif", 8F);
		this.playerNumberLabel.Click += PlayerUserControl_Click;
		this.Controls.Add(this.playerNumberLabel);

		// 
		// playerPositionLabel
		// 
		this.playerPositionLabel.Location = new Point(80, 55);
		this.playerPositionLabel.Size = new Size(100, 20);
		this.playerPositionLabel.Font = new Font("Microsoft Sans Serif", 8F);
		this.playerPositionLabel.Click += PlayerUserControl_Click;
		this.Controls.Add(this.playerPositionLabel);

		// 
		// captainIndicator
		// 
		this.captainIndicator.Location = new Point(10, 75);
		this.captainIndicator.Size = new Size(20, 20);
		this.captainIndicator.SizeMode = PictureBoxSizeMode.StretchImage;
		this.captainIndicator.Visible = false;
		this.captainIndicator.Click += PlayerUserControl_Click;
		this.Controls.Add(this.captainIndicator);

		// 
		// favoriteIndicator
		// 
		this.favoriteIndicator.Location = new Point(170, 10);
		this.favoriteIndicator.Size = new Size(20, 20);
		this.favoriteIndicator.SizeMode = PictureBoxSizeMode.StretchImage;
		this.favoriteIndicator.Cursor = Cursors.Hand;
		this.favoriteIndicator.Click += FavoriteIndicator_Click;
		this.Controls.Add(this.favoriteIndicator);

		// 
		// contextMenu
		// 
		this.favoriteMenuItem.Text = "Add to Favorites";
		this.favoriteMenuItem.Click += FavoriteMenuItem_Click;
		this.unfavoriteMenuItem.Text = "Remove from Favorites";
		this.unfavoriteMenuItem.Click += UnfavoriteMenuItem_Click;
		this.contextMenu.Items.Add(this.favoriteMenuItem);
		this.contextMenu.Items.Add(this.unfavoriteMenuItem);
		this.ContextMenuStrip = this.contextMenu;

		this.ResumeLayout(false);
	}

	private void SetupPlayerDisplay()
	{
		if (Player == null) return;

		// Display player information
		playerNameLabel.Text = Player.Name ?? "Unknown Player";
		playerNumberLabel.Text = $"#{Player.ShirtNumber}";
		playerPositionLabel.Text = Player.Position ?? "Unknown Position";

		// Load player image
		LoadPlayerImage();

		// Show captain indicator
		if (Player.Captain)
		{
			captainIndicator.Visible = true;
			captainIndicator.Image = CreateCaptainImage();
		}

		// Update favorite indicator
		UpdateFavoriteIndicator();
	}

	private void LoadPlayerImage()
	{
		try
		{
			// Try to load player image from file or URL
			if (!string.IsNullOrEmpty(Player.ImagePath) && File.Exists(Player.ImagePath))
			{
				playerPictureBox.Image = Image.FromFile(Player.ImagePath);
			}
			else
			{
				// Load default player image
				playerPictureBox.Image = CreateDefaultPlayerImage();
			}
		}
		catch (Exception)
		{
			// If loading fails, use default image
			playerPictureBox.Image = CreateDefaultPlayerImage();
		}
	}

	private Image CreateDefaultPlayerImage()
	{
		// Create a simple default player image
		Bitmap defaultImage = new Bitmap(60, 60);
		using (Graphics g = Graphics.FromImage(defaultImage))
		{
			g.FillRectangle(Brushes.LightGray, 0, 0, 60, 60);
			g.DrawRectangle(Pens.Black, 0, 0, 59, 59);

			// Draw a simple person icon
			g.FillEllipse(Brushes.DarkGray, 20, 15, 20, 20); // Head
			g.FillRectangle(Brushes.DarkGray, 18, 35, 24, 20); // Body

			// Draw jersey number
			using (Font font = new Font("Arial", 8, FontStyle.Bold))
			{
				string number = Player?.ShirtNumber.ToString() ?? "?";
				SizeF textSize = g.MeasureString(number, font);
				g.DrawString(number, font, Brushes.White,
					(60 - textSize.Width) / 2, 38);
			}
		}
		return defaultImage;
	}

	private Image CreateCaptainImage()
	{
		// Create captain indicator (C in a circle)
		Bitmap captainImage = new Bitmap(20, 20);
		using (Graphics g = Graphics.FromImage(captainImage))
		{
			g.FillEllipse(Brushes.Gold, 0, 0, 20, 20);
			g.DrawEllipse(Pens.DarkGoldenrod, 0, 0, 19, 19);
			using (Font font = new Font("Arial", 10, FontStyle.Bold))
			{
				g.DrawString("C", font, Brushes.Black, 6, 3);
			}
		}
		return captainImage;
	}

	private Image CreateStarImage(bool filled)
	{
		Bitmap starImage = new Bitmap(20, 20);
		using (Graphics g = Graphics.FromImage(starImage))
		{
			// Simple star shape using points
			Point[] starPoints = {
				new Point(10, 2), new Point(12, 8), new Point(18, 8),
				new Point(13, 12), new Point(15, 18), new Point(10, 15),
				new Point(5, 18), new Point(7, 12), new Point(2, 8),
				new Point(8, 8)
			};

			if (filled)
			{
				g.FillPolygon(Brushes.Gold, starPoints);
			}
			else
			{
				g.DrawPolygon(new Pen(Color.Gray, 2), starPoints);
			}
		}
		return starImage;
	}

	private void UpdateFavoriteIndicator()
	{
		if (Player == null) return;

		favoriteIndicator.Image = CreateStarImage(Player.IsFavorite);

		// Update context menu
		favoriteMenuItem.Visible = !Player.IsFavorite;
		unfavoriteMenuItem.Visible = Player.IsFavorite;
	}

	private void SetupContextMenu()
	{
		// Context menu is already set up in InitializeComponent
		UpdateFavoriteIndicator();
	}

	private void SetupDragDrop()
	{
		this.AllowDrop = true;
		this.MouseDown += PlayerUserControl_MouseDown;
		this.DragEnter += PlayerUserControl_DragEnter;
		this.DragDrop += PlayerUserControl_DragDrop;
	}

	// Event handlers
	private void PlayerUserControl_Click(object sender, EventArgs e)
	{
		IsSelected = !IsSelected;
		UpdateSelectionAppearance();
		PlayerClicked?.Invoke(this, Player);
	}

	private void FavoriteIndicator_Click(object sender, EventArgs e)
	{
		ToggleFavorite();
	}

	private void FavoriteMenuItem_Click(object sender, EventArgs e)
	{
		ToggleFavorite();
	}

	private void UnfavoriteMenuItem_Click(object sender, EventArgs e)
	{
		ToggleFavorite();
	}

	private void ToggleFavorite()
	{
		if (Player != null)
		{
			Player.IsFavorite = !Player.IsFavorite;
			UpdateFavoriteIndicator();
			FavoriteToggled?.Invoke(this, Player);
		}
	}

	private void UpdateSelectionAppearance()
	{
		if (IsSelected)
		{
			this.BackColor = Color.LightBlue;
			this.BorderStyle = BorderStyle.Fixed3D;
		}
		else
		{
			this.BackColor = Color.White;
			this.BorderStyle = BorderStyle.FixedSingle;
		}
	}

	// Drag and Drop functionality
	private void PlayerUserControl_MouseDown(object sender, MouseEventArgs e)
	{
		if (e.Button == MouseButtons.Left && Player != null)
		{
			this.DoDragDrop(Player, DragDropEffects.Move | DragDropEffects.Copy);
		}
	}

	private void PlayerUserControl_DragEnter(object sender, DragEventArgs e)
	{
		if (e.Data.GetDataPresent(typeof(Player)))
		{
			e.Effect = DragDropEffects.Move;
		}
		else
		{
			e.Effect = DragDropEffects.None;
		}
	}

	private void PlayerUserControl_DragDrop(object sender, DragEventArgs e)
	{
		if (e.Data.GetDataPresent(typeof(Player)))
		{
			Player droppedPlayer = (Player)e.Data.GetData(typeof(Player));
			// Handle the drop logic here
			// You might want to raise an event or call a method to handle player position swap
		}
	}

	// Public methods
	public void UpdatePlayer(Player player)
	{
		Player = player;
		SetupPlayerDisplay();
	}

	public void SetSelected(bool selected)
	{
		IsSelected = selected;
		UpdateSelectionAppearance();
	}

	// Clean up resources
	protected override void Dispose(bool disposing)
	{
		if (disposing)
		{
			playerPictureBox?.Image?.Dispose();
			captainIndicator?.Image?.Dispose();
			favoriteIndicator?.Image?.Dispose();
			contextMenu?.Dispose();
		}
		base.Dispose(disposing);
	}
}