﻿using System;
using System.Diagnostics;

namespace TheKing.New {
	interface IDayStarter {
		void OnDayStart();
	}

	struct Date {
		public int Day   { get; }
		public int Month { get; }
		public int Year  { get; }

		public int TotalDays => Day + (Month - 1) * 30 + (Year - 1) * 30 * 12;

		public Date(int day, int month, int year) {
			Day   = day;
			Month = month;
			Year  = year;
		}

		public override string ToString() {
			return string.Format("{0:D2}.{1:D2}.{2:D4}", Day, Month, Year);
		}


		public static Date NextDay(Date date) {
			var newDay = date.Day + 1;
			var newMonth = date.Month;
			if ( newDay > 30 ) {
				newDay = 1;
				newMonth++;
			}
			var newYear = date.Year;
			if ( newMonth > 12 ) {
				newMonth = 1;
				newYear++;
			}
			return new Date(newDay, newMonth, newYear);
		}

		public bool IsEquals(Date other) {
			return TotalDays == other.TotalDays;
		}
	}

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
