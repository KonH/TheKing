using System;
using System.Collections.Generic;
using System.Linq;
using TheKing.Controllers;
using TheKing.Features.Context;
using TheKing.Features.Money;

namespace TheKing.Interfaces {
	class MoneyInteface : IUpdateHandler, IStartHandler {
		OutputController  _out;
		ContextController _context;
		CountryController _country;
		MoneyController   _money;
		TimeController    _time;

		public MoneyInteface(OutputController output, ContextController context, CountryController country, MoneyController money, TimeController time) {
			_out     = output;
			_context = context;
			_country = country;
			_money   = money;
			_time    = time;
		}

		public void OnStart() {
			_context.AddCase(Content.go_to_bank, () => {
				_context.GoTo(this);
				_out.Write(Content.bank_welcome);
			});
		}

		public void Update() {
			_context.AddCase(
				Content.bank_balance_request,
				() => _out.WriteFormat(Content.bank_balance_response, _money.GetBalance(_country.PlayerCountry)));

			AddCaseIfNonEmpty(Content.income_request,  Content.income_response,  _money.GetIncome(_country.PlayerCountry, _time.CurDate));
			AddCaseIfNonEmpty(Content.outcome_request, Content.outcome_response, _money.GetExpenses(_country.PlayerCountry, _time.CurDate));
			_context.AddBackCase();
		}

		void AddCaseIfNonEmpty(string request, string response, List<HistoryItem> items) {
			if ( items.Any() ) {
				_context.AddCase(
					request,
					() => HandleHistoryRequest(response, items));
			}
		}

		void HandleHistoryRequest(string title, IEnumerable<HistoryItem> items) {
			var sum = items.Sum(item => item.Gold.Value);
			_out.WriteFormat(title, Math.Abs(sum));
			foreach ( var it in items ) {
				_out.WriteFormat($"- {it.Title}: {Math.Abs(it.Gold.Value)}");
			}
		}
	}
}
