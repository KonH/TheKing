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

		bool   _isFail;
		bool   _isWin;
		string _endDescription;

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
			_isFail        = true;
			_endDescription = description;
		}

		void Win(string description) {
			_isWin          = true;
			_endDescription = description;
		}

		bool Update() {
			_context.ClearCases();
			_context.Update();
			_out.Write();
			if ( _isWin || _isFail ) {
				_out.Write(_endDescription);
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
			_out.Write();
			_out.WriteFormat(Content.time_report, _time.CurDate);
			_out.Write();
		}

		public void OnCountryRemoved(Country country, string reason) {
			if ( _country.PlayerCountry == country ) {
				Fail(reason);
			}
			if ( !HasOtherCountries() ) {
				Win(Content.win_conquest);
			}
		}

		bool HasOtherCountries() {
			foreach ( var country in _country.Countries ) {
				if ( !country.Player ) {
					return true;
				}
			}
			return false;
		}
	}
}
