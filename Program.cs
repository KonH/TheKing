using System;
using System.Globalization;

namespace TheKing {
	class Program {
		static void Main(string[] args) {
			RunWithCulture(null);
		}

		static void RunWithCulture(string cultureKey) {
			if ( !string.IsNullOrEmpty(cultureKey) ) {
				Content.Culture = new CultureInfo("ru-RU");
			}
			var state = new GameState();
			state.Run();
			Console.WriteLine("Press any key...");
			Console.ReadKey();
		}
	}
}
