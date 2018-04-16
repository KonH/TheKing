using System.Linq;
using System.Diagnostics;
using System.Collections.Generic;
using TheKing.Utils;
using TheKing.Settings;
using TheKing.Features.Countries;

namespace TheKing.Generators {
	class CountryGenerator {
		public List<Country> Countries { get; } = new List<Country>();

		GameSettings _gameSettings;
		RaceSettings _raceSettings;

		public CountryGenerator(GameSettings gameSettings, RaceSettings raceSettings) {
			_gameSettings = gameSettings;
			_raceSettings = raceSettings;
		}

		public void Generate() {
			Countries.Clear();
			if ( _gameSettings.WithPlayer ) {
				var playerCountry = Generate(true);
				Countries.Add(playerCountry);
				Debug.WriteLine($"CountryGenerator: New player: {playerCountry}");
			}
			for ( int i = 0; i < _gameSettings.Enemies; i++ ) {
				var newCountry = Generate(false);
				Countries.Add(newCountry);
				Debug.WriteLine($"CountryGenerator: New enemy: {newCountry}");
			}
		}

		Race GetRace(bool player) {
			if ( player && (_raceSettings.PlayerRace != null) ) {
				return _raceSettings.PlayerRace;
			}
			var allRaces = _raceSettings.AllRaces;
			var raceChances = new List<double>();
			foreach ( var race in allRaces ) {
				var count = Countries.Count(c => c.Kind.Id == race);
				raceChances.Add(1 / (2 * count + 1));
			}
			var id = RandUtils.GetItemWithChances(allRaces, raceChances);
			return _raceSettings.Get(id);
		}

		string GenerateUniqueName(RaceId race) {
			do {
				var name = GetNamePart(race, 0) + GetNamePart(race, 1) + GetNamePart(race, 2);
				if ( Countries.Find(c => c.Name == name) == null ) {
					return name;
				}
			} while ( true );
		}

		string GetNamePart(RaceId race, int index) {
			var path = $"country_{race}_{index}";
			var nameParts = Content.ResourceManager.GetString(path).Split(";");
			return RandUtils.GetItem(nameParts);
		}

		Country Generate(bool player) {
			var race = GetRace(player);
			return new Country(GenerateUniqueName(race.Id), race, player);
		}
	}
}
