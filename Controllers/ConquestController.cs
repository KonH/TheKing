using System;
using TheKing.Features.Map;
using TheKing.Features.Army;
using TheKing.Features.Move;
using TheKing.Features.Conquest;
using TheKing.Features.Countries;

namespace TheKing.Controllers {
	class ConquestController {
		ArmyController      _army;
		DiscoveryController _discovery;
		MoveController      _move;

		public ConquestController(ArmyController army, DiscoveryController discovery, MoveController move) {
			_army      = army;
			_discovery = discovery;
			_move      = move;
		}

		public void StartConquest(
			Country invader, IReadOnlySquad invaderSquad, Location homeLoc, Location targetLoc, Action<ConquestResult, Location> onComplete
		) {
			_move.SendTo(invader, homeLoc, targetLoc, invaderSquad, (moveResult) => {
				if ( moveResult.Success ) {
					onComplete(TryConquest(invader, invaderSquad, targetLoc, moveResult), targetLoc);
				} else {
					onComplete(new ConquestResult(0, false, moveResult), targetLoc);
				}
			});
		}

		ConquestResult TryConquest(Country invader, IReadOnlySquad invaderSquad, Location loc, MoveResult moveResult) {
			if ( !moveResult.Success ) {
				return new ConquestResult(0, false, moveResult);
			}
			_discovery.MarkDiscovered(invader, loc);
			if ( loc.Owner == null ) {
				OnConquestSuccess(invader, invaderSquad, loc);
				return new ConquestResult(0, true, moveResult);
			} else {
				var defender = loc.Owner;
				var defenderSquad = _army.AquireMaxSquad(defender);
				var conquestResult = TryDefeat(moveResult, invader, ref invaderSquad, defender, ref defenderSquad);
				if ( conquestResult.Success ) {
					OnConquestSuccess(invader, invaderSquad, loc);
				}
				if ( defenderSquad != null ) {
					_army.ReleaseSquad(defender, defenderSquad);
				}
				return conquestResult;
			}
		}

		ConquestResult TryDefeat(MoveResult moveResult, Country invader, ref IReadOnlySquad invaderSquad, Country defender, ref IReadOnlySquad defenderSquad) {
			var agressorPower = invaderSquad.Count;
			var defenderPower = defenderSquad.Count;
			defenderSquad = _army.KillInSquad(defender, defenderSquad, agressorPower);
			invaderSquad = _army.KillInSquad(invader, invaderSquad, defenderPower);
			var loses = agressorPower - (invaderSquad != null ? invaderSquad.Count : 0);
			return new ConquestResult(loses, defenderSquad == null, moveResult);
		}

		void OnConquestSuccess(Country invader, IReadOnlySquad squad, Location loc) {
			loc.Owner = invader;
			if ( squad != null ) {
				_army.ReleaseSquad(invader, squad);
			}
		}
	}
}
