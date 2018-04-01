namespace TheKing.Features.Map {
	struct Point {
		public int X { get; }
		public int Y { get; }

		public Point(int x, int y) {
			X = x;
			Y = y;
		}

		public bool IsEqual(Point other) {
			return (X == other.X) && (Y == other.Y);
		}
	}
}
