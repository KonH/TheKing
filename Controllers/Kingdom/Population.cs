using System;
using TheKing.Controllers.Money;

namespace TheKing.Controllers.Kingdom {
	class Population {
		public int    Count      { get; set; } = 100;
		public double TaxRate    { get; }      = 0.25;
		public double GrowthRate { get; }      = 0.01;

		double _growthAccum;

		public Gold GetDailyTaxIncome() {
			return new Gold((int)Math.Round(Count * TaxRate));
		}

		public int TryGrowForDay() {
			_growthAccum += Count * GrowthRate;
			var grow = 0;
			while ( _growthAccum > 1 ) {
				_growthAccum--;
				grow++;
			}
			Count += grow;
			return grow;
		}
	}
}
