namespace TheKing.Controllers.Community {
	class Country {
		public string Name { get; }
		public Race   Kind { get; }

		public Country(string name, Race kind) {
			Name = name;
			Kind = kind;
		}
	}
}
