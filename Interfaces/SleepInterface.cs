using TheKing.Controllers;
using TheKing.Features.Context;

namespace TheKing.Interfaces {
	class SleepInterface : IStartHandler {
		TimeController    _time;
		ContextController _context;

		public SleepInterface(ContextController context, TimeController time) {
			_context = context;
			_time    = time;
		}

		public void OnStart() {
			_context.AddCase(Content.next_day, () => {
				_time.NextDay();
			});
		}
	}
}
