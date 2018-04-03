using TheKing.Controllers;
using TheKing.Features.Time;
using TheKing.Features.Countries;
using TheKing.Utils;

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
			if ( _context.AutoUpdate ) {
				return true;
			}
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
			} else {
				var raceName = LocUtils.TranslateRaceName(country);
				_out.WriteFormat(Content.enemy_failed, country.Name, raceName);
				if ( _country.Countries.Count == 1 ) {
					var lastCountry = _country.Countries[0];
					var lastRace = LocUtils.TranslateRaceName(lastCountry);
					Win(string.Format(Content.one_alive, lastCountry.Name, lastRace));
				}
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
