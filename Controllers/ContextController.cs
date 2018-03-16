using System;
using System.Collections.Generic;

namespace TheKing.Controllers {
	class Case {
		public string Title    { get; }
		public Action Callback { get; }

		public Case(string title, Action callback) {
			Title    = title;
			Callback = callback;
		}
	}

	class ContextController : StateController {
		public List<Case> Cases => new List<Case>(_cases);

		IUpdateController _curContext;
		bool              _started;

		List<Case> _cases = new List<Case>();

		public ContextController(GameState state) : base(state) { }

		public void AddCase(string title, Action callback) {
			_cases.Add(new Case(title, callback));
		}

		public void ClearCases() {
			_cases.Clear();
		}

		public void ResetContext() {
			_curContext = null;
		}

		public void Update() {
			if ( _curContext == null ) {
				if ( _started ) {
					State.Out.Write(Content.root_message);
				} else {
					State.Out.Write(Content.hello_message);
					_started = true;
				}
				AddCase(Content.go_to_map, () => _curContext = State.Map);
			} else {
				_curContext.Update();
			}
		}
	}
}
