using System;

namespace TheKing.Controllers {
	class PopulationController : StateController, INextDayHandler {
		public int    Population { get; } = 100;
		public double TaxRate    { get; } = 0.25;

		public PopulationController(GameState state) : base(state) { }

		public void OnNextDay() {
			var taxCount = (int)Math.Round(Population * TaxRate);
			State.Money.Add(Content.taxes_name, new Gold(taxCount));
		}
	}
}
