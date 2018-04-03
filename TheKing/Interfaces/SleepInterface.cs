using TheKing.Controllers;
using TheKing.Features.Context;

namespace TheKing.Interfaces {
	class SleepInterface : IStartHandler {
		CountryController _country;
		TimeController    _time;
		ContextController _context;

		public SleepInterface(CountryController country, ContextController context, TimeController time) {
			_country = country;
			_context = context;
			_time    = time;
		}

		public void OnStart() {
			if ( _country.PlayerCountry != null ) {
				_context.AddCase(Content.next_day, () => {
					_time.NextDay();
				});
			}
		}
	}
}
