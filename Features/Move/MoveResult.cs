namespace TheKing.Features.Move {
	class MoveResult {
		public int  Loses   { get; }
		public bool Success { get; }

		public MoveResult(int loses, bool result) {
			Loses   = loses;
			Success = result;
		}
	}
}
