using System.Diagnostics;
using TheKing.Controllers.Kingdom;

namespace TheKing.Controllers {
	class PopulationController : StateController {
		public PopulationController(GameState state) : base(state) {
			State.Time.OnDayStart += OnDayStart;
		}

		public void Add(Country country, int count) {
			country.Population.Count += count;
			Debug.WriteLine($"Add {country} population: +{count} = {country.Population.Count}");
		}

		public void Remove(Country country, int count) {
			country.Population.Count -= count;
			Debug.WriteLine($"Remove {country} population: -{count} = {country.Population.Count}");
		}

		void OnDayStart() {
			foreach ( var country in State.Country.Countries ) {
				var population = country.Population;
				var taxes = population.GetDailyTaxIncome();
				Money.Add(country, $"{Content.taxes_name} ({population.Count})", taxes);
				var growCount = population.TryGrowForDay();
				if ( growCount > 0 ) {
					Debug.WriteLine($"Grow {country} population: +{growCount} = {country.Population.Count}");
				}
			}
		}
	}
}
