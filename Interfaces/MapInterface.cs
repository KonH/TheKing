using TheKing.Controllers;
using TheKing.Features.Map;
using TheKing.Features.Context;

namespace TheKing.Interfaces {
	class MapInterface : IUpdateHandler, IStartHandler {

		MapController     _map;
		OutputController  _out;
		ContextController _context;

		Point _position;

		public MapInterface(MapController map, OutputController output, ContextController context) {
			_map     = map;
			_out     = output;
			_context = context;
		}

		public void OnStart() {
			_context.AddCase(Content.go_to_map, () => _context.GoTo(this));
		}

		public void Update() {
			var curLocation = _map.GetLocationAt(_position);
			DescribeLocation(curLocation);

			foreach ( var dir in _map.GetDirections() ) {
				var locationAt = _map.GetLocationAt(_position, dir);
				if ( locationAt != null ) {
					var title = string.Format(Content.look_at, dir);
					_context.AddCase(title, () => {
						_position = _map.TransformPoint(_position, dir);
					});
				}
			}

			_context.AddBackCaseWith(() => _position = new Point(0, 0));
		}

		void DescribeLocation(Location loc) {
			_out.WriteFormat(Content.here_is, loc.Name);
			if ( loc.Owner != null ) {
				var raceName = Content.ResourceManager.GetString("race_" + loc.Owner.Kind.Id);
				_out.WriteFormat(Content.here_live, loc.Owner.Name, raceName);
			} else {
				_out.Write(Content.here_empty);
			}
		}
	}
}
