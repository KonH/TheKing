using System;
using System.Collections.Generic;
using TheKing.Controllers;
using TheKing.Features.Context;
using TheKing.Features.Countries;
using TheKing.Features.Map;
using TheKing.Utils;

namespace TheKing.Interfaces {
	class FreeModeInterface : IStartHandler {
		OutputController _out;
		ContextController _context;
		CountryController _country;
		MapController     _map;
		TimeController    _time;

		Dictionary<Country, ConsoleColor> _colorMap = new Dictionary<Country, ConsoleColor>();

		public FreeModeInterface(
			OutputController output, ContextController context,
			CountryController country, MapController map, TimeController time
		) {
			_out     = output;
			_context = context;
			_country = country;
			_map     = map;
			_time    = time;
		}

		public void OnStart() {
			if ( _country.PlayerCountry == null ) {
				if ( _context.AutoUpdate ) {
					_time.NextDay();
				} else {
					_context.AddCase("Next", () => {
						_time.NextDay();
					});
					_context.AddCase("Auto", () => {
						_context.AutoUpdate = true;
					});
				}
				Overview();
				ShowLegend();
			}
		}

		void Overview() {
			for ( var y = 0; ; y++ ) {
				for ( var x = 0; ; x++ ) {
					var pos = new Point(x, y);
					var locAt = _map.GetLocationAt(pos);
					if ( locAt == null ) {
						_out.Write();
						if ( x == 0 ) {
							return;
						}
						break;
					}
					var locName = locAt.Name;
					var color = GetLocationColor(locAt);
					_out.WriteCustomFormat("{0,20} ", color, locName);
				}
			}
		}

		ConsoleColor GetLocationColor(Location loc) {
			if ( loc.Owner != null ) {
				return GetCountryColor(loc.Owner);
			}
			return ConsoleColor.White;
		}

		ConsoleColor GetCountryColor(Country country) {
			return DictUtils.GetOrCreate(country, _colorMap, () => GetNewCountryColor(_colorMap.Values));
		}

		ConsoleColor GetNewCountryColor(ICollection<ConsoleColor> exceptions) {
			var values = new List<ConsoleColor>((ConsoleColor[])Enum.GetValues(typeof(ConsoleColor)));
			values.Remove(ConsoleColor.Black);
			values.Remove(ConsoleColor.White);
			values.Remove(ConsoleColor.Green);
			foreach ( var exc in exceptions ) {
				if ( values.Count > 0 ) {
					values.Remove(exc);
				}
			}
			return RandUtils.GetItem(values);
		}

		void ShowLegend() {
			_out.Write();
			foreach ( var pair in _colorMap ) {
				if ( _country.Countries.Contains(pair.Key) ) {
					_out.WriteCustom($"{pair.Key.Name} ({LocUtils.TranslateRaceName(pair.Key)}) ", pair.Value);
				}
			}
		}
	}
}
