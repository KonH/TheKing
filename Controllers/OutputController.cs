using System;

namespace TheKing.Controllers {
	class OutputController {

		public void Write(string text) {
			Console.WriteLine(text);
		}

		public void WriteFormat(string text, params object[] args) {
			Console.WriteLine(text, args);
		}
	}
}
