using System.Collections.Generic;
using TheKing.Controllers.Kingdom;

namespace TheKing.Controllers {
	abstract class StateController {
		private GameState State { get; }

		protected ContextController Context => State.Context;
		protected InputController   Input   => State.Input;
		protected OutputController  Out     => State.Out;

		protected CountryController CountryCtrl => State.Country;
		protected List<Country>     Countries   => State.Country.Countries;
		protected Country           Player      => State.Country.PlayerCountry;
		protected Country           Enemy       => State.Country.PlayerCountry;

		protected TimeController       Time       => State.Time;
		protected MapController        Map        => State.Map;
		protected ArmyController       Army       => State.Army;
		protected ConquestController   Conquest   => State.Conquest;
		protected MoneyController      Money      => State.Money;
		protected PopulationController Population => State.Population;

		public StateController(GameState state) {
			State = state;
		}
	}
}
