namespace TheKing.Controllers.Community {
	enum RaceId {
		Human,
		Goblin
	}

	class Race {
		public RaceId Id { get; }

		public Race(RaceId name) {
			Id = name;
		}
	}
}
