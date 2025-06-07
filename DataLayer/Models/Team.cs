using Newtonsoft.Json;

namespace DataLayer.Models
{
	public class Team
	{
		

		[JsonProperty("country")]
		public string Country { get; set; }

		[JsonProperty("fifa_code")]
		public string FifaCode { get; set; }

		[JsonProperty("games_played")]
		public int GamesPlayed { get; set; }

		[JsonProperty("wins")]
		public int Wins { get; set; }

		[JsonProperty("draws")]
		public int Draws { get; set; }

		[JsonProperty("losses")]
		public int Losses { get; set; }

		[JsonProperty("goals_for")]
		public int GoalsFor { get; set; }

		[JsonProperty("goals_against")]
		public int GoalsAgainst { get; set; }

		[JsonProperty("goal_differential")]
		public int GoalDifferential { get; set; }
		[JsonIgnore]
		public int Points => Wins * 3 + Draws;

		public override string ToString()
		{
			return $"{Country} ({FifaCode})";
		}
	}
}
