using System;
using System.Collections.Generic;
using System.Diagnostics;
using TheKing.Controllers.Kingdom;
using TheKing.Controllers.Money;

namespace TheKing.Controllers {
	class ArmyController : StateController, IUpdateHandler, IWelcomeHandler {
		class ArmyState {
			public int  Count { get; set; }
			public Gold Price { get; set; } = new Gold(5);

			public Gold GetDailyUsage() {
				return new Gold(Count * Price.Value);
			}
		}

		Dictionary<Country, ArmyState> _armyStates = new Dictionary<Country, ArmyState>();

		public ArmyController(GameState state):base(state) {
			State.Time.OnDayStart += OnDayStart;
		}

		public void Welcome() {
			Out.Write(Content.army_welcome);
		}

		public void Update() {
			Context.AddCase(
				Content.army_recruit_request,
				TryRecruit);
			if ( GetCount(Player) > 0 ) {
				Context.AddCase(
					Content.army_conquest_request,
					() => Context.GoTo(State.Conquest));
			}
			Context.AddBackCase();
		}

		void TryRecruit() {
			var population = Population.GetCount(Player);
			Out.WriteFormat(Content.army_recruit_request_2, GetPrice(Player), population);
			while ( true ) {
				var count = Input.ReadInt();
				if ( (count > 0) && (population >= count) ) {
					Out.Write(Content.army_recruit_response);
					Recruit(Player, count);
					break;	
				}
			}
		}

		ArmyState GetArmy(Country country) {
			return Utils.GetOrCreate(country, _armyStates, () => new ArmyState());
		}

		public int GetCount(Country country) {
			return GetArmy(country).Count;
		}

		public Gold GetPrice(Country country) {
			return GetArmy(country).Price;
		}

		public Gold GetDailyUsage(Country country) {
			return GetArmy(country).GetDailyUsage();
		}

		public void Recruit(Country country, int count) {
			Population.Remove(country, count);
			var army = GetArmy(country);
			army.Count += count;
			Debug.WriteLine($"Recruit {country} army: +{count} = {army.Count}");
		}

		public void Kill(Country country, int count) {
			var army = GetArmy(country);
			army.Count = Math.Max(army.Count - count, 0);
			Debug.WriteLine($"Kill {country} army: -{count} = {army.Count}");
		}

		void OnDayStart() {
			foreach ( var country in Countries ) {
				var usage = GetDailyUsage(country);
				Money.Remove(country, $"{Content.army_name} ({GetCount(country)})", usage);
			}
		}
	}
}
