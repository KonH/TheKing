using TheKing.Features.Map;

namespace TheKing.Features.Countries {
	class Race {
		public RaceId       Id           { get; }
		public int          Population   { get; }
		public double       GrowthRate   { get; }
		public double       Power        { get; }
		public double       Speed        { get; }
		public double       TaxRate      { get; }
		public int          SoldierPrice { get; }
		public LocationType StartLoc     { get; }

		public Race(
			RaceId       name, 
			int          population,
			double       growthRate,
			double       power,
			double       speed,
			double       taxRate,
			int          soldierPrice,
			LocationType startLoc
		) {
			Id           = name;
			Population   = population;
			GrowthRate   = growthRate;
			Power        = power;
			Speed        = speed;
			TaxRate      = taxRate;
			SoldierPrice = soldierPrice;
			StartLoc     = startLoc;
		}
	}
}
