using System;
using System.Linq;
using System.Collections.Generic;
using TheKing.Features.Context;

namespace TheKing.Controllers {
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
				var selection = ReadString();
				if ( int.TryParse(selection, out var value) ) {
					return value;
				}
			}
		}

		public string ReadString() {
			return Console.ReadLine();
		}
	}
}
