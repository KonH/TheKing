using System.Diagnostics;

namespace TheKing.New {
	class PopulationUpdater {
		CountryController    _country;
		PopulationController _population;
		MoneyController      _money;
		MapController        _map;

		public PopulationUpdater(TimeController time, CountryController country, PopulationController population, MoneyController money, MapController map) {
			_country    = country;
			_population = population;
			_money      = money;
			_map        = map;
			time.OnDayStart += OnDayStart;
		}

		void OnDayStart() {
			// TODO:
			/*foreach ( var country in _country.Countries ) {
				var population = _population.GetPopulation(country);
				var taxes = population.GetDailyTaxIncome();
				Money.Add(country, $"{Content.taxes_name} ({population.Count})", taxes);
				var locCount = Map.GetCountryLocations(country).Count;
				var growCount = population.TryGrowForDay(locCount);
				if ( growCount > 0 ) {
					Debug.WriteLine($"Grow {country} population: +{growCount} = {population.Count}");
				}
			}*/
		}
	}
}
