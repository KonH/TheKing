using TheKing.Features.Move;
using TheKing.Features.Countries;
using TheKing.Features.Map;

namespace TheKing.Features.Conquest {
	class ConquestResult {
		public Location   Location      { get; }
		public Country    Invader       { get; }
		public Country    Defender      { get; }
		public SquadInfo  InvaderSquad  { get; }
		public SquadInfo  DefenderSquad { get; }
		public bool       Success       { get; }
		public MoveResult Move          { get; }

		public ConquestResult(
			Location location, Country invader, Country defender, SquadInfo invaderSquad, SquadInfo defenderSquad, bool result, MoveResult moveResult
		) {
			Location      = location;
			Invader       = invader;
			Defender      = defender;
			InvaderSquad  = invaderSquad;
			DefenderSquad = defenderSquad;
			Success       = result;
			Move          = moveResult;
		}
	}
}
