using System;
using System.Collections.Generic;
using TheKing.Features.Map;

namespace TheKing.Settings {
	class MapSettings {
		public int Width  { get; }
		public int Height { get; }

		public Dictionary<LocationType, Func<double, double>> ChanceByYAxis { get; } = 
			new Dictionary<LocationType, Func<double, double>>();

		public MapSettings(int width, int height) {
			Width  = width;
			Height = height;
		}

		/// <summary>
		/// Input: 
		/// Y (0.0, 1.0) = (0, max) - normalized y axis value
		/// Output:
		/// Chance (0.0, 1.0) = change of type on given y pos
		/// </summary>
		/// <param name="type"></param>
		/// <param name="func"></param>
		/// <returns></returns>
		public MapSettings WithChance(LocationType type, Func<double, double> func) {
			ChanceByYAxis.Add(type, func);
			return this;
		}
	}
}
