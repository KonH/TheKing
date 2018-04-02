using System;
using System.Collections.Generic;

namespace TheKing.Utils {
	static class RandUtils {
		public static Random Rand { get; } = new Random(DateTime.Now.Millisecond);

		public static T GetItem<T>(IList<T> container) {
			var randIndex = Rand.Next(container.Count);
			return container[randIndex];
		}
	}
}
