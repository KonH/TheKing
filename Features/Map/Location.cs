using TheKing.Features.Countries;

namespace TheKing.Features.Map {
	class Location {
		public Point   Point      { get; }
		public string  Name       { get; }
		public Country Owner      { get; set; }
		public bool    Reachable  { get; }
		public double  Difficulty { get; }
		public int     Distance   { get; }

		public Location(Point point, string name, bool reachable, double difficulty, int distance, Country owner = null) {
			Point      = point;
			Name       = name;
			Reachable  = reachable;
			Difficulty = difficulty;
			Distance   = distance;
			Owner      = owner;
		}
	}
}
