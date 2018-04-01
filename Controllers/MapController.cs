using System.Linq;
using System.Collections.Generic;
using TheKing.Generators;
using TheKing.Features.Map;
using TheKing.Features.Countries;

namespace TheKing.Controllers {
	class MapController {
		Direction[] AllDirections = {
			Direction.North,
			Direction.East,
			Direction.South,
			Direction.West,
		};

		CountryController _country;

		Dictionary<Point, Location> _locations;

		public MapController(CountryController country, MapGenerator mapGen) {
			_country = country;

			_locations = new Dictionary<Point, Location>(mapGen.Locations);

			country.OnCountryRemoved += (c, r) => RemoveCountry(c);
		}

		void AddLocation(Location loc) {
			_locations.Add(loc.Point, loc);
		}

		void RemoveCountry(Country c) {
			foreach ( var loc in _locations ) {
				if ( loc.Value.Owner == c ) {
					loc.Value.Owner = null;
				}
			}
		}

		public Direction[] GetDirections() {
			return AllDirections;
		}

		public Location GetLocationAt(Point pos) {
			if ( _locations.TryGetValue(pos, out var loc) ) {
				return loc;
			}
			return null;
		}

		public Location GetLocationAt(Point pos, Direction dir) {
			return GetLocationAt(TransformPoint(pos, dir));
		}

		public Point TransformPoint(Point p, Direction dir) {
			switch ( dir ) {
				case Direction.North: return new Point(p.X    , p.Y - 1);
				case Direction.East : return new Point(p.X + 1, p.Y    );
				case Direction.South: return new Point(p.X    , p.Y + 1);
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

		public void ChangeOwner(Location loc, Country country) {
			var prevOwner = loc.Owner;
			loc.Owner = country;
			if ( prevOwner != null ) {
				CheckLocationFail(prevOwner);
			}
		}

		void CheckLocationFail(Country country) {
			if ( GetCountryLocations(country).Count == 0 ) {
				_country.Remove(country, Content.game_over_locations);
			}
		}
	}
}
