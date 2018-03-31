using TheKing.Controllers;
using TheKing.Features.Time;

namespace TheKing.Handlers {
	class ArmyUpdater : IDayStarter {
		CountryController _country;
		ArmyController    _army;
		MoneyController   _money;

		public ArmyUpdater(CountryController country, ArmyController army, MoneyController money) {
			_country = country;
			_army    = army;
			_money   = money;
		}

		public void OnDayStart() {
			foreach ( var country in _country.Countries ) {
				var usage = _army.GetDailyUsage(country);
				_money.Remove(country, $"{Content.army_name} ({_army.GetTotalCount(country)})", usage);
			}
		}
	}
}
