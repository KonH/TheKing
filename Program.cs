﻿using System.Globalization;

namespace TheKing {
	class Program {
		static void Main(string[] args) {
			//Content.Culture = new CultureInfo("ru-RU");
			var state = new GameState();
			state.Run();
		}
	}
}
