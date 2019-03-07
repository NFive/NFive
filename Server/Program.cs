using CitizenFX.Core;
using CitizenFX.Core.Native;
using JetBrains.Annotations;
using NFive.SDK.Core.Diagnostics;
using NFive.SDK.Core.Plugins;
using NFive.SDK.Plugins;
using NFive.SDK.Plugins.Configuration;
using NFive.SDK.Server.Configuration;
using NFive.SDK.Server.Controllers;
using NFive.SDK.Server.Migrations;
using NFive.Server.Configuration;
using NFive.Server.Controllers;
using NFive.Server.Diagnostics;
using NFive.Server.Events;
using NFive.Server.Rpc;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Data.Entity.Migrations.Infrastructure;
using System.IO;
using System.Linq;
using System.Reflection;
using NFive.Server.Rcon;

namespace NFive.Server
{
	[UsedImplicitly]
	public class Program : BaseScript
	{
		private readonly Dictionary<Name, List<Controller>> controllers = new Dictionary<Name, List<Controller>>();

		public Program()
		{
			// Set the AppDomain working directory to the current resource root
			Environment.CurrentDirectory = FileManager.ResolveResourcePath();

			var config = ConfigurationManager.Load<CoreConfiguration>("nfive.yml");

			var logger = new Logger(config.Log.Core);

			//ServerConfiguration.LogLevel = config.Log.Level;
			API.SetGameType(config.Display.Game);
			API.SetMapName(config.Display.Map);

			// Setup RPC handlers
			RpcManager.Configure(config.Log.Rpc, this.EventHandlers);

			var events = new EventManager(config.Log.Events);
			var rcon = new RconManager(new RpcHandler());

			// Load core controllers
			this.controllers.Add(new Name("NFive/Server"), new List<Controller>());

			var dbController = new DatabaseController(new Logger(config.Log.Core, "Database"), events, new RpcHandler(), rcon, ConfigurationManager.Load<DatabaseConfiguration>("database.yml"));
			this.controllers[new Name("NFive/Server")].Add(dbController);

			// Resolve dependencies
			var graph = DefinitionGraph.Load();

			var pluginDefaultLogLevel = config.Log.Plugins.ContainsKey("default") ? config.Log.Plugins["default"] : LogLevel.Info;

			// Load plugins into the AppDomain
			foreach (var plugin in graph.Plugins)
			{
				logger.Info($"Loading {plugin.FullName}");

				// Load include files
				foreach (var includeName in plugin.Server?.Include ?? new List<string>())
				{
					var includeFile = Path.Combine("plugins", plugin.Name.Vendor, plugin.Name.Project, $"{includeName}.net.dll");
					if (!File.Exists(includeFile)) throw new FileNotFoundException(includeFile);

					AppDomain.CurrentDomain.Load(File.ReadAllBytes(includeFile));
				}

				// Load main files
				foreach (var mainName in plugin.Server?.Main ?? new List<string>())
				{
					var mainFile = Path.Combine("plugins", plugin.Name.Vendor, plugin.Name.Project, $"{mainName}.net.dll");
					if (!File.Exists(mainFile)) throw new FileNotFoundException(mainFile);

					var types = Assembly.LoadFrom(mainFile).GetTypes().Where(t => !t.IsAbstract && t.IsClass).ToList();

					//logger.Debug($"{mainName}: {types.Count} {string.Join(Environment.NewLine, types)}");

					// Find migrations
					foreach (var migrationType in types.Where(t => t.BaseType != null && t.BaseType.IsGenericType && t.BaseType.GetGenericTypeDefinition() == typeof(MigrationConfiguration<>)))
					{
						var configuration = (DbMigrationsConfiguration)Activator.CreateInstance(migrationType);
						var migrator = new DbMigrator(configuration);

						if (!migrator.GetPendingMigrations().Any()) continue;

						if (!ServerConfiguration.AutomaticMigrations) throw new MigrationsPendingException($"Plugin {plugin.FullName} has pending migrations but automatic migrations are disabled");

						logger.Debug($"{mainName}: Running migrations {string.Join(", ", migrator.GetPendingMigrations())}");

						migrator.Update();
					}

					// Find controllers
					foreach (var controllerType in types.Where(t => t.IsSubclassOf(typeof(Controller)) || t.IsSubclassOf(typeof(ConfigurableController<>))))
					{
						var logLevel = config.Log.Plugins.ContainsKey(plugin.Name) ? config.Log.Plugins[plugin.Name] : pluginDefaultLogLevel;

						var constructorArgs = new List<object>
						{
							new Logger(logLevel, plugin.Name),
							events,
							new RpcHandler(),
							rcon
						};

						// Check if controller is configurable
						if (controllerType.BaseType != null && controllerType.BaseType.IsGenericType && controllerType.BaseType.GetGenericTypeDefinition() == typeof(ConfigurableController<>))
						{
							// Initialize the controller configuration
							constructorArgs.Add(ConfigurationManager.InitializeConfig(plugin.Name, controllerType.BaseType.GetGenericArguments()[0]));
						}

						// Construct controller instance
						var controller = (Controller)Activator.CreateInstance(controllerType, constructorArgs.ToArray());

						if (!this.controllers.ContainsKey(plugin.Name)) this.controllers.Add(plugin.Name, new List<Controller>());
						this.controllers[plugin.Name].Add(controller);
					}
				}
			}

			rcon.Controllers = this.controllers;

			new RpcHandler().Event(SDK.Core.Rpc.RpcEvents.ClientPlugins).On(e => e.Reply(graph.Plugins));

			events.Raise(SDK.Server.Events.ServerEvents.ServerInitialized);

			logger.Debug($"{graph.Plugins.Count} plugin(s) loaded, {this.controllers.Count} controller(s) created");
		}
	}
}
