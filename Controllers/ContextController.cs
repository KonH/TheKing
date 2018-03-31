using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using TheKing.Features.Context;

namespace TheKing.Controllers {
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
