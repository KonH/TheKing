using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;

namespace TheKing.New {
	class Case {
		public string Title    { get; }
		public Action Callback { get; }

		public Case(string title, Action callback) {
			Title    = title;
			Callback = callback;
		}
	}
	
	interface IUpdateHandler {
		void Update();
	}

	interface IStartHandler {
		void OnStart();
	}

	interface IContext<T> : IUpdateHandler {
	}

	class ContextController {
		public event Action OnStart = new Action(() => { });

		public List<Case> Cases => new List<Case>(_cases);

		IServiceProvider _services;
		OutputController _out;

		IUpdateHandler _curContext;
		bool           _started;

		List<Case> _cases = new List<Case>();

		public ContextController(IServiceProvider services, OutputController output) {
			_services = services;
			_out      = output;
		}

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
					_out.Write(Content.root_message);
				} else {
					_out.Write(Content.hello_message);
					_started = true;
				}
				OnStart.Invoke();
			} else {
				_curContext.Update();
			}
		}

		public void WriteCases() {
			for ( var i = 0; i < _cases.Count; i++ ) {
				_out.Write($"{i + 1}) {_cases[i].Title}");
			}
		}

		public void GoTo(IUpdateHandler controller) {
			_curContext = controller;
		}

		public void GoToRelatedContext<T>() {
			var context = _services.GetService<IContext<T>>();
			GoTo(context);
		}
	}
}
