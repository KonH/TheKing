using System;

namespace TheKing.Controllers {
	class PopulationController : StateController {
		public int    Count { get; set; } = 100;
		public double TaxRate    { get; } = 0.25;
		public double GrowthRate { get; } = 0.01;

		double _growthAccum;

		public PopulationController(GameState state) : base(state) {
			state.OnNextDay += OnNextDay;
		}

		public void OnNextDay() {
			var taxCount = (int)Math.Round(Count * TaxRate);
			State.Money.Add($"{Content.taxes_name} ({Count})", new Gold(taxCount));
			_growthAccum += Count * GrowthRate;
			while ( _growthAccum > 1 ) {
				_growthAccum--;
				Count++;
			}
		}
	}
}
