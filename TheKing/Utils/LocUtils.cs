using TheKing.Features.Countries;

namespace TheKing.Utils {
	static class LocUtils {
		public static string TranslateRaceName(Country country) {
			return Content.ResourceManager.GetString("race_" + country.Kind.Id);
		}
	}
}
