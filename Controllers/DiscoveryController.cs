using System.Collections.Generic;
using TheKing.Utils;
using TheKing.Features.Map;
using TheKing.Features.Countries;

namespace TheKing.Controllers {
	class DiscoveryController {
		Dictionary<Country, Dictionary<Location, bool>> _discoveredStates
			= new Dictionary<Country, Dictionary<Location, bool>>();

		Dictionary<Location, bool> GetCountryState(Country country) {
			return DictUtils.GetOrCreate(country, _discoveredStates, () => new Dictionary<Location, bool>());
		}

		bool GetDiscoveredState(Country country, Location loc) {
			return DictUtils.GetOrCreate(loc, GetCountryState(country), () => false);
		}

		public bool IsDiscovered(Country country, Location loc) {
			if ( loc.Owner == country ) {
				return true;
			}
			return GetDiscoveredState(country, loc);
		}

		public void MarkDiscovered(Country country, Location loc) {
			var countryState = GetCountryState(country);
			if ( countryState.ContainsKey(loc) ) {
				countryState[loc] = true;
			} else {
				countryState.Add(loc, true);
			}
		}
	}
}
