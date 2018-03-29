using System;
using System.Collections.Generic;
using TheKing.Controllers.Kingdom;
using TheKing.Controllers.Map;

namespace TheKing.Controllers {
	class ConquestController : StateController, IUpdateHandler {
		public ConquestController(GameState state):base(state) { }

		public void Update() {
			if ( Army.GetAvailableCount(Player) > 0 ) {
				var locs = GetAcceptableLocations();
				foreach ( var loc in locs ) {
					var name = loc.Name;
					if ( loc.Owner != null ) {
						name += $" ({loc.Owner.Name})";
					}
					name += ".";
					Context.AddCase(name, () => TryStartConquest(loc));
				}
			}
			Context.AddCase(
				Content.go_back,
				() => State.Context.GoTo(State.Army, false));
		}

		HashSet<Location> GetAcceptableLocations() {
			var result = new HashSet<Location>();
			var playerLocs = State.Map.GetCountryLocations(State.Country.PlayerCountry);
			foreach ( var playerLoc in playerLocs ) {
				var nearLocs = State.Map.GetNearLocations(playerLoc.Point);
				foreach ( var nearLoc in nearLocs ) {
					if ( nearLoc.Owner != State.Country.PlayerCountry ) {
						result.Add(nearLoc);
					}
				}
			}
			return result;
		}

		void TryStartConquest(Location location) {
			var maxCount = Army.GetAvailableCount(Player);
			Out.WriteFormat(Content.army_conquest_request_2, maxCount);
			while ( true ) {
				var count = Input.ReadInt();
				if ( (count > 0) && (maxCount >= count) ) {
					Out.Write(Content.army_conquest_response);
					var squad = Army.TryAquireSquad(Player, count);
					TryConquest(Player, squad, location);
					break;
				}
			}
		}

		void TryConquest(Country invader, IReadOnlySquad invaderSquad, Location loc) {
			if ( loc.Owner == null ) {
				OnConquestSuccess(invader, invaderSquad, loc);
			} else {
				var defender = loc.Owner;
				var defenderSquad = Army.AquireMaxSquad(defender);
				if ( TryDefeat(invader, ref invaderSquad, defender, ref defenderSquad) ) {
					if ( defenderSquad != null ) {
						Army.ReleaseSquad(defender, defenderSquad);
					}
					OnConquestSuccess(invader, invaderSquad, loc);
				} else {
					OnConquestFailed(loc);
				}
			}
			State.Context.GoTo(State.Army, false);
		}

		bool TryDefeat(Country invader, ref IReadOnlySquad invaderSquad, Country defender, ref IReadOnlySquad defenderSquad) {
			var agressorPower = invaderSquad.Count;
			var defenderPower = defenderSquad.Count;
			defenderSquad = Army.KillInSquad(defender, defenderSquad, agressorPower);
			invaderSquad = Army.KillInSquad(invader, invaderSquad, defenderPower);
			return defenderSquad == null;
		}

		void OnConquestSuccess(Country invader, IReadOnlySquad squad, Location loc) {
			loc.Owner = invader;
			Population.Add(invader, 100);
			if ( squad != null ) {
				Army.ReleaseSquad(invader, squad);
			}
			State.Out.WriteFormat(Content.conquest_success, loc.Name);
		}

		void OnConquestFailed(Location loc) {
			State.Out.WriteFormat(Content.conquest_failed, loc.Name);
		}
	}
}
