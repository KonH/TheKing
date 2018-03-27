namespace TheKing.Controllers {
	abstract class StateController {
		protected GameState         State   { get; }
		protected ContextController Context => State.Context;
		protected InputController   Input   => State.Input;
		protected OutputController  Out     => State.Out;

		public StateController(GameState state) {
			State = state;
		}
	}
}
