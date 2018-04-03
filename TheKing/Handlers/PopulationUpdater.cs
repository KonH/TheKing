using System.Diagnostics;
using TheKing.Controllers;
using TheKing.Features.Time;

namespace TheKing.Handlers {
	class PopulationUpdater : IDayStarter {
		CountryController    _country;
		PopulationController _population;
		MoneyController      _money;
		MapController        _map;

		public PopulationUpdater(CountryController country, PopulationController population, MoneyController money, MapController map) {
			_country    = country;
			_population = population;
			_money      = money;
			_map        = map;
		}

		public void OnDayStart() {
			foreach ( var country in _country.Countries ) {
				var taxes = _population.GetDailyTaxIncome(country);
				_money.Add(country, $"{Content.taxes_name} ({_population.GetCount(country)})", taxes);

				var locCount = _map.GetCountryLocations(country).Count;
				var growCount = _population.TryGrowForDay(country, locCount);
				if ( growCount > 0 ) {
					Debug.WriteLine($"Grow {country} population: +{growCount} = {_population.GetCount(country)}");
				}
			}
		}
	}
}
