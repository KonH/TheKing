namespace TheKing.Controllers {
	class ArmyController : StateController, IUpdateHandler, IWelcomeHandler {
		public int Count { get; set; }
		public int Price { get; set; } = 5;

		public ArmyController(GameState state):base(state) {
			state.OnNextDay += OnNextDay;
		}

		public void Welcome() {
			State.Out.Write(Content.army_welcome);
		}

		public void Update() {
			State.Context.AddCase(
				Content.army_recruit_request,
				TryRecruit);
			State.Context.AddBackCase();
		}

		void TryRecruit() {
			State.Out.Write(Content.army_recruit_request_2);
			while ( true ) {
				var value = State.Input.ReadInt();
				if ( (value > 0) && (State.Population.Count > value) ) {
					State.Out.Write(Content.army_recruit_response);
					State.Population.Count -= value;
					Count += value;
					break;	
				}
			}
		}

		void OnNextDay() {
			State.Money.Add($"{Content.army_name} ({Count})", new Gold(-Count * Price));
		}
	}
}
