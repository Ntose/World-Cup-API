using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.Services;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using DataLayer;
using DataLayer.Models;
using WpfApp.Properties;

namespace WpfApp.ViewModels
{
	public class MainViewModel : INotifyPropertyChanged
	{
		private readonly DataLayer.DataManager _dataService;

		public ObservableCollection<Team> Teams { get; set; } = new ObservableCollection<Team>();
		public ObservableCollection<Team> Opponents { get; set; } = new ObservableCollection<Team>();

		private Team _selectedTeam;
		public Team SelectedTeam
		{
			get => _selectedTeam;
			set
			{
				if (_selectedTeam != value)
				{
					_selectedTeam = value;
					OnPropertyChanged();
					OnPropertyChanged(nameof(HasSelectedTeam));
					LoadOpponentsForSelectedTeam();
				}
			}
		}

		private Team _selectedOpponent;
		public Team SelectedOpponent
		{
			get => _selectedOpponent;
			set
			{
				if (_selectedOpponent != value)
				{
					_selectedOpponent = value;
					OnPropertyChanged();
					OnPropertyChanged(nameof(HasSelectedOpponent));
					UpdateMatchResult();
				}
			}
		}

		private bool _isLoading;
		public bool IsLoading
		{
			get => _isLoading;
			set
			{
				_isLoading = value;
				OnPropertyChanged();
			}
		}

		private string _matchResultText;
		public string MatchResultText
		{
			get => _matchResultText;
			set
			{
				_matchResultText = value;
				OnPropertyChanged();
			}
		}

		public bool HasSelectedTeam => SelectedTeam != null;
		public bool HasSelectedOpponent => SelectedOpponent != null;

		public string SettingsMenuText => "File";
		public string ChangeSettingsText => "Settings";
		public string ExitText => "Exit";
		public string SelectTeamText => "Select Team";
		public string SelectOpponentText => "Select Opponent";
		public string SelectedTeamInfoText => "Team Info";
		public string OpponentTeamInfoText => "Opponent Info";
		public string StatusText => "Ready";
		public string ChampionshipText => "World Cup";
		public string LoadingText => "Loading...";

		public MainViewModel()
		{
			_dataService = new DataManager();
			LoadTeams();
		}

		public async Task LoadTeams()
		{
			IsLoading = true;

			var teams = await _dataService.GetTeamsAsync(ConfigurationManager.SelectedChampionship);
			Teams.Clear();
			foreach (var team in teams)
			{
				Teams.Add(team);
			}

			await LoadFavoriteTeamAsync(); // Ensure this is awaited

			IsLoading = false;
		}




		public void LoadOpponentsForSelectedTeam()
		{
			Opponents.Clear();
			if (SelectedTeam == null) return;

			foreach (var opponent in Teams)
			{
				if (opponent != SelectedTeam)
				{
					Opponents.Add(opponent);
				}
			}
		}

		public void UpdateMatchResult()
		{
			if (SelectedTeam != null && SelectedOpponent != null)
			{
				// Placeholder logic for match result
				MatchResultText = $"{SelectedTeam.Country} vs {SelectedOpponent.Country}: {SelectedTeam.GoalsFor} : {SelectedOpponent.GoalsFor}";
			}
			else
			{
				MatchResultText = string.Empty;
			}
		}

		public void ReloadTexts()
		{
			// Placeholder for language switch implementation
			OnPropertyChanged(string.Empty); // refresh all bindings
		}
		private async Task LoadFavoriteTeamAsync()
		{
			var fileName = ConfigurationManager.SelectedChampionship == "men"
				? "favorite_team_men.txt"
				: "favorite_team_women.txt";

			if (!File.Exists(fileName)) return;

			string savedLine = File.ReadAllText(fileName).Trim();

			// Extract FIFA code from format: "Country (FRA)"
			int start = savedLine.IndexOf('(');
			int end = savedLine.IndexOf(')');

			if (start == -1 || end == -1 || end <= start) return;

			string fifaCode = savedLine.Substring(start + 1, end - start - 1);

			// Match the team from the list of loaded teams by FifaCode
			var matchedTeam = Teams.FirstOrDefault(t => t.FifaCode == fifaCode);
			if (matchedTeam != null)
			{
				SelectedTeam = matchedTeam; // this sets _selectedTeam and raises OnPropertyChanged
			}
		}



		public event PropertyChangedEventHandler PropertyChanged;
		public void OnPropertyChanged([CallerMemberName] string name = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
		}
	}
}
