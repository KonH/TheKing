using System.Linq;
using System.Diagnostics;
using System.Collections.Generic;
using TheKing.Features.Countries;

namespace TheKing.Settings {
	class RaceSettings {
		public Race PlayerRace { get; private set; }

		Dictionary<RaceId, Race> _races = new Dictionary<RaceId, Race>();

		public RaceSettings With(Race race) {
			_races.Add(race.Id, race);
			return this;
		}

		public void SelectPlayerRace(RaceId raceId) {
			PlayerRace = Get(raceId);
			Debug.WriteLine($"RaceSettings.SelectPlayerRace: {raceId} => {PlayerRace}");
		}

		public List<RaceId> AllRaces => _races.Keys.ToList();

		public Race Get(RaceId id) => _races[id];
	}
}
