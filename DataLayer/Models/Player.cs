using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.Models
{
	public class Player
	{
		public string Name { get; set; }
		public int ShirtNumber { get; set; }
		public string Position { get; set; }
		public bool Captain { get; set; }
		public int Goals { get; set; }
		public int YellowCards { get; set; }
		public string ImagePath { get; set; }
		public bool IsFavorite { get; set; }
	}
}
