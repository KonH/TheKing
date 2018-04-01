using System;
using System.Linq;
using System.Diagnostics;
using System.Collections.Generic;
using TheKing.Utils;
using TheKing.Features.Time;
using TheKing.Features.Money;
using TheKing.Features.Countries;

namespace TheKing.Controllers {
	class MoneyController {
		class MoneyState {
			public Gold              Balance { get; private set; } = new Gold();
			public List<HistoryItem> History { get; private set; } = new List<HistoryItem>();

			public void Add(Date date, string title, Gold value) {
				History.Add(new HistoryItem(date, title, value));
				Balance = Balance.Add(value);
			}
		}

		CountryController _country;
		TimeController    _time;

		Dictionary<Country, MoneyState> _moneyStates = new Dictionary<Country, MoneyState>();

		public MoneyController(CountryController country, TimeController time) {
			_country = country;
			_time    = time;
		}

		MoneyState GetMoney(Country country) {
			return DictUtils.GetOrCreate(country, _moneyStates, () => new MoneyState());
		}

		public Gold GetBalance(Country country) {
			return GetMoney(country).Balance;
		}

		public List<HistoryItem> GetIncome(Country country, Date date) {
			return GetHistoryItems(country, date, item => item.Gold > Gold.Zero);
		}



		public List<HistoryItem> GetExpenses(Country country, Date date) {
			return GetHistoryItems(country, date, item => item.Gold < Gold.Zero);
		}

		List<HistoryItem> GetHistoryItems(Country country, Date date, Func<HistoryItem, bool> selector) {
			var items = GetMoney(country).History.
				Where(it => it.Date.IsEquals(date)).
				Where(selector);
			return items.ToList();
		}

		public void Add(Country country, string title, Gold gold) {
			if ( gold.Value == 0 ) {
				return;
			}
			if ( (_country.PlayerCountry == country) && (gold.Value < 0) && Cheats.Money_NoLoses ) {
				return;
			}
			var money = GetMoney(country);
			var curDate = _time.CurDate;
			money.Add(curDate, title, gold);
			Debug.WriteLine($"Add {country} money: {gold} ('{title}') = {money.Balance}");
		}

		public void Remove(Country country, string title, Gold gold) {
			Add(country, title, new Gold(-gold.Value));
			CheckMoneyFail(country);
		}

		void CheckMoneyFail(Country country) {
			if ( GetMoney(country).Balance.Value < 0 ) {
				_country.Remove(country, Content.game_over_money);
			}
		}
	}
}
