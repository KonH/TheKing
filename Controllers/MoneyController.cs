using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using TheKing.Controllers.Kingdom;
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

		class MoneyState {
			public Gold              Balance { get; private set; } = new Gold();
			public List<HistoryItem> History { get; private set; } = new List<HistoryItem>();

			public void Add(string title, Gold value) {
				History.Add(new HistoryItem(title, value));
				Balance = Balance.Add(value);
			}
		}

		Dictionary<Country, MoneyState> _moneyStates = new Dictionary<Country, MoneyState>();

		public MoneyController(GameState state) : base(state) {
			Time.OnDayEnd += OnDayEnd;
		}

		MoneyState GetMoney(Country country) {
			return Utils.GetOrCreate(country, _moneyStates, () => new MoneyState());
		}

		public void Add(Country country, string title, Gold gold) {
			if ( gold.Value == 0 ) {
				return;
			}
			var money = GetMoney(country);
			money.Add(title, gold);
			Debug.WriteLine($"Add {country} money: {gold} ('{title}') = {money.Balance}");
		}

		public void Remove(Country country, string title, Gold gold) {
			Add(country, title, new Gold(-gold.Value));
			CheckFail(country);
		}

		void OnDayEnd() {
			ClearHistory();
		}

		void CheckFail(Country country) {
			if ( GetMoney(country).Balance.Value < 0 ) {
				CountryCtrl.Remove(country, Content.game_over_money);
			}
		}

		void ClearHistory() {
			foreach ( var country in Countries ) {
				GetMoney(country).History.Clear();
			}
		}

		public void Welcome() {
			Out.Write(Content.bank_welcome);
		}

		public void Update() {
			Context.AddCase(
				Content.bank_balance_request,
				() => Out.WriteFormat(Content.bank_balance_response, GetMoney(Player).Balance));

			AddCaseIfNonEmpty(Content.income_request,  Content.income_response,  item => item.Gold > Gold.Zero);
			AddCaseIfNonEmpty(Content.outcome_request, Content.outcome_response, item => item.Gold < Gold.Zero);
			Context.AddBackCase();
		}

		void AddCaseIfNonEmpty(string request, string response, Func<HistoryItem, bool> selector) {
			var items = GetMoney(Player).History.Where(selector);
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
