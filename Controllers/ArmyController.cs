﻿using System;
using System.Diagnostics;
using TheKing.Controllers.Kingdom;

namespace TheKing.Controllers {
	class ArmyController : StateController, IUpdateHandler, IWelcomeHandler {
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
			if ( Player.Army.Count > 0 ) {
				Context.AddCase(
					Content.army_conquest_request,
					() => Context.GoTo(State.Conquest));
			}
			Context.AddBackCase();
		}

		void TryRecruit() {
			Out.WriteFormat(Content.army_recruit_request_2, Player.Army.Price, Player.Population.Count);
			while ( true ) {
				var count = Input.ReadInt();
				if ( (count > 0) && (Player.Population.Count >= count) ) {
					Out.Write(Content.army_recruit_response);
					Recruit(Player, count);
					break;	
				}
			}
		}

		public void Recruit(Country country, int count) {
			State.Population.Remove(country, count);
			country.Army.Count += count;
			Debug.WriteLine($"Recruit {country} army: +{count} = {country.Army.Count}");
		}

		public void Kill(Country country, int count) {
			country.Army.Count = Math.Max(country.Army.Count - count, 0);
			Debug.WriteLine($"Kill {country} army: -{count} = {country.Army.Count}");
		}

		void OnDayStart() {
			foreach ( var country in Countries ) {
				var usage = country.Army.GetDailyUsage();
				Money.Remove(country, $"{Content.army_name} ({country.Army.Count})", usage);
			}
		}
	}
}
