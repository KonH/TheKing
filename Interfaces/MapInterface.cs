using System.Linq;
using TheKing.Controllers;
using TheKing.Features.Map;
using TheKing.Features.Context;
using TheKing.Features.Countries;
using System.Diagnostics;

namespace TheKing.Interfaces {
	class MapInterface : IUpdateHandler, IStartHandler {
		CountryController   _country;
		MapController       _map;
		DiscoveryController _discovery;
		OutputController    _out;
		ContextController   _context;

		Point _position;

		public MapInterface(
			CountryController country, MapController map, DiscoveryController discovery, OutputController output, ContextController context
		) {
			_country   = country;
			_map       = map;
			_discovery = discovery;
			_out       = output;
			_context   = context;
			ResetPosition();
		}

		public void OnStart() {
			_context.AddCase(Content.go_to_map, () => _context.GoTo(this));
		}

		public void Update() {
			Debug.WriteLine($"Alive countries: {_country.Countries.Count}");

			var curLocation = _map.GetLocationAt(_position);
			DescribeLocation(_country.PlayerCountry, curLocation);

			foreach ( var dir in _map.GetDirections() ) {
				var locationAt = _map.GetLocationAt(_position, dir);
				if ( locationAt != null ) {
					var title = string.Format(Content.look_at, dir);
					_context.AddCase(title, () => {
						_position = _map.TransformPoint(_position, dir);
					});
				}
			}

			_context.AddBackCaseWith(() => ResetPosition());
		}

		void ResetPosition() {
			var firstLocation = _map.GetCountryLocations(_country.PlayerCountry).First();
			_position = firstLocation.Point;
		}

		void DescribeLocation(Country country, Location loc) {
			_out.WriteFormat(Content.here_is, loc.Name);
			if ( !_discovery.IsDiscovered(country, loc) ) {
				_out.Write(Content.here_unknown);
			} else if ( loc.Owner != null ) {
				var raceName = Content.ResourceManager.GetString("race_" + loc.Owner.Kind.Id);
				_out.WriteFormat(Content.here_live, loc.Owner.Name, raceName);
			} else {
				_out.Write(Content.here_empty);
			}
		}
	}
}
