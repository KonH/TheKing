using System;
using System.Linq;
using System.Diagnostics;
using System.Collections.Generic;
using TheKing.Utils;
using TheKing.Features.Army;
using TheKing.Features.Money;
using TheKing.Features.Countries;

namespace TheKing.Controllers {
	class ArmyController {
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

		PopulationController _population;

		Dictionary<Country, ArmyState> _armyStates = new Dictionary<Country, ArmyState>();

		public ArmyController(PopulationController population) {
			_population = population;
		}

		ArmyState GetArmy(Country country) {
			return DictUtils.GetOrCreate(country, _armyStates, () => new ArmyState());
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
			_population.Remove(country, count);
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
	}
}
