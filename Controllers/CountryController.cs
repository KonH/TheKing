using TheKing.Controllers.Community;

namespace TheKing.Controllers {
	class CountryController : StateController {
		public Country PlayerCountry { get; } = new Country("YourKingdom", new Race(RaceId.Human));
		// Temporary
		public Country EnemyCountry { get; } = new Country("Goblington", new Race(RaceId.Goblin));

		public CountryController(GameState state):base(state) { }

	}
}
