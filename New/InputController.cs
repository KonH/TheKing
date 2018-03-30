using System;
using System.Collections.Generic;
using System.Linq;

namespace TheKing.New {
	class InputController {
		public Action Update(IList<Case> cases) {
			if ( cases.Any() ) {
				while ( true ) {
					var selection = Console.ReadLine();
					if ( int.TryParse(selection, out var index) ) {
						if ( (index > 0) && (index <= cases.Count) ) {
							return cases[index - 1].Callback;
						}
					}
				}
			}
			return null;
		}

		public int ReadInt() {
			while ( true ) {
				var selection = Console.ReadLine();
				if ( int.TryParse(selection, out var value) ) {
					return value;
				}
			}
		}
	}
}
