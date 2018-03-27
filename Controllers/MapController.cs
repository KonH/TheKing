using System.Collections.Generic;
using TheKing.Controllers.Community;

namespace TheKing.Controllers {
	class MapController : StateController, IUpdateHandler {

		struct Point {
			public int X { get; }
			public int Y { get; }

			public Point(int x, int y) {
				X = x;
				Y = y;
			}
		}

		enum Direction {
			North,
			East,
			South,
			West,
		}

		class Location {
			public string  Name  { get; }
			public Country Owner { get; }

			public Location(string name, Country owner = null) {
				Name  = name;
				Owner = owner;
			}
		}

		Direction[] AllDirections = {
			Direction.North,
			Direction.East,
			Direction.South,
			Direction.West,
		};

		Point                       _position;
		Dictionary<Point, Location> _locations;

		public MapController(GameState state):base(state) {
			_locations = new Dictionary<Point, Location>();

			var oceanLoc = new Location("Great Ocean");

			var humanRace = new Race(RaceId.Human);
			var yourKingdom = new Country("Your Kingdom", humanRace);

			var goblinRace = new Race(RaceId.Goblin);
			var goblinKingdom = new Country("Goblington", goblinRace);

			_locations.Add(new Point(0, 0), new Location("Home Planes", yourKingdom));

			_locations.Add(new Point(0, 1), new Location("Snow Mountains", goblinKingdom));
			_locations.Add(new Point(0, 2), new Location("North Pole"));

			_locations.Add(new Point(-1, 0), new Location("White Coast"));
			_locations.Add(new Point(-2, 0), oceanLoc);

			_locations.Add(new Point(1, 0), new Location("Wild Forests"));
			_locations.Add(new Point(2, 0), oceanLoc);

			_locations.Add(new Point(0, -1), new Location("Death Barrens"));
			_locations.Add(new Point(0, -2), oceanLoc);

			ResetPosition();
		}

		void ResetPosition() {
			_position = new Point(0, 0);
		}

		public void Update() {
			var curLocation = GetLocationAt(_position);
			DescribeLocation(curLocation);

			foreach ( var dir in AllDirections ) {
				var locationAt = GetLocationAt(_position, dir);
				if ( locationAt != null ) {
					var title = string.Format(Content.look_at, dir);
					State.Context.AddCase(title, () => {
						_position = TransformPoint(_position, dir);
					});
				}
			}
			State.Context.AddBackCaseWith(ResetPosition);
		}

		void DescribeLocation(Location loc) {
			State.Out.WriteFormat(Content.here_is, loc.Name);
			if ( loc.Owner != null ) {
				var raceName = Content.ResourceManager.GetString("race_" + loc.Owner.Kind.Id);
				State.Out.WriteFormat(Content.here_live, loc.Owner.Name, raceName);
			} else {
				State.Out.Write(Content.here_empty);
			}
		}

		Location GetLocationAt(Point pos) {
			if ( _locations.TryGetValue(pos, out var loc) ) {
				return loc;
			}
			return null;
		}

		Location GetLocationAt(Point pos, Direction dir) {
			return GetLocationAt(TransformPoint(pos, dir));
		}

		Point TransformPoint(Point p, Direction dir) {
			switch ( dir ) {
				case Direction.North: return new Point(p.X    , p.Y + 1);
				case Direction.East : return new Point(p.X + 1, p.Y    );
				case Direction.South: return new Point(p.X    , p.Y - 1);
				case Direction.West : return new Point(p.X - 1, p.Y    );
			}
			return p;
		}
	}
}
