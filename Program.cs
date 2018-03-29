using System;
using Microsoft.Extensions.DependencyInjection;

namespace TheKing.New {
	class Program {
		static void Main(string[] args) {
			var provider = Configure();
			Run(provider);
		}

		static ServiceProvider Configure() {
			var services = new ServiceCollection()
				.AddSingleton<TimeController>()
				.AddSingleton<CountryController>()
				.AddSingleton<MapController>()
				.AddSingleton<MoneyController>()
				.AddSingleton<PopulationController>()
				.AddSingleton<ArmyController>()
				.AddSingleton<ConquestController>()
				.AddSingleton<ArmyUpdater>()
				.AddSingleton<PopulationUpdater>()
				.AddSingleton<GameState>();
			return services.BuildServiceProvider();
		}

		static void Run(ServiceProvider provider) {
			var armyUpdater = provider.GetService<ArmyUpdater>();
			var gameState = provider.GetService<GameState>();
			gameState.Run(() => {
				var countryCtrl = provider.GetService<CountryController>();
				var armyCtrl = provider.GetService<ArmyController>();
				armyCtrl.Recruit(countryCtrl.PlayerCountry, 1);

			});

			Console.WriteLine("Press any key...");
			Console.ReadKey();
		}
	}
}
