using TheKing.Controllers;
using TheKing.Features.Time;
using TheKing.Features.Countries;

namespace TheKing.Common {
	class GameLogics : IDayStarter, ICountryHandler {
		InputController   _input;
		OutputController  _out;
		ContextController _context;
		CountryController _country;
		TimeController    _time;

		bool   _isFailed;
		string _failDescription;

		public GameLogics(InputController input, OutputController output, ContextController context, CountryController country, TimeController time) {
			_input   = input;
			_out     = output;
			_context = context;
			_country = country;
			_time    = time;
		}

		public void Run() {
			_time.FirstDay();
			while ( Update() ) { }
		}

		void Fail(string description) {
			_isFailed        = true;
			_failDescription = description;
		}

		bool Update() {
			_context.ClearCases();
			_context.Update();
			_out.Write();
			if ( _isFailed ) {
				_out.Write(_failDescription);
				return false;
			}
			_context.WriteCases();
			var nextAction = _input.Update(_context.Cases);
			if ( nextAction != null ) {
				_out.Write();
				nextAction();
				return true;
			}
			return false;
		}

		public void OnDayStart() {
			_out.WriteFormat(Content.time_report, _time.CurDate);
			_out.Write();
		}

		public void OnCountryRemoved(Country country, string reason) {
			if ( _country.PlayerCountry == country ) {
				Fail(reason);
			}
		}
	}
}
