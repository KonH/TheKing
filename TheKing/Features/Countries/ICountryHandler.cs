namespace TheKing.Features.Countries {
	interface ICountryHandler {
		void OnCountryRemoved(Country country, string reason);
	}
}
