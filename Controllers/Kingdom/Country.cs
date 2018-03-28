namespace TheKing.Controllers.Kingdom {
	class Country {
		public string Name { get; }
		public Race   Kind { get; }

		public Money      Money      { get; } = new Money();
		public Population Population { get; } = new Population();
		public Army       Army       { get; } = new Army();

		public Country(string name, Race kind) {
			Name = name;
			Kind = kind;
		}

		public override string ToString() {
			return $"Country ('{Name}', {Kind.Id})";
		}
	}
}
