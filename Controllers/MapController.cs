using System.Collections.Generic;
using System.Linq;
using TheKing.Controllers.Kingdom;
using TheKing.Controllers.Map;

namespace TheKing.Controllers {
	class MapController : StateController, IUpdateHandler {

		enum Direction {
			North,
			East,
			South,
			West,
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

			AddLocation(new Location(new Point(0, 0), "Home Planes", State.Country.PlayerCountry));

			AddLocation(new Location(new Point(0, 1), "Snow Mountains", State.Country.EnemyCountry));
			AddLocation(new Location(new Point(0, 2), "North Pole"));

			AddLocation(new Location(new Point(-1, 0), "White Coast"));
			AddLocation(new Location(new Point(-2, 0), "Great Ocean"));

			AddLocation(new Location(new Point(1, 0), "Wild Forests"));
			AddLocation(new Location(new Point(2, 0), "Great Ocean"));

			AddLocation(new Location(new Point(0, -1), "Death Barrens"));
			AddLocation(new Location(new Point(0, -2), "Great Ocean"));

			State.Country.OnCountryRemoved += (c, r) => RemoveCountry(c);

			ResetPosition();
		}

		void AddLocation(Location loc) {
			_locations.Add(loc.Point, loc);
		}

		void ResetPosition() {
			_position = new Point(0, 0);
		}

		void RemoveCountry(Country c) {
			foreach ( var loc in _locations ) {
				if ( loc.Value.Owner == c ) {
					loc.Value.Owner = null;
				}
			}
		}

		public void Update() {
			var curLocation = GetLocationAt(_position);
			DescribeLocation(curLocation);

			foreach ( var dir in AllDirections ) {
				var locationAt = GetLocationAt(_position, dir);
				if ( locationAt != null ) {
					var title = string.Format(Content.look_at, dir);
					Context.AddCase(title, () => {
						_position = TransformPoint(_position, dir);
					});
				}
			}
			Context.AddBackCaseWith(ResetPosition);
		}

		void DescribeLocation(Location loc) {
			Out.WriteFormat(Content.here_is, loc.Name);
			if ( loc.Owner != null ) {
				var raceName = Content.ResourceManager.GetString("race_" + loc.Owner.Kind.Id);
				Out.WriteFormat(Content.here_live, loc.Owner.Name, raceName);
			} else {
				Out.Write(Content.here_empty);
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

		public List<Location> GetCountryLocations(Country country) {
			return _locations.Values.Where(loc => loc.Owner == country).ToList();
		}

		public List<Location> GetNearLocations(Point point) {
			var result = new List<Location>();
			foreach ( var dir in AllDirections ) {
				var locationAt = GetLocationAt(point, dir);
				if ( locationAt != null ) {
					result.Add(locationAt);
				}
			}
			return result;
		}
	}
}
