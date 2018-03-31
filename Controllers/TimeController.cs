using System;
using System.Diagnostics;
using TheKing.Features.Time;

namespace TheKing.Controllers {
	class TimeController {
		public Date CurDate { get; private set; } = new Date(1, 1, 1);

		public event Action OnDayStart = new Action(() => { });

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
		}
	}
}
