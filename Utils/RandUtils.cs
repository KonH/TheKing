using System;
using System.Collections.Generic;
using System.Linq;

namespace TheKing.Utils {
	static class RandUtils {
		public static Random Rand { get; } = new Random(DateTime.Now.Millisecond);

		public static T GetItem<T>(IList<T> container) {
			var randIndex = Rand.Next(container.Count);
			return container[randIndex];
		}

		public static T GetItemWithChances<T>(IList<T> items, IList<double> chances) {
			var max = chances.Sum();
			var randValue = Rand.NextDouble() * max;
			var accum = 0.0;
			for ( var i = 0; i < items.Count; i++ ) {
				accum += chances[i];
				if ( randValue < accum ) {
					return items[i];
				}
			}
			return items.Last();
		}
	}
}
