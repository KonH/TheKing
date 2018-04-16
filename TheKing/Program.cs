using System;
using Microsoft.Extensions.DependencyInjection;
using TheKing.Common;
using TheKing.Handlers;
using TheKing.Generators;
using TheKing.Interfaces;
using TheKing.Controllers;
using TheKing.Features.Time;
using TheKing.Features.Context;
using TheKing.Features.Countries;
using TheKing.Settings;
using TheKing.Features.Map;
using TheKing.Features.Conquest;

namespace TheKing {
	class Program {
		static void Main(string[] args) {
			var mapSettings = new MapSettings(6, 6)
				.WithChance(LocationType.Sea,       y => y * 0.8)
				.WithChance(LocationType.Lands,     y => Math.Max(Math.Sin((y - 0.3) * Math.PI * 2), 0.0))
				.WithChance(LocationType.Woods,     y => Math.Max(Math.Sin((y - 0.1) * Math.PI * 2), 0.0))
				.WithChance(LocationType.Mountains, y => (0.75 - y))
				.WithChance(LocationType.Barrens,   y => y * 0.75);

			var raceSettings = new RaceSettings()
				// population, growthRate, power, speed, taxRate, soldierPrice, loc
				.With(new Race(RaceId.Human,    100, 0.020, 1.10, 1.00, 0.25,  5, LocationType.Lands))
				.With(new Race(RaceId.Dwarf,     50, 0.015, 1.85, 0.50, 0.75, 10, LocationType.Mountains))
				.With(new Race(RaceId.Elf,       50, 0.019, 1.60, 1.25, 0.50,  7, LocationType.Woods))
				.With(new Race(RaceId.Goblin,   200, 0.025, 0.33, 2.00, 0.10,  3, LocationType.Barrens))
				.With(new Race(RaceId.Halfling, 100, 0.024, 0.43, 0.75, 0.50,  4, LocationType.Lands))
				.With(new Race(RaceId.Orc,      150, 0.023, 0.76, 1.50, 0.15,  4, LocationType.Barrens));

			var provider = Configure(mapSettings, raceSettings).PreRun().Generate().Init();

			provider.Run();
		}

		static ServiceProvider Configure(MapSettings mapSettings, RaceSettings raceSettings) {
			var services = new ServiceCollection()
				.AddSingleton(mapSettings)
				.AddSingleton(raceSettings)
				.AddSingleton<CountryGenerator>()
				.AddSingleton<MapGenerator>()
				.AddGameLogics()
				.AddSingleton<CheatController>()
				.AddSingleton<InputController>()
				.AddSingleton<OutputController>()
				.AddSingleton<ContextController>()
				.AddSingleton<TimeController>()
				.AddSingleton<CountryController>()
				.AddSingleton<MapController>()
				.AddSingleton<MoneyController>()
				.AddSingleton<PopulationController>()
				.AddSingleton<ArmyController>()
				.AddSingleton<ConquestController>()
				.AddSingleton<DiscoveryController>()
				.AddSingleton<MoveController>()
				.AddSingleton<BotController>()
				.AddSingleton<IDayStarter, ArmyUpdater>()
				.AddSingleton<IDayStarter, PopulationUpdater>()
				.AddSingleton<IDayStarter, BotController>()
				.AddSingleton<IStartHandler, MapInterface>()
				.AddSingleton<IStartHandler, MoneyInteface>()
				.AddArmyInterface()
				.AddSingleton<IStartHandler, SleepInterface>()
				.AddSingleton<IStartHandler, FreeModeInterface>()
				.AddConquestInterface()
				.AddSingleton<GameSettings>()
				.AddSingleton<StartMenuController>()
				.AddGameController();
			return services.BuildServiceProvider();
		}
	}
}
