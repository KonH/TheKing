using System;

namespace TheKing.Controllers {
	class PopulationController : StateController, INextDayHandler {
		public int    Population { get; set; } = 100;
		public double TaxRate    { get; } = 0.25;
		public double GrowthRate { get; } = 0.01;

		double _growthAccum;

		public PopulationController(GameState state) : base(state) { }

		public void OnNextDay() {
			var taxCount = (int)Math.Round(Population * TaxRate);
			State.Money.Add($"{Content.taxes_name} ({Population})", new Gold(taxCount));
			_growthAccum += Population * GrowthRate;
			while ( _growthAccum > 1 ) {
				_growthAccum--;
				Population++;
			}
		}
	}
}
