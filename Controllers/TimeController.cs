using TheKing.Controllers.Time;

namespace TheKing.Controllers {
	class TimeController : StateController {
		public Date CurDate { get; private set; } = new Date(1, 1, 1);

		public TimeController(GameState state) : base(state) { }

		public void NextDay() {
			State.Money.ClearHistory();
			CurDate = Date.NextDay(CurDate);
			State.Out.WriteFormat(Content.time_report, CurDate);
			State.FireNextDay();
			State.Out.Write();
		}
	}
}
