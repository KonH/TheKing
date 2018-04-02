using System.Collections.Generic;

namespace TheKing.Controllers {
	class StartMenuController {
		InputController  _input;
		OutputController _output;
		CheatController  _cheats;

		public StartMenuController(InputController input, OutputController output, CheatController cheats = null) {
			_input  = input;
			_output = output;
			_cheats = cheats;
		}

		public void Run() {
			_output.Write(Content.title);
			_output.Write();
			if ( _cheats != null ) {
				_output.Write("CHEATS:");

				var cheatDict = new Dictionary<char, string> {
					{ 'm', nameof(CheatController.MoneyDecreaseDisabed) },
					{ 'd', nameof(CheatController.AllDiscovered) }
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
			}
			_output.Write();
		}
	}
}
