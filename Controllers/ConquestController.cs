using System.Collections.Generic;
using TheKing.Controllers.Kingdom;
using TheKing.Controllers.Map;

namespace TheKing.Controllers {
	class ConquestController : StateController, IUpdateHandler {
		public ConquestController(GameState state):base(state) { }

		public void Update() {
			if ( Army.GetCount(Player) > 0 ) {
				var locs = GetAcceptableLocations();
				foreach ( var loc in locs ) {
					var name = loc.Name;
					if ( loc.Owner != null ) {
						name += $" ({loc.Owner.Name})";
					}
					Context.AddCase(name, () => TryConquest(Player, loc));
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

		void TryConquest(Country invader, Location loc) {
			if ( loc.Owner == null ) {
				OnConquestSuccess(invader, loc);
			} else {
				var defender = loc.Owner;
				var agressorPower = Army.GetCount(invader);
				var defenderPower = Army.GetCount(defender);
				State.Army.Kill(invader, defenderPower);
				State.Army.Kill(defender, agressorPower);
				if ( agressorPower > defenderPower ) {
					OnConquestSuccess(invader, loc);
				} else {
					OnConquestFailed(loc);
				}
			}
			State.Context.GoTo(State.Army, false);
		}

		void OnConquestSuccess(Country invader, Location loc) {
			loc.Owner = invader;
			Population.Add(invader, 100);
			State.Out.WriteFormat(Content.conquest_success, loc.Name);
		}

		void OnConquestFailed(Location loc) {
			State.Out.WriteFormat(Content.conquest_failed, loc.Name);
		}
	}
}
