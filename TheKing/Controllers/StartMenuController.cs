using System.Collections.Generic;
using TheKing.Utils;
using TheKing.Settings;

namespace TheKing.Controllers {
	class StartMenuController {
		GameSettings     _settings;
		InputController  _input;
		OutputController _output;
		RaceSettings     _races;
		CheatController  _cheats;

		public StartMenuController(
			GameSettings settings, InputController input, OutputController output, RaceSettings races, CheatController cheats = null
		) {
			_settings = settings;
			_input    = input;
			_output   = output;
			_races    = races;
			_cheats   = cheats;
		}

		public void Run() {
			_output.Write(Content.title);
			_output.Write();
			UpdateCheats();
			if ( _settings.WithPlayer ) {
				UpdateRaceSelection();
			}
		}

		void UpdateCheats() {
			if ( _cheats != null ) {
				_output.Write("CHEATS:");

				var cheatDict = new Dictionary<char, string> {
					{ 'm', nameof(CheatController.MoneyDecreaseDisabled) },
					{ 'd', nameof(CheatController.AllDiscovered) },
					{ 'c', nameof(CheatController.NonConquestable) },
					{ 'f', nameof(CheatController.FreeMode) }
				};

				foreach ( var cheat in cheatDict ) {
					_output.Write($"{cheat.Value} => '{cheat.Key}'");
				}
				var input = _input.ReadString();
				foreach ( var cheat in cheatDict ) {
					if ( input.Contains(cheat.Key.ToString()) ) {
						typeof(CheatController).GetProperty(cheat.Value).SetValue(_cheats, true);
					}
				}
				if ( _cheats.FreeMode ) {
					_settings.WithPlayer = false;
				}
			}
			_output.Write();
		}

		void UpdateRaceSelection() {
			_output.Write(Content.race_select);
			var races = _races.AllRaces;
			for ( var i = 0; i < races.Count; i++ ) {
				_output.Write($"{i + 1}) {LocUtils.TranslateRaceName(races[i])}");
			}
			do {
				var selection = _input.ReadInt();
				if ( (selection > 0) && (selection <= races.Count)) {
					_races.SelectPlayerRace(races[selection - 1]);
					return;
				}
			} while ( true );
		}
	}
}
