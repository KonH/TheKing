using TheKing.Features.Move;
using TheKing.Features.Countries;

namespace TheKing.Features.Conquest {
	class ConquestResult {
		public Country    PrevOwner { get; }
		public int        Loses     { get; }
		public bool       Success   { get; }
		public MoveResult Move      { get; }

		public ConquestResult(Country prevOwner, int loses, bool result, MoveResult moveResult) {
			PrevOwner = prevOwner;
			Loses     = loses;
			Success   = result;
			Move      = moveResult;
		}
	}
}
