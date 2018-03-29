using System;
using System.Collections.Generic;

namespace TheKing {
	public static class Utils {
		public static TValue GetOrCreate<TKey, TValue>(TKey key, Dictionary<TKey, TValue> dict, Func<TValue> init) {
			if ( dict.TryGetValue(key, out var value) ) {
				return value;
			}
			var newValue = init();
			dict.Add(key, newValue);
			return newValue;
		}
	}
}
