using System;

namespace TheKing.New {
	class OutputController {
		public void Write(string text = "") {
			Console.WriteLine(text);
		}

		public void WriteFormat(string text, params object[] args) {
			Console.WriteLine(text, args);
		}
	}
}
