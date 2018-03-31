using System.Linq;
using System.Diagnostics;
using TheKing.Features.Countries;
using TheKing.Features.Time;

namespace TheKing.Controllers {
	class BotController : IDayStarter {
		TimeController    _time;
		CountryController _country;
		MoneyController   _money;
		ArmyController    _army;

		public BotController(TimeController time, CountryController country, MoneyController money, ArmyController army) {
			_time    = time;
			_country = country;
			_money   = money;
			_army    = army;
		}

		public void OnDayStart() {
			foreach ( var country in _country.Countries ) {
				if ( !country.Player ) {
					UpdateCountry(country);
				}
			}
		}

		void UpdateCountry(Country country) {
			var prevDate = Date.PrevDay(_time.CurDate);
			var income = _money.GetIncome(country, prevDate).Sum(it => it.Gold.Value);
			var expenses = _army.GetTotalUsage(country).Value;
			var availableMoneyPerDay = income - expenses;
			var soldierUsage = _army.GetUsagePerSoldier(country).Value;
			var newArmyCount = availableMoneyPerDay / soldierUsage - 1;
			if ( newArmyCount > 0 ) {
				Debug.WriteLine($"BotController: Recruit {newArmyCount} soldiers");
				_army.Recruit(country, newArmyCount);
			}
		}
	}
}
