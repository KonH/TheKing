using System;
using System.Collections.Generic;
using System.Linq;
using TheKing.Controllers.Kingdom;
using TheKing.Controllers.Money;

namespace TheKing.Controllers {
	class MoneyController : StateController, IUpdateHandler, IWelcomeHandler {
		public MoneyController(GameState state) : base(state) {
			State.Time.OnDayEnd += OnDayEnd;
		}

		public void Add(Country country, string title, Gold value) {
			country.Money.Add(title, value);
		}

		public void Remove(Country country, string title, Gold value) {
			Add(country, title, new Gold(-value.Value));
			CheckFail(country);
		}

		void OnDayEnd() {
			ClearHistory();
		}

		void CheckFail(Country country) {
			if ( country.Money.Balance.Value < 0 ) {
				State.Country.Remove(country, Content.game_over_money);
			}
		}

		void ClearHistory() {
			foreach ( var country in Countries ) {
				country.Money.History.Clear();
			}
		}

		public void Welcome() {
			Out.Write(Content.bank_welcome);
		}

		public void Update() {
			Context.AddCase(
				Content.bank_balance_request,
				() => Out.WriteFormat(Content.bank_balance_response, Player.Money.Balance));

			AddCaseIfNonEmpty(Content.income_request,  Content.income_response,  item => item.Gold > Gold.Zero);
			AddCaseIfNonEmpty(Content.outcome_request, Content.outcome_response, item => item.Gold < Gold.Zero);
			Context.AddBackCase();
		}

		void AddCaseIfNonEmpty(string request, string response, Func<HistoryItem, bool> selector) {
			var items = Player.Money.History.Where(selector);
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
