using System.Linq;
using System.Collections.Generic;
using TheKing.Utils;
using TheKing.Generators;
using TheKing.Features.Map;
using TheKing.Features.Countries;

namespace TheKing.Controllers {
	class MapController {
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

		public Location GetLocationAt(Point pos) {
			return _locations.GetLocationAt(pos);
		}

		public Location GetLocationAt(Point pos, Direction dir) {
			return _locations.GetLocationAt(pos, dir);
		}
		public List<Location> GetCountryLocations(Country country) {
			return _locations.Values.Where(loc => loc.Owner == country).ToList();
		}

		public List<Location> GetNearLocations(Point point) {
			return _locations.GetNearLocations(point);
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
