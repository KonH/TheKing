namespace TheKing.Controllers {
	abstract class StateController {
		protected GameState State { get; }

		public StateController(GameState state) {
			State = state;
		}
	}
}
