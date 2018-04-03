using System.Collections.Generic;
using System.Diagnostics;
using TheKing.Utils;
using TheKing.Settings;
using TheKing.Features.Countries;
using System.Linq;

namespace TheKing.Generators {
	class CountryGenerator {
		public List<Country> Countries { get; } = new List<Country>();

		RaceSettings _settings;

		public CountryGenerator(RaceSettings settings) {
			_settings = settings;
		}

		public void Generate(int enemies) {
			Countries.Clear();
			var playerCountry = Generate(true);
			Countries.Add(playerCountry);
			Debug.WriteLine($"CountryGenerator: New player: {playerCountry}");
			for ( int i = 0; i < enemies; i++ ) {
				var newCountry = Generate(false);
				Countries.Add(newCountry);
				Debug.WriteLine($"CountryGenerator: New enemy: {newCountry}");
			}
		}

		Race GetRace(bool player) {
			if ( player && (_settings.PlayerRace != null) ) {
				return _settings.PlayerRace;
			}
			var allRaces = _settings.AllRaces;
			var raceChances = new List<double>();
			foreach ( var race in allRaces ) {
				var count = Countries.Count(c => c.Kind.Id == race);
				raceChances.Add(1 / (2 * count + 1));
			}
			var id = RandUtils.GetItemWithChances(allRaces, raceChances);
			return _settings.Get(id);
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
