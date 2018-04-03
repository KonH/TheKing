namespace TheKing.Features.Conquest {
	public class SquadInfo {
		public int Count { get; }
		public int Loses { get; }

		public SquadInfo(int count, int loses) {
			Count = count;
			Loses = loses;
		}
	}
}
