using System;
using System.Collections.Generic;

namespace TheKing.Utils {
	static class RandUtils {
		static Random Rand = new Random(DateTime.Now.Millisecond);

		public static T GetItem<T>(IList<T> container) {
			var randIndex = Rand.Next(container.Count);
			return container[randIndex];
		}
	}
}
