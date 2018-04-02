using System.Collections.Generic;
using TheKing.Features.Map;

namespace TheKing.Utils {
	static class MapUtils {
		static Direction[] AllDirections = {
			Direction.North,
			Direction.East,
			Direction.South,
			Direction.West,
		};

		public static Direction[] GetDirections() {
			return AllDirections;
		}

		public static Location GetLocationAt(this Dictionary<Point, Location> container, Point pos) {
			if ( container.TryGetValue(pos, out var loc) ) {
				return loc;
			}
			return null;
		}

		public static Location GetLocationAt(this Dictionary<Point, Location> container, Point pos, Direction dir) {
			return GetLocationAt(container, TransformPoint(pos, dir));
		}

		public static Point TransformPoint(Point p, Direction dir) {
			switch ( dir ) {
				case Direction.North: return new Point(p.X, p.Y - 1);
				case Direction.East: return new Point(p.X + 1, p.Y);
				case Direction.South: return new Point(p.X, p.Y + 1);
				case Direction.West: return new Point(p.X - 1, p.Y);
			}
			return p;
		}

		public static List<Location> GetNearLocations(this Dictionary<Point, Location> container, Point point) {
			var result = new List<Location>();
			foreach ( var dir in AllDirections ) {
				var locationAt = GetLocationAt(container, point, dir);
				if ( locationAt != null ) {
					result.Add(locationAt);
				}
			}
			return result;
		}
	}
}
