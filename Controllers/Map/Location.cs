using TheKing.Controllers.Kingdom;

namespace TheKing.Controllers.Map {
	class Location {
		public Point   Point { get; }
		public string  Name  { get; }
		public Country Owner { get; set; }

		public Location(Point point, string name, Country owner = null) {
			Point = point;
			Name  = name;
			Owner = owner;
		}
	}
}
