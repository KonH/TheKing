using System.Collections.Generic;
using TheKing.Controllers;

namespace TheKing {
	abstract class StateController {
		protected GameState State { get; }

		public StateController(GameState state) {
			State = state;
		}
	}

	interface IUpdateHandler {
		void Update();
	}

	interface IWelcomeHandler {
		void Welcome();
	}

	interface INextDayHandler {
		void OnNextDay();
	}

	class GameState {
		public ContextController    Context    { get; }
		public InputController      Input      { get; }
		public OutputController     Out        { get; }
		public MapController        Map        { get; }
		public MoneyController      Money      { get; }
		public TimeController       Time       { get; }
		public PopulationController Population { get; }

		public List<INextDayHandler> NextDayHandlers { get; } = new List<INextDayHandler>();

		public GameState() {
			Context    = new ContextController(this);
			Input      = new InputController(this);
			Out        = new OutputController();
			Map        = new MapController(this);
			Money      = new MoneyController(this);
			Time       = new TimeController(this);
			Population = AddController(new PopulationController(this));
		}

		T AddController<T>(T controller) {
			if ( controller is INextDayHandler nextDay ) {
				NextDayHandlers.Add(nextDay);
			}
			return controller;
		}

		public void Run() {
			while ( Update() ) { }
		}

		bool Update() {
			Context.ClearCases();
			Context.Update();
			Out.Write();
			var nextAction = Input.Update(Context.Cases);
			if ( nextAction != null ) {
				Out.Write();
				nextAction();
				return true;
			}
			return false;
		}

	}
}
