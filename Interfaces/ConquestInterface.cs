using System;
using System.Collections.Generic;
using TheKing.Controllers;
using TheKing.Features.Map;
using TheKing.Features.Context;
using TheKing.Features.Conquest;
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
				var locPairs = GetAcceptableLocations(player);
				foreach ( var pair in locPairs ) {
					var homeLoc = pair.Item1;
					var targetLoc = pair.Item2;
					var name = targetLoc.Name;
					if ( _discovery.IsDiscovered(player, targetLoc) ) {
						if ( targetLoc.Owner != null ) {
							var raceName = Content.ResourceManager.GetString("race_" + targetLoc.Owner.Kind.Id);
							name += $" ({targetLoc.Owner.Name}, {raceName})";
						}
					} else {
						name += " (?)";
					}
					name += ".";
					_context.AddCase(name, () => TryStartConquest(homeLoc, targetLoc, player));
				}
			}

			_context.AddCase(
				Content.go_back,
				() => _context.GoToRelatedContext<ArmyController>());
		}

		HashSet<Tuple<Location, Location>> GetAcceptableLocations(Country country) {
			var result = new HashSet<Tuple<Location, Location>>();
			var countryLocs = _map.GetCountryLocations(country);
			foreach ( var playerLoc in countryLocs ) {
				var nearLocs = _map.GetNearLocations(playerLoc.Point);
				foreach ( var nearLoc in nearLocs ) {
					if ( nearLoc.Reachable && _conquest.CanConquest(country, nearLoc) ) {
						var alreadyAdded = false;
						foreach ( var item in result ) {
							if ( item.Item2 == nearLoc ) {
								alreadyAdded = true;
								break;
							}
						}
						if ( !alreadyAdded ) {
							result.Add(Tuple.Create(playerLoc, nearLoc));
						}
					}
				}
			}
			return result;
		}

		void TryStartConquest(Location homeLoc, Location targetLoc, Country country) {
			var maxCount = _army.GetAvailableCount(country);
			_out.WriteFormat(Content.army_conquest_request_2, maxCount);
			while ( true ) {
				var count = _input.ReadInt();
				if ( (count > 0) && (maxCount >= count) ) {
					_out.Write(Content.army_conquest_response);
					var squad = _army.TryAquireSquad(country, count);
					_conquest.StartConquest(country, squad, homeLoc, targetLoc, OnConquestComplete);
					break;
				}
			}
		}

		void OnConquestComplete(ConquestResult result, Location loc) {
			if ( result.Success ) {
				_out.WriteFormat(Content.conquest_success, loc.Name, result.Move.Loses, result.Loses);
			} else {
				_out.WriteFormat(Content.conquest_failed, loc.Name, result.Move.Loses, result.Loses);
			}
		}
	}
}
