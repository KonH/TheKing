using System;
using TheKing.Controllers;

namespace TheKing {
	class GameState {
		public ContextController    Context    { get; }
		public InputController      Input      { get; }
		public OutputController     Out        { get; }
		public CountryController    Country    { get; }
		public MapController        Map        { get; }
		public MoneyController      Money      { get; }
		public TimeController       Time       { get; }
		public PopulationController Population { get; }
		public ArmyController       Army       { get; }
		public ConquestController   Conquest   { get; }

		public event Action OnNextDay = new Action(() => {});

		bool _failed;

		public GameState() {
			Context    = new ContextController(this);
			Input      = new InputController(this);
			Out        = new OutputController();
			Country    = new CountryController(this);
			Map        = new MapController(this);
			Money      = new MoneyController(this);
			Time       = new TimeController(this);
			Population = new PopulationController(this);
			Army       = new ArmyController(this);
			Conquest   = new ConquestController(this);
		}

		public void Run() {
			while ( Update() ) { }
		}

		bool Update() {
			if ( _failed ) {
				return false;
			}
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

		public void FireNextDay() {
			OnNextDay?.Invoke();
			if ( Money.Balance.Value < 0 ) {
				Out.Write(Content.game_over_money);
				_failed = true;
			}
		}
	}
}
