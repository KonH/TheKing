using TheKing.Controllers;
using TheKing.Features.Context;

namespace TheKing.Interfaces {
	class ArmyInterface : IContext<ArmyController>, IUpdateHandler, IStartHandler {
		CountryController    _country;
		PopulationController _population;
		ArmyController       _army;
		InputController      _input;
		OutputController     _out;
		ContextController    _context;

		public ArmyInterface(
			CountryController country, PopulationController population, ArmyController army,
			InputController input, OutputController output, ContextController context
		) {
			_country    = country;
			_population = population;
			_army       = army;
			_input      = input;
			_out        = output;
			_context    = context;
		}

		public void OnStart() {
			if ( _country.PlayerCountry == null ) {
				return;
			}
			_context.AddCase(Content.go_to_army, () => {
				_context.GoTo(this);
				_out.Write(Content.army_welcome);
			});
		}

		public void Update() {
			_context.AddCase(
				Content.army_recruit_request,
				TryRecruit);
			
			if ( _army.GetAvailableCount(_country.PlayerCountry) > 0 ) {
				_context.AddCase(
					Content.army_conquest_request,
					() => _context.GoToRelatedContext<ConquestController>());
			}

			_context.AddBackCase();
		}

		void TryRecruit() {
			var player = _country.PlayerCountry;
			var population = _population.GetCount(player);
			_out.WriteFormat(Content.army_recruit_request_2, _army.GetUsagePerSoldier(player), population);
			while ( true ) {
				var count = _input.ReadInt();
				if ( (count > 0) && (population >= count) ) {
					_out.Write(Content.army_recruit_response);
					_army.Recruit(player, count);
					break;
				}
			}
		}
	}
}
