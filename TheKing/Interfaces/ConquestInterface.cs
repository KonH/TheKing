using TheKing.Controllers;
using TheKing.Features.Map;
using TheKing.Features.Context;
using TheKing.Features.Conquest;
using TheKing.Features.Countries;
using TheKing.Utils;

namespace TheKing.Interfaces {
	class ConquestInterface : IUpdateHandler, IContext<ConquestController>, IConquestHandler {
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
				var locPairs = _conquest.GetAcceptableLocations(player);
				foreach ( var pair in locPairs ) {
					var homeLoc = pair.Item1;
					var targetLoc = pair.Item2;
					var name = targetLoc.Name;
					if ( _discovery.IsDiscovered(player, targetLoc) ) {
						if ( targetLoc.Owner != null ) {
							var raceName = LocUtils.TranslateRaceName(targetLoc.Owner);
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
			if ( result.Defender != null ) {
				var raceName = Content.ResourceManager.GetString("race_" + result.Defender.Kind.Id);
				_out.WriteFormat(Content.here_lived, result.Defender.Name, raceName, loc.Name);
			}
			var batlleLoses = result.InvaderSquad != null ? result.InvaderSquad.Loses : 0;
			if ( result.Success ) {
				_out.WriteFormat(Content.conquest_success, loc.Name, result.Move.Loses, batlleLoses);
			} else {
				_out.WriteFormat(Content.conquest_failed, loc.Name, result.Move.Loses, batlleLoses);
			}
		}

		public void OnConquest(ConquestResult result) {
			if ( (result.Defender != null) && (result.Defender == _country.PlayerCountry) ) {
				_out.WriteFormat(
					Content.enemy_conquest_result,
					result.Location.Name,
					result.Invader.Name,
					LocUtils.TranslateRaceName(result.Invader),
					result.InvaderSquad.Count,
					result.DefenderSquad.Count,
					result.InvaderSquad.Loses,
					result.DefenderSquad.Loses
				);
				_out.WriteFormat(result.Success ? Content.enemy_conquest_success : Content.enemy_conquest_fail, result.Location.Name);
			}
		}
	}
}
