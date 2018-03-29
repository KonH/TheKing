using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using TheKing.Controllers.Kingdom;
using TheKing.Controllers.Money;

namespace TheKing.Controllers {
	interface IReadOnlySquad {
		int Count { get; }
	}

	class ArmyController : StateController, IUpdateHandler, IWelcomeHandler {
		class Squad : IReadOnlySquad {
			public int  Count { get; set; }
			public bool Used  { get; private set; }
			public bool Free => !Used;

			public Squad(int count = 0) {
				Count = count;
			}

			public Squad Aquire() {
				Used = true;
				return this;
			}

			public void Release() {
				Used = false;
			}
		}

		class ArmyState {
			public Gold        Usage  { get; set; } = new Gold(5);
			public List<Squad> Squads { get; }      = new List<Squad>();
		}

		Dictionary<Country, ArmyState> _armyStates = new Dictionary<Country, ArmyState>();

		public ArmyController(GameState state):base(state) {
			Time.OnDayStart += OnDayStart;
		}

		public void Welcome() {
			Out.Write(Content.army_welcome);
		}

		public void Update() {
			Context.AddCase(
				Content.army_recruit_request,
				TryRecruit);
			if ( GetAvailableCount(Player) > 0 ) {
				Context.AddCase(
					Content.army_conquest_request,
					() => Context.GoTo(Conquest));
			}
			Context.AddBackCase();
		}

		void TryRecruit() {
			var population = Population.GetCount(Player);
			Out.WriteFormat(Content.army_recruit_request_2, GetUsage(Player), population);
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

		Squad GetOrCreateSquad(ArmyState army) {
			var firstFreeSquad = army.Squads.FirstOrDefault(s => s.Free);
			if ( firstFreeSquad == null ) {
				var newSquad = new Squad();
				army.Squads.Add(newSquad);
				return newSquad;
			}
			return firstFreeSquad;
		}

		public int GetTotalCount(Country country) {
			return GetArmy(country).Squads
				.Sum(s => s.Count);
		}

		public int GetAvailableCount(Country country) {
			return GetArmy(country).Squads
				.Where(s => s.Free)
				.Sum(s => s.Count);
		}

		public Gold GetUsage(Country country) {
			return GetArmy(country).Usage;
		}

		public Gold GetDailyUsage(Country country) {
			return GetArmy(country).Usage * GetTotalCount(country);
		}

		public void Recruit(Country country, int count) {
			Population.Remove(country, count);
			var army = GetArmy(country);
			var squad = GetOrCreateSquad(army);
			squad.Count += count;
			Debug.WriteLine($"Recruit {country} army: +{count} = {GetTotalCount(country)}");
		}

		public IReadOnlySquad TryAquireSquad(Country country, int count) {
			var squads = GetArmy(country).Squads;
			var firstFitSquad = squads.FirstOrDefault(s => s.Free && (s.Count >= count));
			if ( firstFitSquad != null ) {
				squads.Remove(firstFitSquad);
				var restCount = firstFitSquad.Count - count;
				if ( restCount > 0 ) {
					squads.Add(new Squad(restCount));
				}
				var newSquad = new Squad(count).Aquire();
				squads.Add(newSquad);
				Debug.WriteLine($"Aquire {country} squad: {newSquad.Count} (free now: {GetAvailableCount(country)})");
				return newSquad;
			}
			return null;
		}

		public IReadOnlySquad AquireMaxSquad(Country country) {
			var squad = GetOrCreateSquad(GetArmy(country)).Aquire();
			Debug.WriteLine($"Aquire {country} squad: {squad.Count} (free now: {GetAvailableCount(country)})");
			return squad;
		}

		void EnsureSquadContains(Squad squad, List<Squad> squads) {
			if ( !squads.Contains(squad) ) {
				throw new ArgumentException("Invalid squad");
			}
		}

		public void ReleaseSquad(Country country, IReadOnlySquad squadWrapper) {
			var squad = (squadWrapper as Squad);
			var squads = GetArmy(country).Squads;
			EnsureSquadContains(squad, squads);
			var firstFreeSquad = squads.FirstOrDefault(s => s.Free);
			if ( firstFreeSquad != null ) {
				firstFreeSquad.Count += squad.Count;
				squads.Remove(squad);
			} else {
				squad.Release();
			}
			Debug.WriteLine($"Release {country} squad: {squad.Count} (free now: {GetAvailableCount(country)})");
		}

		public IReadOnlySquad KillInSquad(Country country, IReadOnlySquad squadWrapper, int count) {
			var squad = (squadWrapper as Squad);
			var squads = GetArmy(country).Squads;
			EnsureSquadContains(squad, squads);
			squad.Count -= count;
			if ( squad.Count < 0 ) {
				squads.Remove(squad);
				squad = null;
			}
			Debug.WriteLine($"Kill {country} in army squad: -{count} = {GetTotalCount(country)}");
			return squad;
		}

		void OnDayStart() {
			foreach ( var country in Countries ) {
				var usage = GetDailyUsage(country);
				Money.Remove(country, $"{Content.army_name} ({GetTotalCount(country)})", usage);
			}
		}
	}
}
