using System.Collections.Generic;
using TheKing.Controllers.Map;

namespace TheKing.Controllers {
	class ConquestController : StateController, IUpdateHandler {
		public ConquestController(GameState state):base(state) { }

		public void Update() {
			var locs = GetAcceptableLocations();
			foreach ( var loc in locs ) {
				var name = loc.Name;
				if ( loc.Owner != null ) {
					name += $" ({loc.Owner.Name})";
				}
				Context.AddCase(name, () => TryConquest(loc));
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

		void TryConquest(Location loc) {
			loc.Owner = State.Country.PlayerCountry;
		}
	}
}
