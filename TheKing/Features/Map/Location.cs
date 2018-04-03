using TheKing.Features.Countries;

namespace TheKing.Features.Map {
	class Location {
		public Point        Point      { get; }
		public LocationType Type       { get; }
		public string       Name       { get; }
		public Country      Owner      { get; set; }
		public double       Difficulty { get; }
		public int          Distance   { get; }

		public bool Reachable => Type != LocationType.Sea;

		public Location(Point point, LocationType type, string name, double difficulty, int distance, Country owner = null) {
			Point      = point;
			Type       = type;
			Name       = name;
			Difficulty = difficulty;
			Distance   = distance;
			Owner      = owner;
		}
	}
}
