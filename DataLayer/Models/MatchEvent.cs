using Newtonsoft.Json;

namespace DataLayer.Models
{
	public class MatchEvent
	{

		[JsonProperty("type_of_event")]
		public string TypeOfEvent { get; set; }

		[JsonProperty("player")]
		public string Player { get; set; }
	}
}