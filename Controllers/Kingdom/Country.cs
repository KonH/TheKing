namespace TheKing.Controllers.Kingdom {
	class Country {
		public string Name { get; }
		public Race   Kind { get; }

		public Country(string name, Race kind) {
			Name = name;
			Kind = kind;
		}

		public override string ToString() {
			return $"('{Name}', {Kind.Id})";
		}

		public override int GetHashCode() {
			return Name.GetHashCode();
		}
	}
}
