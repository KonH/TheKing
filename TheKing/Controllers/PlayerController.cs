namespace TheKing.Controllers {
	class PlayerController : IGameController {
		ContextController _context;
		InputController   _input;
		OutputController  _out;

		public PlayerController(ContextController context, InputController input, OutputController output) {
			_context = context;
			_input   = input;
			_out     = output;
		}

		public bool Update() {
			_context.WriteCases();
			var nextAction = _input.Update(_context.Cases);
			if ( nextAction != null ) {
				_out.Write();
				nextAction();
				return true;
			}
			return false;
		}
	}
}
