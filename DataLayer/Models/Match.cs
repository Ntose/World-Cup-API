using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.Models
{
	public class Match
	{
		public string Venue { get; set; }
		public string Location { get; set; }
		public string Status { get; set; }
		public DateTime DateTime { get; set; }
		public string Stage { get; set; }
		public Team HomeTeam { get; set; }
		public Team AwayTeam { get; set; }
		public string Winner { get; set; }
		public int HomeTeamGoals { get; set; }
		public int AwayTeamGoals { get; set; }
		public List<Player> HomeTeamStartingEleven { get; set; }
		public List<Player> AwayTeamStartingEleven { get; set; }
		public List<Player> HomeTeamSubstitutes { get; set; }
		public List<Player> AwayTeamSubstitutes { get; set; }
		public int Attendance { get; set; }
	}
}
