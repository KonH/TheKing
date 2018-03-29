﻿using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace TheKing.New {
	class PopulationController {
		class PopulationState {
			public int    Count      { get; set; } = 100;
			public double TaxRate    { get; }      = 0.25;
			public double GrowthRate { get; }      = 0.01;

			double _growthAccum;

			public Gold GetDailyTaxIncome() {
				return new Gold((int)Math.Round(Count * TaxRate));
			}

			public int TryGrowForDay(int locations) {
				_growthAccum += Count * GrowthRate * locations;
				var grow = 0;
				while ( _growthAccum > 1 ) {
					_growthAccum--;
					grow++;
				}
				Count += grow;
				return grow;
			}
		}

		Dictionary<Country, PopulationState> _populationStates = new Dictionary<Country, PopulationState>();

		PopulationState GetPopulation(Country country) {
			return Utils.GetOrCreate(country, _populationStates, () => new PopulationState());
		}

		public int GetCount(Country country) {
			return GetPopulation(country).Count;
		}

		public void Add(Country country, int count) {
			var population = GetPopulation(country);
			population.Count += count;
			Debug.WriteLine($"Add {country} population: +{count} = {population.Count}");
		}

		public void Remove(Country country, int count) {
			var population = GetPopulation(country);
			population.Count -= count;
			Debug.WriteLine($"Remove {country} population: -{count} = {population.Count}");
		}
	}
}
