using System;
using System.Diagnostics;
using System.Collections.Generic;
using TheKing.Features.Map;
using TheKing.Features.Army;
using TheKing.Features.Move;
using TheKing.Features.Conquest;
using TheKing.Features.Countries;

namespace TheKing.Controllers {
	class ConquestController {
		MapController       _map;
		ArmyController      _army;
		DiscoveryController _discovery;
		MoveController      _move;
		CheatController     _cheats;

		public Action<ConquestResult> OnConquest = new Action<ConquestResult>(_ => { });

		public ConquestController(
			MapController map, ArmyController army, DiscoveryController discovery, MoveController move, CheatController cheats = null
		) {
			_map       = map;
			_army      = army;
			_discovery = discovery;
			_move      = move;
			_cheats    = cheats;
		}

		public HashSet<Tuple<Location, Location>> GetAcceptableLocations(Country country) {
			var result = new HashSet<Tuple<Location, Location>>();
			var countryLocs = _map.GetCountryLocations(country);
			foreach ( var playerLoc in countryLocs ) {
				var nearLocs = _map.GetNearLocations(playerLoc.Point);
				foreach ( var nearLoc in nearLocs ) {
					if ( nearLoc.Reachable && CanConquest(country, nearLoc) ) {
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

		public bool CanConquest(Country country, Location location) {
			if ( (_cheats != null) && _cheats.NonConquestable ) {
				if ( (location.Owner != null) && location.Owner.Player ) {
					return false;
				}
			}
			if ( location.Owner != country ) {
				return !_move.HasRoute(country, location);
			}
			return false;
		}

		public void StartConquest(
			Country invader, IReadOnlySquad invaderSquad, Location homeLoc, Location targetLoc, Action<ConquestResult, Location> onComplete
		) {
			_move.SendTo(invader, homeLoc, targetLoc, invaderSquad, (moveResult) => {
				if ( moveResult.Success ) {
					onComplete(TryConquest(invader, invaderSquad, targetLoc, moveResult), targetLoc);
				} else {
					onComplete(RaiseConquestResult(new ConquestResult(targetLoc, invader, null, null, null, false, moveResult)), targetLoc);
				}
			});
		}

		ConquestResult TryConquest(Country invader, IReadOnlySquad invaderSquad, Location loc, MoveResult moveResult) {
			if ( !moveResult.Success ) {
				return RaiseConquestResult(new ConquestResult(loc, invader, null, null, null, false, moveResult));
			}
			_discovery.MarkDiscovered(invader, loc);
			if ( loc.Owner == null ) {
				OnConquestSuccess(invader, invaderSquad, loc);
				Debug.WriteLine($"ConquestController.TryConquest: {invader} auto-conquest '{loc.Name}'");
				return RaiseConquestResult(new ConquestResult(loc, invader, null, new SquadInfo(invaderSquad.Count, 0), null, true, moveResult));
			} else {
				var defender = loc.Owner;
				var defenderSquad = _army.AquireMaxSquad(defender);
				var conquestResult = TryDefeat(loc, moveResult, invader, ref invaderSquad, defender, ref defenderSquad);
				if ( conquestResult.Success ) {
					OnConquestSuccess(invader, invaderSquad, loc);
				}
				if ( defenderSquad != null ) {
					_army.ReleaseSquad(defender, defenderSquad);
				}
				Debug.WriteLine(
					$"ConquestController.TryConquest: {invader} conquest '{loc.Name}', result: ({conquestResult.Success}, " +
					$"invader: {conquestResult.InvaderSquad.Count} - {conquestResult.InvaderSquad.Loses}," +
					$"defender: {conquestResult.DefenderSquad.Count} - {conquestResult.DefenderSquad.Loses})"
				);
				return RaiseConquestResult(conquestResult);
			}
		}

		ConquestResult TryDefeat(
			Location location, MoveResult moveResult, Country invader, ref IReadOnlySquad invaderSquad, Country defender, ref IReadOnlySquad defenderSquad
		) {
			var invaderCount = invaderSquad.Count;
			var defenderCount = defenderSquad.Count;
			defenderSquad = _army.KillInSquad(defender, defenderSquad, invaderCount);
			invaderSquad = _army.KillInSquad(invader, invaderSquad, defenderCount);
			var invaderLoses = invaderCount - (invaderSquad != null ? invaderSquad.Count : 0);
			var defenderLoses = defenderCount - (defenderSquad != null ? defenderSquad.Count : 0);
			return new ConquestResult(
				location,
				invader, defender, 
				new SquadInfo(invaderCount, invaderLoses),
				new SquadInfo(defenderCount, defenderLoses), 
				defenderSquad == null, moveResult);
		}

		void OnConquestSuccess(Country invader, IReadOnlySquad squad, Location loc) {
			_map.ChangeOwner(loc, invader);
			if ( squad != null ) {
				_army.ReleaseSquad(invader, squad);
			}
		}

		ConquestResult RaiseConquestResult(ConquestResult result) {
			OnConquest.Invoke(result);
			return result;
		}
	}
}
