using System;
using System.Diagnostics;
using System.Collections.Generic;
using TheKing.Generators;
using TheKing.Features.Countries;

namespace TheKing.Controllers {
	class CountryController {
		public Country PlayerCountry { get; }

		public List<Country> Countries => new List<Country>(_countries);

		public event Action<Country, string> OnCountryRemoved = new Action<Country, string>((c, s) => { });

		List<Country> _countries = new List<Country>();

		public CountryController(CountryGenerator generator) {
			_countries.AddRange(generator.Countries);
			PlayerCountry = _countries.Find(c => c.Player);
		}

		public void Remove(Country country, string reason) {
			Debug.WriteLine($"CountryController.Remove: {country}");
			OnCountryRemoved.Invoke(country, reason);
			_countries.Remove(country);
		}
	}
}
