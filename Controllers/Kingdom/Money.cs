using System.Collections.Generic;
using TheKing.Controllers.Money;

namespace TheKing.Controllers.Kingdom {
	class HistoryItem {
		public string Title { get; }
		public Gold   Gold  { get; }

		public HistoryItem(string title, Gold value) {
			Title = title;
			Gold  = value;
		}
	}

	class Money {
		public Gold              Balance { get; private set; } = new Gold();
		public List<HistoryItem> History { get; private set; } = new List<HistoryItem>();

		public void Add(string title, Gold value) {
			History.Add(new HistoryItem(title, value));
			Balance = Balance.Add(value);
		}
	}
}
