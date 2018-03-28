using TheKing.Controllers.Kingdom;

namespace TheKing.Controllers {
	class PopulationController : StateController {
		public PopulationController(GameState state) : base(state) {
			State.Time.OnDayStart += OnDayStart;
		}

		public void Remove(Country country, int count) {
			country.Population.Count -= count;
		}

		void OnDayStart() {
			foreach ( var country in State.Country.Countries ) {
				var population = country.Population;
				var taxes = population.GetDailyTaxIncome();
				Money.Add(country, $"{Content.taxes_name} ({population.Count})", taxes);
				population.TryGrowForDay();
			}
		}
	}
}
