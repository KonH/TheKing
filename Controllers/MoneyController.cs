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
			Out.Write(Content.bank_welcome);
		}

		public void Update() {
			Context.AddCase(
				Content.bank_balance_request,
				() => Out.WriteFormat(Content.bank_balance_response, Balance));

			AddCaseIfNonEmpty(Content.income_request,  Content.income_response,  item => item.Gold > Gold.Zero);
			AddCaseIfNonEmpty(Content.outcome_request, Content.outcome_response, item => item.Gold < Gold.Zero);
			Context.AddBackCase();
		}

		void AddCaseIfNonEmpty(string request, string response, Func<HistoryItem, bool> selector) {
			var items = _history.Where(selector);
			if ( items.Any() ) {
				Context.AddCase(
					request,
					() => HandleHistoryRequest(response, items));
			}
		}

		void HandleHistoryRequest(string title, IEnumerable<HistoryItem> items) {
			var sum = items.Sum(item => item.Gold.Value);
			Out.WriteFormat(title, Math.Abs(sum));
			foreach ( var it in items ) {
				Out.WriteFormat($"- {it.Title}: {Math.Abs(it.Gold.Value)}");
			}
			
		}
	}
}
