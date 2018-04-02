using System;
using System.Linq;
using System.Diagnostics;
using System.Collections.Generic;
using TheKing.Utils;
using TheKing.Features.Bot;
using TheKing.Features.Time;
using TheKing.Features.Conquest;
using TheKing.Features.Countries;

namespace TheKing.Controllers {
	class BotController : IDayStarter {
		TimeController     _time;
		CountryController  _country;
		MoneyController    _money;
		ArmyController     _army;
		ConquestController _conquest;

		Dictionary<Country, BotBehaviour> _behaviors = new Dictionary<Country, BotBehaviour>();

		public BotController(
			TimeController time, CountryController country, MoneyController money, ArmyController army, ConquestController conquest
		) {
			_time     = time;
			_country  = country;
			_money    = money;
			_army     = army;
			_conquest = conquest;
		}

		public void OnDayStart() {
			foreach ( var country in _country.Countries ) {
				if ( !country.Player ) {
					UpdateCountry(country);
				}
			}
		}

		BotBehaviour GetBehaviour(Country country) {
			return DictUtils.GetOrCreate(country, _behaviors, () => new BotBehaviour());
		}

		void UpdateCountry(Country country) {
			var behaviour = GetBehaviour(country);
			Recruit(country, behaviour);
			Conquest(country, behaviour);
		}

		void Recruit(Country country, BotBehaviour behaviour) {
			var prevDate = Date.PrevDay(_time.CurDate);
			var income = _money.GetIncome(country, prevDate).Sum(it => it.Gold.Value);
			var expenses = _army.GetTotalUsage(country).Value;
			var availableMoneyPerDay = income - expenses;
			var soldierUsage = _army.GetUsagePerSoldier(country).Value;
			var newArmyCount = availableMoneyPerDay / soldierUsage - 1;
			if ( newArmyCount > 0 ) {
				Debug.WriteLine($"BotController ({country}).Recruit: {newArmyCount} soldiers");
				_army.Recruit(country, newArmyCount);
			}
		}

		void Conquest(Country country, BotBehaviour behaviour) {
			var totalArmy = _army.GetTotalCount(country);
			var availableArmy = _army.GetAvailableCount(country);
			var canBeUsedArmy = (int)Math.Ceiling(availableArmy - totalArmy * (1 - behaviour.Aggressive));
			if ( canBeUsedArmy > 0 ) {
				Debug.WriteLine(
					$"BotController ({country}).Conquest: Can send {canBeUsedArmy} soldiers (available = {availableArmy}, total = {totalArmy})"
				);
				var acceptableLocs = _conquest.GetAcceptableLocations(country);
				if ( acceptableLocs.Count > 0 ) {
					var selectedLocs = acceptableLocs;
					var squadSize = Math.Max(canBeUsedArmy / selectedLocs.Count, 1);
					Debug.WriteLine(
						$"BotController ({country}).Conquest: Squad size = {squadSize}, " +
						$"locations = (all = {acceptableLocs.Count}, selected = {selectedLocs.Count})"
					);
					foreach ( var loc in selectedLocs ) {
						var homeLoc = loc.Item1;
						var targetLoc = loc.Item2;
						var maxCount = _army.GetAvailableCount(country);
						if ( maxCount > 0 ) {
							var curCount = Math.Min(squadSize, maxCount);
							var squad = _army.TryAquireSquad(country, curCount);
							_conquest.StartConquest(country, squad, homeLoc, targetLoc, (r, _) => OnConquestComplete(r));
						}
					}
				}
			}

			void OnConquestComplete(ConquestResult result) {
				// TODO
			}
		}
	}
}
