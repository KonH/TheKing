using System;
using TheKing.Features.Map;
using TheKing.Features.Army;
using TheKing.Features.Countries;
using TheKing.Features.Move;
using System.Diagnostics;

namespace TheKing.Controllers {
	class MoveController {
		static Random Rand = new Random(DateTime.Now.Millisecond);

		TimeController _time;
		ArmyController _army;

		public MoveController(TimeController time, ArmyController army) {
			_time = time;
			_army = army;
		}

		public void SendTo(Country country, Location homeLoc, Location targetLoc, IReadOnlySquad squad, Action<MoveResult> callback) {
			var moveTime = homeLoc.Distance + targetLoc.Distance;
			Debug.WriteLine($"Send {country} squad from {homeLoc.Name} to {targetLoc.Name}: days = {moveTime}");
			_time.SheduleAction(moveTime, () => {
				var loses = ProcessSquadLoses(country, targetLoc.Difficulty, ref squad);
				var isDone = squad?.Count > 0;
				Debug.WriteLine($"Send {country} squad from {homeLoc.Name} to {targetLoc.Name}: loses = {loses}, isDone = {isDone}");
				callback(new MoveResult(loses, isDone));
			});
		}

		int ProcessSquadLoses(Country country, double difficulty, ref IReadOnlySquad squad) {
			var loses = 0;
			for ( var i = 0; i < squad.Count; i++ ) {
				if ( Rand.NextDouble() < difficulty ) {
					loses++;
				}
			}
			if ( loses > 0 ) {
				_army.KillInSquad(country, squad, loses);
			}
			return loses;
		}
	}
}
