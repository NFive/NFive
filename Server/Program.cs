using Autofac;
using CitizenFX.Core;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using NFive.SDK.Core.Events;
using NFive.SDK.Core.Plugins;
using NFive.SDK.Plugins;
using NFive.SDK.Plugins.Configuration;
using NFive.SDK.Server;
using NFive.SDK.Server.Configuration;
using NFive.SDK.Server.Controllers;
using NFive.SDK.Server.Events;
using NFive.Server.Communications;
using NFive.Server.Configuration;
using NFive.Server.Controllers;
using NFive.Server.Diagnostics;
using NFive.Server.IoC;
using NFive.Server.Rcon;
using NFive.Server.Rpc;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using static CitizenFX.Core.Native.API;

namespace NFive.Server
{
	[UsedImplicitly]
	public class Program : ServerScript
	{
		private readonly Dictionary<Name, List<Controller>> controllers = new Dictionary<Name, List<Controller>>();

		public Program()
		{
			// Print exception messages in English
			Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture;
			CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.InvariantCulture;

			// Set the AppDomain working directory to the current resource root
			Environment.CurrentDirectory = Path.GetFullPath(GetResourcePath(GetCurrentResourceName()));

			this.Tick += OnFirstTick;
		}

		private async Task OnFirstTick()
		{
			this.Tick -= OnFirstTick;

			await Startup();
		}

		private async Task Startup()
		{
			new Logger().Info($"NFive {typeof(Program).Assembly.GetCustomAttributes<AssemblyInformationalVersionAttribute>().First().InformationalVersion}");

			// TODO: Check and warn if local CitizenFX.Core.Server.dll is found

			var config = ConfigurationManager.Load<CoreConfiguration>("core.yml");
			var databaseConfig = ConfigurationManager.Load<DatabaseConfiguration>("database.yml");

			// Use configured culture for output
			Thread.CurrentThread.CurrentCulture = config.Locale.Culture.First();
			CultureInfo.DefaultThreadCurrentCulture = config.Locale.Culture.First();

			ServerConfiguration.Locale = config.Locale;
			ServerLogConfiguration.Output = config.Log.Output;

			SetGameType(config.Display.Game);
			SetMapName(config.Display.Map);

			var logger = new Logger(config.Log.Core);

			var graph = DefinitionGraph.Load();

			var builder = new ContainerBuilder();

			builder.Register(c => this.Players).As<PlayerList>().SingleInstance();

			builder.Register(c => new BaseScriptProxy(this.EventHandlers, this.Exports, c.Resolve<PlayerList>())).As<IBaseScriptProxy>().SingleInstance();

			builder.RegisterModule(new CoreModule(config, databaseConfig));

			builder.RegisterModule(new PluginModule(graph.Plugins, databaseConfig));

			var container = builder.Build();

			using (var scope = container.BeginLifetimeScope())
			{
				RpcManager.Configure(config.Log.Comms, this.EventHandlers, scope.Resolve<PlayerList>());

				// Load core controllers
				try
				{
					var dbController = scope.Resolve<DatabaseController>();
					await dbController.Loaded();

					this.controllers.Add(new Name("NFive/Database"), new List<Controller>
					{
						dbController
					});
				}
				catch (Exception ex)
				{
					logger.Error(ex, "Database connection error");
					logger.Warn("Fatal error, exiting");
					Environment.Exit(1);
				}

				var eventController = scope.Resolve<EventController>();
				await eventController.Loaded();

				this.controllers.Add(new Name("NFive/RawEvents"), new List<Controller>
				{
					eventController
				});

				var sessionController = scope.Resolve<SessionController>();
				await sessionController.Loaded();

				this.controllers.Add(new Name("NFive/Session"), new List<Controller>
				{
					sessionController
				});

				// Load plugins
				foreach (var plugin in graph.Plugins)
				{
					var dbContexts = scope.ResolveNamed<IEnumerable<DbContext>>(plugin.Name);

					foreach (var ctx in dbContexts)
					{
						var migrator = ctx.Database;

						if (!(await migrator.GetPendingMigrationsAsync()).Any()) continue;

						if (!ServerConfiguration.AutomaticMigrations) throw new Exception($"Plugin {plugin.FullName} has pending migrations but automatic migrations are disabled");

						foreach (var migration in await migrator.GetPendingMigrationsAsync())
						{
							new Logger(config.Log.Core, "Database").Debug($"[{plugin.FullName}] Running migration: {migration}");

							await migrator.MigrateAsync();
						}
					}

					var controllers = scope.ResolveNamed<IEnumerable<Controller>>(plugin.Name);

					if (!controllers.Any()) continue;

					foreach (var controller in controllers) await controller.Loaded();

					this.controllers.Add(plugin.Name, controllers.ToList());

					logger.Trace($"Adding {controllers.Count()} controllers from plugin {plugin.Name}");
				}

				await Task.WhenAll(this.controllers.SelectMany(c => c.Value).Select(s => s.Started()).ToArray());

				var rcon = scope.Resolve<RconManager>();

				var comms = scope.Resolve<CommunicationManager>();

				rcon.Controllers = this.controllers;

				comms.Event(CoreEvents.ClientPlugins).FromClients().OnRequest(e => e.Reply(graph.Plugins));

				logger.Debug($"{graph.Plugins.Count.ToString(CultureInfo.InvariantCulture)} plugin(s) loaded, {this.controllers.Count.ToString(CultureInfo.InvariantCulture)} controller(s) created");

				comms.Event(ServerEvents.ServerInitialized).ToServer().Emit();

				logger.Info("Server ready");
			}
		}
	}
}
