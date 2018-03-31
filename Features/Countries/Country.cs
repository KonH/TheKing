namespace TheKing.Features.Countries {
	class Country {
		public string Name   { get; }
		public Race   Kind   { get; }
		public bool   Player { get; }

		public Country(string name, Race kind, bool player) {
			Name   = name;
			Kind   = kind;
			Player = player;
		}

		public override string ToString() {
			return $"('{Name}', {Kind.Id})";
		}

		public override int GetHashCode() {
			return Name.GetHashCode();
		}
	}
}
