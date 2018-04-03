using System;
using System.Collections.Generic;
using System.Diagnostics;
using TheKing.Utils;
using TheKing.Settings;
using TheKing.Features.Map;
using TheKing.Features.Countries;
using System.Linq;

namespace TheKing.Generators {
	class MapGenerator {
		public Dictionary<Point, Location> Locations { get; private set; }

		CountryGenerator _country;
		MapSettings      _settings;

		HashSet<string> _usedNames = new HashSet<string>();

		public MapGenerator(CountryGenerator country, MapSettings settings) {
			_country  = country;
			_settings = settings;
		}

		public void Generate() {
			Locations = new Dictionary<Point, Location>();
			_usedNames.Clear();
			CreateSpace();
			FillLocations();
			AssignCountries();
		}

		void CreateSpace() {
			for ( var x = 0; x < _settings.Width; x++ ) {
				for ( var y = 0; y < _settings.Height; y++ ) {
					Locations.Add(new Point(x, y), null);
				}
			}
		}

		void FillLocations() {
			var points = new List<Point>(Locations.Keys);
			foreach ( var p in points ) {
				var loc = GenerateLocation(p);
				Locations[p] = loc;
				Debug.WriteLine($"MapGenerator: New location: '{loc.Name}' at ({p.X}, {p.Y})");
			}
		}

		bool IsSidePoint(Point p) {
			return 
				(p.X == 0) ||
				(p.Y == 0) ||
				(p.X == _settings.Width - 1) || 
				(p.Y == _settings.Height - 1);
		}
		Location GenerateLocation(Point pos) {
			var type = GetLocationType(pos.Y, IsSidePoint(pos));
			var name = GetName(pos, type);
			var difficulty = GetDifficulty(type);
			var distance = GetDistance(type);
			return new Location(pos, type, name, difficulty, distance);
		}

		string GetName(Point pos, LocationType type) {
			if ( type == LocationType.Sea ) {
				return GenerateNameNeighborsUsed(pos, type);
			}
			return GenerateNameNoRepeats(type);
		}

		string GenerateNameNeighborsUsed(Point pos, LocationType type) {
			var nearLocs = Locations.GetNearLocations(pos);
			foreach ( var loc in nearLocs ) {
				if ( (loc != null) && (loc.Type == type) ) {
					return loc.Name;
				}
			}
			return GenerateNameNoRepeats(type);
		}

		string GenerateNameNoRepeats(LocationType type) {
			var attempts = 0;
			var newName = string.Empty;
			do {
				newName = GenerateNameFromResources(type);
				if ( attempts > 10 ) {
					break;
				}
				attempts++;
			} while ( _usedNames.Contains(newName) );
			_usedNames.Add(newName);
			return newName;
		}

		string GenerateNameFromResources(LocationType type) {
			var prefixes = $"loc_prefixes_{type}";
			var names = $"loc_names_{type}";
			return $"{SelectName(prefixes)} {SelectName(names)}";
		}

		string SelectName(string path) {
			var names = Content.ResourceManager.GetString(path).Split(";");
			return RandUtils.GetItem(names);
		}

		double GetDifficulty(LocationType type) {
			var rand = RandUtils.Rand.NextDouble();
			switch ( type ) {
				case LocationType.Lands    : return 0.10 + rand * 0.10;
				case LocationType.Barrens  : return 0.20 + rand * 0.15;
				case LocationType.Woods    : return 0.30 + rand * 0.20;
				case LocationType.Mountains: return 0.40 + rand * 0.30;
				default: return 0;
			}
		}

		int GetDistance(LocationType type) {
			var rand = RandUtils.Rand.Next(3);
			switch ( type ) {
				case LocationType.Lands    : return 1 + rand;
				case LocationType.Barrens  : return 2 + rand;
				case LocationType.Woods    : return 4 + rand;
				case LocationType.Mountains: return 6 + rand;
				default: return 0;
			}
		}

		LocationType GetLocationType(double y, bool isSide) {
			var allTypes = new List<LocationType>((LocationType[])Enum.GetValues(typeof(LocationType)));
			if ( !isSide ) {
				allTypes.Remove(LocationType.Sea);
			}
			var allChances = new double[allTypes.Count];
			for ( var i = 0; i < allTypes.Count; i++ ) {
				var chanceFunc = _settings.ChanceByYAxis[allTypes[i]];
				var normalizedY = y / _settings.Height;
				allChances[i] = chanceFunc(normalizedY);
			}
			return RandUtils.GetItemWithChances(allTypes, allChances);
		}

		void AssignCountries() {
			var availableLocs = new List<Location>();
			foreach ( var loc in Locations.Values ) {
				if ( loc.Reachable ) {
					availableLocs.Add(loc);
				}
			}

			for ( var i = 0; i < _country.Countries.Count; i++ ) {
				var country = _country.Countries[i];
				var location = SelectLocation(availableLocs, country);
				location.Owner = country;
				availableLocs.Remove(location);
				Debug.WriteLine(
					$"MapGenerator: Assign {_country.Countries[i]} to location: " +
					$"{location.Name} ({location.Point.X}, {location.Point.Y})"
				);
			}
		}

		Location SelectLocation(List<Location> availableLocs, Country country) {
			var raceLocs = availableLocs.Where(l => l.Type == country.Kind.StartLoc);
			var selectableLocs = raceLocs.Any() ? raceLocs.ToList() : availableLocs;
			return RandUtils.GetItem(selectableLocs);
		}
	}
}
