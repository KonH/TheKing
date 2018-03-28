using TheKing.Controllers.Money;

namespace TheKing.Controllers.Kingdom {
	class Army {
		public int Count { get; set; }
		public int Price { get; set; } = 5;

		public Gold GetDailyUsage() {
			return new Gold(Count * Price);
		}
	}
}
