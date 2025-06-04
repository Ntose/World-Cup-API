using Newtonsoft.Json;

public class Player
{
	[JsonProperty("name")]
	public string Name { get; set; }

	[JsonProperty("shirt_number")]
	public int ShirtNumber { get; set; }

	[JsonProperty("position")]
	public string Position { get; set; }

	[JsonProperty("captain")]
	public bool Captain { get; set; }
	public int Goals { get; set; }
	public int YellowCards { get; set; }
	public string ImagePath { get; set; }
	public bool IsFavorite { get; set; }
}
