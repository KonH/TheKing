using System;
using System.Collections.Generic;
using System.Diagnostics;
using TheKing.Controllers.Kingdom;

namespace TheKing.Controllers {
	class CountryController {
		public Country PlayerCountry { get; } = new Country("YourKingdom", new Race(RaceId.Human));
		// Temporary
		public Country EnemyCountry { get; } = new Country("Goblington", new Race(RaceId.Goblin));

		public List<Country> Countries => new List<Country>(_countries);

		public Action<Country, string> OnCountryRemoved = new Action<Country, string>((c, s) => { });

		List<Country> _countries = new List<Country>();

		public CountryController() {
			_countries.Add(PlayerCountry);
			_countries.Add(EnemyCountry);
		}

		public void Remove(Country country, string reason) {
			Debug.WriteLine($"CountryController.Remove: {country}");
			_countries.Remove(country);
			OnCountryRemoved.Invoke(country, reason);
		}
	}
}
