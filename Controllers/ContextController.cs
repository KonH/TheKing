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

		IUpdateHandler  _curContext;
		IWelcomeHandler _curWelcome;
		bool            _started;

		List<Case> _cases = new List<Case>();

		public ContextController(GameState state) : base(state) { }

		public void AddCase(string title, Action callback) {
			_cases.Add(new Case(title, callback));
		}

		public void AddBackCase() {
			AddBackCaseWith(null);
		}

		public void AddBackCaseWith(Action callback) {
			AddCase(Content.go_back_to_start, () => {
				callback?.Invoke();
				ResetContext();
			});
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
				AddCase(Content.go_to_map, () => GoTo(State.Map));
				AddCase(Content.go_to_bank, () => GoTo(State.Money));
				AddCase(Content.go_to_army, () => GoTo(State.Army));
				AddCase(Content.next_day, () => State.Time.NextDay());
			} else {
				if ( _curWelcome != null ) {
					_curWelcome.Welcome();
					_curWelcome = null;
				}
				_curContext.Update();
			}
		}

		public void GoTo(IUpdateHandler controller, bool firstTime = true) {
			_curContext = controller;
			if ( firstTime ) {
				_curWelcome = _curContext as IWelcomeHandler;
			}
		}
	}
}
