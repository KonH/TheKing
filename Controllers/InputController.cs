using System;
using System.Collections.Generic;
using System.Linq;

namespace TheKing.Controllers {
	class InputController : StateController {

		public InputController(GameState state) : base(state) { }

		public bool Update(IList<Case> cases) {
			if ( cases.Any() ) {
				for ( var i = 0; i < cases.Count; i++ ) {
					State.Out.Write($"{i + 1}) {cases[i].Title}");
				}
				while ( true ) {
					var selection = Console.ReadLine();
					if ( int.TryParse(selection, out var index) ) {
						if ( (index > 0) && (index <= cases.Count) ) {
							cases[index - 1].Callback();
							return true;
						}
					}
				}
			}
			return false;
		}
	}
}
