using TheKing.Features.Countries;

namespace TheKing.Utils {
	static class LocUtils {
		public static string TranslateRaceName(Country country) {
			return TranslateRaceName(country.Kind.Id);
		}

		public static string TranslateRaceName(RaceId raceId) {
			return Content.ResourceManager.GetString("race_" + raceId);
		}
	}
}
