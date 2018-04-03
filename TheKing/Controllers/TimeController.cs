using System;
using System.Diagnostics;
using System.Collections.Generic;
using TheKing.Features.Time;

namespace TheKing.Controllers {
	class TimeController {
		class SheduleItem {
			public int    Days     { get; set; }
			public Action Callback { get; private set; }

			public SheduleItem(int days, Action callback) {
				Days     = days;
				Callback = callback;
			}
		}

		public Date CurDate { get; private set; } = new Date(1, 1, 1);

		public event Action OnDayStart = new Action(() => { });

		List<SheduleItem> _sheduledActions = new List<SheduleItem>();

		public void FirstDay() {
			FireDayStart();
		}

		public void NextDay() {
			FireDayEnd();
			CurDate = Date.NextDay(CurDate);
			FireDayStart();
		}

		public void SheduleAction(int days, Action action) {
			_sheduledActions.Add(new SheduleItem(days, action));
		}

		void FireDayStart() {
			Debug.WriteLine($"Start of the day: {CurDate}");
			ProcessSheduledActions();
			OnDayStart.Invoke();
			
		}

		void FireDayEnd() {
			Debug.WriteLine($"End of the day: {CurDate}");
		}

		void ProcessSheduledActions() {
			for ( var i = 0; i < _sheduledActions.Count; i++ ) {
				var item = _sheduledActions[i];
				item.Days--;
				if ( item.Days <= 0 ) {
					item.Callback();
					_sheduledActions.RemoveAt(i);
					i--;
				}
			}
		}
	}
}
