using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace DataLayer.Models
{
	public class Match
	{
		public string Venue { get; set; }
		public string Location { get; set; }
		public string Status { get; set; }

		[JsonProperty("datetime")]
		public DateTime DateTime { get; set; }

		public string Stage { get; set; }

		[JsonProperty("home_team")]
		public Team HomeTeam { get; set; }

		[JsonProperty("away_team")]
		public Team AwayTeam { get; set; }

		public string Winner { get; set; }

		[JsonProperty("home_team_events")]
		public List<MatchEvent> HomeTeamEvents { get; set; }
		[JsonProperty("home_team_statistics")]
		public TeamStatistics HomeTeamStatistics { get; set; }

		[JsonProperty("away_team_statistics")]
		public TeamStatistics AwayTeamStatistics { get; set; }

		[JsonProperty("home_team_goals")]
		public int HomeTeamGoals { get; set; }

		[JsonProperty("away_team_goals")]
		public int AwayTeamGoals { get; set; }

		public int Attendance { get; set; }
	}

	public class TeamStatistics
	{
		[JsonProperty("starting_eleven")]
		public List<Player> StartingEleven { get; set; }

		[JsonProperty("substitutes")]
		public List<Player> Substitutes { get; set; }
	}
}
