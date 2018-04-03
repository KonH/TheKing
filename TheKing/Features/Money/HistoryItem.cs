using TheKing.Features.Time;

namespace TheKing.Features.Money {
	class HistoryItem {
		public Date   Date  { get; }
		public string Title { get; }
		public Gold   Gold  { get; }

		public HistoryItem(Date date, string title, Gold value) {
			Date  = date;
			Title = title;
			Gold  = value;
		}
	}
}
