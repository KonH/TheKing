using TheKing.Controllers;

namespace TheKing {
	abstract class StateController {
		protected GameState State { get; }

		public StateController(GameState state) {
			State = state;
		}
	}

	interface IUpdateController {
		void Update();
	}

	class GameState {
		public ContextController Context { get; }
		public InputController   Input   { get; }
		public OutputController  Out     { get; }
		public MapController     Map     { get; }

		public GameState() {
			Context = new ContextController(this);
			Input   = new InputController(this);
			Out     = new OutputController();
			Map     = new MapController(this);
		}

		public void Run() {
			while ( Update() ) { }
		}

		bool Update() {
			Out.Write("");
			Context.ClearCases();
			Context.Update();
			Out.Write("");
			return Input.Update(Context.Cases);
		}

	}
}
