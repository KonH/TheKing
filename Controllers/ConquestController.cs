using TheKing.Features.Map;
using TheKing.Features.Army;
using TheKing.Features.Countries;

namespace TheKing.Controllers {
	class ConquestController {
		ArmyController _army;

		public ConquestController(ArmyController army) {
			_army = army;
		}

		public bool TryConquest(Country invader, IReadOnlySquad invaderSquad, Location loc) {
			if ( loc.Owner == null ) {
				OnConquestSuccess(invader, invaderSquad, loc);
				return true;
			} else {
				var defender = loc.Owner;
				var defenderSquad = _army.AquireMaxSquad(defender);
				if ( TryDefeat(invader, ref invaderSquad, defender, ref defenderSquad) ) {
					if ( defenderSquad != null ) {
						_army.ReleaseSquad(defender, defenderSquad);
					}
					OnConquestSuccess(invader, invaderSquad, loc);
					return true;
				}
			}
			return false;
		}

		bool TryDefeat(Country invader, ref IReadOnlySquad invaderSquad, Country defender, ref IReadOnlySquad defenderSquad) {
			var agressorPower = invaderSquad.Count;
			var defenderPower = defenderSquad.Count;
			defenderSquad = _army.KillInSquad(defender, defenderSquad, agressorPower);
			invaderSquad = _army.KillInSquad(invader, invaderSquad, defenderPower);
			return defenderSquad == null;
		}

		void OnConquestSuccess(Country invader, IReadOnlySquad squad, Location loc) {
			loc.Owner = invader;
			if ( squad != null ) {
				_army.ReleaseSquad(invader, squad);
			}
		}
	}
}
