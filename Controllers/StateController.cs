﻿using System.Collections.Generic;
using TheKing.Controllers.Kingdom;

namespace TheKing.Controllers {
	abstract class StateController {
		protected GameState         State   { get; }
		protected ContextController Context => State.Context;
		protected InputController   Input   => State.Input;
		protected OutputController  Out     => State.Out;

		protected List<Country>        Countries  => State.Country.Countries;
		protected Country              Player     => State.Country.PlayerCountry;
		protected ArmyController       Army       => State.Army;
		protected MoneyController      Money      => State.Money;
		protected PopulationController Population => State.Population;

		public StateController(GameState state) {
			State = state;
		}
	}
}
