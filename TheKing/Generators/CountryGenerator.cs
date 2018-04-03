using System;
using System.Collections.Generic;
using System.Diagnostics;
using TheKing.Features.Countries;
using TheKing.Utils;

namespace TheKing.Generators {
	class CountryGenerator {
		public List<Country> Countries { get; } = new List<Country>();

		public void Generate(int enemies) {
			Countries.Clear();
			var playerRace = RaceId.Human;
			var playerCountry = new Country(GenerateUniqueName(playerRace), new Race(playerRace), true);
			Countries.Add(playerCountry);
			Debug.WriteLine($"CountryGenerator: New player: {playerCountry}");
			for ( int i = 0; i < enemies; i++ ) {
				var newCountry = Generate();
				Countries.Add(newCountry);
				Debug.WriteLine($"CountryGenerator: New enemy: {newCountry}");
			}
		}

		RaceId GetRace() {
			var allRaces = (RaceId[])Enum.GetValues(typeof(RaceId));
			return RandUtils.GetItem(allRaces);
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

		Country Generate() {
			var race = GetRace();
			return new Country(GenerateUniqueName(race), new Race(race), false);
		}
	}
}
