using System;
using System.Diagnostics;
using System.Collections.Generic;
using TheKing.Features.Countries;

namespace TheKing.Controllers {
	class CountryController {
		public Country PlayerCountry { get; } = new Country("YourKingdom", new Race(RaceId.Human));
		public Country EnemyCountry  { get; } = new Country("Goblington", new Race(RaceId.Goblin));

		public List<Country> Countries => new List<Country>(_countries);

		public event Action<Country, string> OnCountryRemoved = new Action<Country, string>((c, s) => { });

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
