using System;
using Microsoft.Extensions.DependencyInjection;
using TheKing.Common;
using TheKing.Handlers;
using TheKing.Interfaces;
using TheKing.Controllers;
using TheKing.Features.Time;
using TheKing.Features.Context;
using TheKing.Features.Countries;

namespace TheKing {
	class Program {
		static void Main(string[] args) {
			var provider = Configure();
			Init(provider);
			Run(provider);
		}

		static class SingletonFactory<T> where T : class {
			static T _instance;

			public static Func<IServiceProvider, T> Create(Func<IServiceProvider, T> create) {
				return provider => {
					if ( _instance == null ) {
						_instance = create(provider);
					}
					return _instance;
				};
			}
		}

		static ServiceProvider Configure() {
			var gameLogics = SingletonFactory<GameLogics>.Create(provider => {
				return new GameLogics(
					provider.GetService<InputController>(),
					provider.GetService<OutputController>(),
					provider.GetService<ContextController>(),
					provider.GetService<CountryController>(),
					provider.GetService<TimeController>()
				);
			});

			var armyInterface = SingletonFactory<ArmyInterface>.Create(provider => {
				return new ArmyInterface(
					provider.GetService<CountryController>(),
					provider.GetService<PopulationController>(),
					provider.GetService<ArmyController>(),
					provider.GetService<InputController>(),
					provider.GetService<OutputController>(),
					provider.GetService<ContextController>()
				);
			});

			var bot = SingletonFactory<BotController>.Create(provider => {
				return new BotController(
					provider.GetService<TimeController>(),
					provider.GetService<CountryController>(),
					provider.GetService<MoneyController>(),
					provider.GetService<ArmyController>()
					);
			});

			var services = new ServiceCollection()
				.AddSingleton(gameLogics)
				.AddSingleton<IDayStarter, GameLogics>(gameLogics)
				.AddSingleton<ICountryHandler, GameLogics>(gameLogics)
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
				.AddSingleton<IStartHandler, ArmyInterface>(armyInterface)
				.AddSingleton<IContext<ArmyController>, ArmyInterface>(armyInterface)
				.AddSingleton<IStartHandler, SleepInterface>()
				.AddSingleton<IContext<ConquestController>, ConquestInterface>();
			return services.BuildServiceProvider();
		}

		static void Init(IServiceProvider provider) {
			var time = provider.GetService<TimeController>();
			var updaters = provider.GetServices<IDayStarter>();
			foreach ( var updater in updaters ) {
				time.OnDayStart += updater.OnDayStart;
			}
			var context = provider.GetService<ContextController>();
			var starters = provider.GetServices<IStartHandler>();
			foreach ( var starter in starters ) {
				context.OnStart += starter.OnStart;
			}
			var country = provider.GetService<CountryController>();
			var countryHandlers = provider.GetServices<ICountryHandler>();
			foreach ( var handler in countryHandlers ) {
				country.OnCountryRemoved += handler.OnCountryRemoved;
			}
		}

		static void Run(IServiceProvider provider) {
			provider.GetService<GameLogics>().Run();
			Console.WriteLine("Press any key...");
			Console.ReadKey();
		}
	}
}
