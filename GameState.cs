using TheKing.Controllers;
using TheKing.Controllers.Kingdom;

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

		bool   _isFailed;
		string _failDescription;

		public GameState() {
			Context    = new ContextController(this);
			Input      = new InputController(this);
			Out        = new OutputController();
			Country    = new CountryController();
			Map        = new MapController(this);
			Time       = new TimeController(this);
			Money      = new MoneyController(this);
			Population = new PopulationController(this);
			Army       = new ArmyController(this);
			Conquest   = new ConquestController(this);

			Time.OnDayStart          += OnDayStart;
			Country.OnCountryRemoved += CheckPlayerFail;
		}

		public void Run() {
			Time.FirstDay();
			while ( Update() ) { }
		}

		void CheckPlayerFail(Country country, string reason) {
			if ( Country.PlayerCountry == country ) {
				Fail(reason);
			}
		}

		void Fail(string description) {
			_isFailed        = true;
			_failDescription = description;
		}

		bool Update() {
			Context.ClearCases();
			Context.Update();
			Out.Write();
			if ( _isFailed ) {
				Out.Write(_failDescription);
				return false;
			}
			var nextAction = Input.Update(Context.Cases);
			if ( nextAction != null ) {
				Out.Write();
				nextAction();
				return true;
			}
			return false;
		}

		void OnDayStart() {
			Out.WriteFormat(Content.time_report, Time.CurDate);
			Out.Write();
		}
	}
}
