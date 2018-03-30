using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace TheKing.New {
	interface ICountryHandler {
		void OnCountryRemoved(Country country, string reason);
	}

	enum RaceId {
		Human,
		Goblin
	}

	class Race {
		public RaceId Id { get; }

		public Race(RaceId name) {
			Id = name;
		}
	}

	class Country {
		public string Name { get; }
		public Race   Kind { get; }

		public Country(string name, Race kind) {
			Name = name;
			Kind = kind;
		}

		public override string ToString() {
			return $"('{Name}', {Kind.Id})";
		}

		public override int GetHashCode() {
			return Name.GetHashCode();
		}
	}

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
