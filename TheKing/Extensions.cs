﻿using System;
using Microsoft.Extensions.DependencyInjection;
using TheKing.Common;
using TheKing.Interfaces;
using TheKing.Controllers;
using TheKing.Features.Time;
using TheKing.Features.Context;
using TheKing.Features.Countries;
using TheKing.Features.Conquest;
using TheKing.Settings;
using TheKing.Generators;

namespace TheKing {
	static class Extensions {
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

		public static IServiceCollection AddGameLogics(this IServiceCollection services) {
			var gameLogics = SingletonFactory<GameLogics>.Create(provider => {
				return new GameLogics(
					provider.GetService<InputController>(),
					provider.GetService<OutputController>(),
					provider.GetService<ContextController>(),
					provider.GetService<CountryController>(),
					provider.GetService<TimeController>()
				);
			});
			return services
				.AddSingleton(gameLogics)
				.AddSingleton<IDayStarter, GameLogics>(gameLogics)
				.AddSingleton<ICountryHandler, GameLogics>(gameLogics);
		}

		public static IServiceCollection AddArmyInterface(this IServiceCollection services) {
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
			return services
				.AddSingleton<IStartHandler, ArmyInterface>(armyInterface)
				.AddSingleton<IContext<ArmyController>, ArmyInterface>(armyInterface);
		}

		public static IServiceCollection AddConquestInterface(this IServiceCollection services) {
			var conquestInterface = SingletonFactory<ConquestInterface>.Create(provider => {
				return new ConquestInterface(
					provider.GetService<ContextController>(),
					provider.GetService<InputController>(),
					provider.GetService<OutputController>(),
					provider.GetService<CountryController>(),
					provider.GetService<MapController>(),
					provider.GetService<DiscoveryController>(),
					provider.GetService<ArmyController>(),
					provider.GetService<ConquestController>()
				);
			});
			return services
				.AddSingleton<IContext<ConquestController>, ConquestInterface>()
				.AddSingleton<IConquestHandler, ConquestInterface>(conquestInterface);
		}

		public static IServiceCollection AddGameController(this IServiceCollection services) {
			var controller = SingletonFactory<IGameController>.Create(provider => {
				if ( provider.GetService<GameSettings>().WithPlayer ) {
					return new PlayerController(
						provider.GetService<ContextController>(),
						provider.GetService<InputController>(),
						provider.GetService<OutputController>());
				} else {
					return new AutoController();
				}
			});
			return services.AddSingleton(controller);
		}

		public static IServiceProvider PreRun(this IServiceProvider provider) {
			provider.GetService<StartMenuController>().Run();
			return provider;
		}

		public static IServiceProvider Generate(this IServiceProvider provider) {
			provider.GetService<CountryGenerator>().Generate();
			provider.GetService<MapGenerator>    ().Generate();
			return provider;
		}

		public static IServiceProvider Init(this IServiceProvider provider) {
			provider
				.SubscribeFor<TimeController,     IDayStarter>     ((c, h) => c.OnDayStart       += h.OnDayStart)
				.SubscribeFor<ContextController,  IStartHandler>   ((c, h) => c.OnStart          += h.OnStart)
				.SubscribeFor<CountryController,  ICountryHandler> ((c, h) => c.OnCountryRemoved += h.OnCountryRemoved)
				.SubscribeFor<ConquestController, IConquestHandler>((c, h) => c.OnConquest       += h.OnConquest);
			return provider;
		}

		public static void Run(this IServiceProvider provider) {
			var controller = provider.GetService<IGameController>();
			provider.GetService<GameLogics>().Run(controller);
			Console.WriteLine("Press any key...");
			Console.ReadKey();
		}

		static IServiceProvider SubscribeFor<TController, TInterface>(this IServiceProvider provider, Action<TController, TInterface> act) {
			var source = provider.GetService<TController>();
			var destinations = provider.GetServices<TInterface>();
			foreach ( var listener in destinations ) {
				act(source, listener);
			}
			return provider;
		}
	}
}
