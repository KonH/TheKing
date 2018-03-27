using TheKing.Controllers.Money;

namespace TheKing.Controllers {
	class ArmyController : StateController, IUpdateHandler, IWelcomeHandler {
		public int Count { get; set; }
		public int Price { get; set; } = 5;

		public ArmyController(GameState state):base(state) {
			state.OnNextDay += OnNextDay;
		}

		public void Welcome() {
			Out.Write(Content.army_welcome);
		}

		public void Update() {
			Context.AddCase(
				Content.army_recruit_request,
				TryRecruit);
			if ( Count > 0 ) {
				Context.AddCase(
					Content.army_conquest_request,
					() => Context.GoTo(State.Conquest));
			}
			Context.AddBackCase();
		}

		void TryRecruit() {
			Out.WriteFormat(Content.army_recruit_request_2, Price);
			while ( true ) {
				var value = Input.ReadInt();
				if ( (value > 0) && (State.Population.Count > value) ) {
					Out.Write(Content.army_recruit_response);
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
