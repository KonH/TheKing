using System;
using TheKing.Features.Map;
using TheKing.Features.Army;
using TheKing.Features.Countries;
using TheKing.Features.Move;
using System.Diagnostics;
using System.Collections.Generic;
using TheKing.Utils;

namespace TheKing.Controllers {
	class MoveController {
		TimeController _time;
		ArmyController _army;

		Dictionary<Country, HashSet<Location>> _curRoutes = new Dictionary<Country, HashSet<Location>>();

		public MoveController(TimeController time, ArmyController army) {
			_time = time;
			_army = army;
		}

		HashSet<Location> GetRoutes(Country country) {
			return DictUtils.GetOrCreate(country, _curRoutes, () => new HashSet<Location>());
		}

		public bool HasRoute(Country country, Location location) {
			return GetRoutes(country).Contains(location);
		}

		public void SendTo(Country country, Location homeLoc, Location targetLoc, IReadOnlySquad squad, Action<MoveResult> callback) {
			var moveTime = homeLoc.Distance + targetLoc.Distance;
			Debug.WriteLine($"Send {country} squad from {homeLoc.Name} to {targetLoc.Name}: days = {moveTime}");
			var routes = GetRoutes(country);
			routes.Add(targetLoc);
			_time.SheduleAction(moveTime, () => {
				var loses = ProcessSquadLoses(country, targetLoc.Difficulty, ref squad);
				var isDone = squad?.Count > 0;
				Debug.WriteLine($"Send {country} squad from {homeLoc.Name} to {targetLoc.Name}: loses = {loses}, isDone = {isDone}");
				callback(new MoveResult(loses, isDone));
				routes.Remove(targetLoc);
			});
		}

		int ProcessSquadLoses(Country country, double difficulty, ref IReadOnlySquad squad) {
			var loses = 0;
			for ( var i = 0; i < squad.Count; i++ ) {
				if ( RandUtils.Rand.NextDouble() < difficulty ) {
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
