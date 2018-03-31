using System.Collections.Generic;
using TheKing.Controllers;
using TheKing.Features.Map;
using TheKing.Features.Army;
using TheKing.Features.Context;
using TheKing.Features.Countries;

namespace TheKing.Interfaces {
	class ConquestInterface : IUpdateHandler, IContext<ConquestController> {
		ContextController   _context;
		InputController     _input;
		OutputController    _out;
		CountryController   _country;
		MapController       _map;
		DiscoveryController _discovery;
		ArmyController      _army;
		ConquestController  _conquest;

		public ConquestInterface(
			ContextController context, InputController input, OutputController output, CountryController country,
			MapController map, DiscoveryController discovery, ArmyController army, ConquestController conquest
		) {
			_context   = context;
			_input     = input;
			_out       = output;
			_country   = country;
			_map       = map;
			_discovery = discovery;
			_army      = army;
			_conquest  = conquest;
		}


		public void Update() {
			var player = _country.PlayerCountry;
			if ( _army.GetAvailableCount(player) > 0 ) {
				var locs = GetAcceptableLocations(player);
				foreach ( var loc in locs ) {
					var name = loc.Name;
					if ( _discovery.IsDiscovered(player, loc) ) {
						if ( loc.Owner != null ) {
							name += $" ({loc.Owner.Name})";
						}
					} else {
						name += " (?)";
					}
					name += ".";
					_context.AddCase(name, () => TryStartConquest(loc, player));
				}
			}

			_context.AddCase(
				Content.go_back,
				() => _context.GoToRelatedContext<ArmyController>());
		}

		HashSet<Location> GetAcceptableLocations(Country country) {
			var result = new HashSet<Location>();
			var countryLocs = _map.GetCountryLocations(country);
			foreach ( var playerLoc in countryLocs ) {
				var nearLocs = _map.GetNearLocations(playerLoc.Point);
				foreach ( var nearLoc in nearLocs ) {
					if ( nearLoc.Owner != country ) {
						result.Add(nearLoc);
					}
				}
			}
			return result;
		}

		void TryStartConquest(Location location, Country country) {
			var maxCount = _army.GetAvailableCount(country);
			_out.WriteFormat(Content.army_conquest_request_2, maxCount);
			while ( true ) {
				var count = _input.ReadInt();
				if ( (count > 0) && (maxCount >= count) ) {
					_out.Write(Content.army_conquest_response);
					var squad = _army.TryAquireSquad(country, count);
					TryConquest(country, squad, location);
					break;
				}
			}
		}

		void TryConquest(Country invader, IReadOnlySquad invaderSquad, Location loc) {
			if ( _conquest.TryConquest(invader, invaderSquad, loc) ) {
				OnConquestSuccess(loc);
			} else {
				OnConquestFailed(loc);
			}

			_context.AddCase(
				Content.go_back,
				() => _context.GoToRelatedContext<ArmyController>());
		}

		void OnConquestSuccess(Location loc) {
			_out.WriteFormat(Content.conquest_success, loc.Name);
		}

		void OnConquestFailed(Location loc) {
			_out.WriteFormat(Content.conquest_failed, loc.Name);
		}
	}
}
