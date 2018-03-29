using System.Collections.Generic;
using System.Diagnostics;

namespace TheKing.New {
	class MoneyController {
		class HistoryItem {
			public Date   Date  { get; }
			public string Title { get; }
			public Gold   Gold  { get; }

			public HistoryItem(Date date, string title, Gold value) {
				Title = title;
				Gold  = value;
			}
		}

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
			return Utils.GetOrCreate(country, _moneyStates, () => new MoneyState());
		}

		public void Add(Country country, string title, Gold gold) {
			if ( gold.Value == 0 ) {
				return;
			}
			var money = GetMoney(country);
			var curDate = _time.CurDate;
			money.Add(curDate, title, gold);
			Debug.WriteLine($"Add {country} money: {gold} ('{title}') = {money.Balance}");
		}

		public void Remove(Country country, string title, Gold gold) {
			Add(country, title, new Gold(-gold.Value));
			CheckFail(country);
		}

		void CheckFail(Country country) {
			if ( GetMoney(country).Balance.Value < 0 ) {
				_country.Remove(country, Content.game_over_money);
			}
		}
	}
}
