using System;
using System.Collections.Generic;
using System.Linq;
using TheKing.Controllers.Money;

namespace TheKing.Controllers {
	class MoneyController : StateController, IUpdateHandler, IWelcomeHandler {

		class HistoryItem {
			public string Title { get; }
			public Gold   Gold  { get; }

			public HistoryItem(string title, Gold value) {
				Title = title;
				Gold  = value;
			}
		}

		public Gold Balance { get; private set; } = new Gold();

		List<HistoryItem> _history = new List<HistoryItem>();

		public MoneyController(GameState state) : base(state) { }

		public void Add(string title, Gold value) {
			_history.Add(new HistoryItem(title, value));
			Balance = Balance.Add(value);
		}

		public void ClearHistory() {
			_history.Clear();
		}

		public void Welcome() {
			State.Out.Write(Content.bank_welcome);
		}

		public void Update() {
			State.Context.AddCase(
				Content.bank_balance_request,
				() => State.Out.WriteFormat(Content.bank_balance_response, Balance));

			AddCaseIfNonEmpty(Content.income_request,  Content.income_response,  item => item.Gold > Gold.Zero);
			AddCaseIfNonEmpty(Content.outcome_request, Content.outcome_response, item => item.Gold < Gold.Zero);
			State.Context.AddBackCase();
		}

		void AddCaseIfNonEmpty(string request, string response, Func<HistoryItem, bool> selector) {
			var items = _history.Where(selector);
			if ( items.Any() ) {
				State.Context.AddCase(
					request,
					() => HandleHistoryRequest(response, items));
			}
		}

		void HandleHistoryRequest(string title, IEnumerable<HistoryItem> items) {
			var sum = items.Sum(item => item.Gold.Value);
			State.Out.WriteFormat(title, Math.Abs(sum));
			foreach ( var it in items ) {
				State.Out.WriteFormat($"- {it.Title}: {Math.Abs(it.Gold.Value)}");
			}
			
		}
	}
}
