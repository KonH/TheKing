using System;

namespace TheKing.Controllers {
	class OutputController {
		public void Write(string text = "") {
			Console.WriteLine(text);
		}

		public void WriteFormat(string text, params object[] args) {
			Console.WriteLine(text, args);
		}

		public void WriteCustom(string text, ConsoleColor foregroundColor) {
			var prevColor = Console.ForegroundColor;
			Console.ForegroundColor = foregroundColor;
			Console.Write(text);
			Console.ForegroundColor = prevColor;
		}

		public void WriteCustomFormat(string text, ConsoleColor foregroundColor, params object[] args) {
			WriteCustom(string.Format(text, args), foregroundColor);
		}
	}
}
