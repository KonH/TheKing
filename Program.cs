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

namespace TheKing {
	class Program {
		static void Main(string[] args) {
			var provider = Configure();
			Generate(provider);
			Init(provider);
			Run(provider);
		}

		static ServiceProvider Configure() {
			var services = new ServiceCollection()
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
				.AddSingleton<IContext<ConquestController>, ConquestInterface>()
				.AddSingleton<StartMenuController>();
			return services.BuildServiceProvider();
		}

		static void Generate(IServiceProvider provider) {
			provider.GetService<CountryGenerator>().Generate(5);
			provider.GetService<MapGenerator>().Generate(6, 6);
		}

		static void Init(IServiceProvider provider) {
			provider
				.PerformOneToMany<TimeController, IDayStarter>       ((c, h) => c.OnDayStart += h.OnDayStart)
				.PerformOneToMany<ContextController, IStartHandler>  ((c, h) => c.OnStart += h.OnStart)
				.PerformOneToMany<CountryController, ICountryHandler>((c, h) => c.OnCountryRemoved += h.OnCountryRemoved);
		}

		static void Run(IServiceProvider provider) {
			provider.GetService<StartMenuController>().Run();
			provider.GetService<GameLogics>().Run();
			Console.WriteLine("Press any key...");
			Console.ReadKey();
		}
	}
}
