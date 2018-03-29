using System;

namespace TheKing.New {
	class GameState {
		TimeController _time;

		public GameState(TimeController time) {
			_time = time;
		}

		public void Run(Action tempAct) {
			_time.FirstDay();
			tempAct();
			_time.NextDay();
		}
	}
}
