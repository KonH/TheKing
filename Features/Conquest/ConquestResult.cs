using TheKing.Features.Move;

namespace TheKing.Features.Conquest {
	class ConquestResult {
		public int        Loses   { get; }
		public bool       Success { get; }
		public MoveResult Move    { get; }

		public ConquestResult(int loses, bool result, MoveResult moveResult) {
			Loses   = loses;
			Success = result;
			Move    = moveResult;
		}
	}
}
