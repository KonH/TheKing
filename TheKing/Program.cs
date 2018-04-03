﻿using System;
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
				.With(new Race(RaceId.Human,    100, 0.02, 1.00, 1.00, 0.25,  5, LocationType.Lands))
				.With(new Race(RaceId.Dwarf,     50, 0.01, 1.75, 0.50, 0.75, 10, LocationType.Mountains))
				.With(new Race(RaceId.Elf,       50, 0.01, 1.50, 1.25, 0.50,  7, LocationType.Woods))
				.With(new Race(RaceId.Goblin,   200, 0.04, 0.33, 2.00, 0.10,  2, LocationType.Barrens))
				.With(new Race(RaceId.Halfling, 100, 0.02, 0.33, 0.75, 0.50,  4, LocationType.Lands))
				.With(new Race(RaceId.Orc,      150, 0.03, 0.66, 1.50, 0.15,  3, LocationType.Barrens));

			var provider = Configure(mapSettings, raceSettings);
			var startMenu = provider.GetService<StartMenuController>();
			PreRun(startMenu);
			Generate(provider, startMenu.WithPlayer);
			Init(provider);
			Run(provider);
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
				.AddSingleton<StartMenuController>();
			return services.BuildServiceProvider();
		}

		static void PreRun(StartMenuController startMenu) {
			startMenu.Run();
		}

		static void Generate(IServiceProvider provider, bool withPlayer) {
			provider.GetService<CountryGenerator>().Generate(withPlayer, 5);
			provider.GetService<MapGenerator>().Generate();
		}

		static void Init(IServiceProvider provider) {
			provider
				.PerformOneToMany<TimeController, IDayStarter>         ((c, h) => c.OnDayStart += h.OnDayStart)
				.PerformOneToMany<ContextController, IStartHandler>    ((c, h) => c.OnStart += h.OnStart)
				.PerformOneToMany<CountryController, ICountryHandler>  ((c, h) => c.OnCountryRemoved += h.OnCountryRemoved)
				.PerformOneToMany<ConquestController, IConquestHandler>((c, h) => c.OnConquest += h.OnConquest);
		}

		static void Run(IServiceProvider provider) {
			provider.GetService<GameLogics>().Run();
			Console.WriteLine("Press any key...");
			Console.ReadKey();
		}
	}
}
