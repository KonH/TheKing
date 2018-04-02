namespace TheKing.Settings {
	class MapSettings {
		public int Width  { get; }
		public int Height { get; }

		public double SideSeaChance { get; private set; }
		
		public MapSettings(int width, int height) {
			Width  = width;
			Height = height;
		}

		public MapSettings WithSideSeaChance(double value) {
			SideSeaChance = value;
			return this;
		}
	}
}
