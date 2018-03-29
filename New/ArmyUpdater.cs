namespace TheKing.New {
	class ArmyUpdater {
		CountryController _country;
		ArmyController    _army;
		MoneyController   _money;

		public ArmyUpdater(TimeController time, CountryController country, ArmyController army, MoneyController money) {
			_country = country;
			_army    = army;
			_money   = money;
			time.OnDayStart += OnDayStart;
		}

		void OnDayStart() {
			foreach ( var country in _country.Countries ) {
				var usage = _army.GetDailyUsage(country);
				_money.Remove(country, $"{Content.army_name} ({_army.GetTotalCount(country)})", usage);
			}
		}
	}
}
