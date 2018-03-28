using System;
using System.Diagnostics;
using TheKing.Controllers.Time;

namespace TheKing.Controllers {
	class TimeController : StateController {
		public Date CurDate { get; private set; } = new Date(1, 1, 1);

		public Action OnDayStart = new Action(() => { });
		public Action OnDayEnd   = new Action(() => { });

		public TimeController(GameState state) : base(state) { }

		public void FirstDay() {
			FireDayStart();
		}

		public void NextDay() {
			FireDayEnd();
			CurDate = Date.NextDay(CurDate);
			FireDayStart();
		}

		void FireDayStart() {
			Debug.WriteLine($"Start of the day: {CurDate}");
			OnDayStart.Invoke();
		}

		void FireDayEnd() {
			Debug.WriteLine($"End of the day: {CurDate}");
			OnDayEnd.Invoke();
		}
	}
}
