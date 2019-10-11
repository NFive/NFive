using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using CitizenFX.Core;
using JetBrains.Annotations;
using NFive.Client.Commands;
using NFive.Client.Communications;
using NFive.Client.Diagnostics;
using NFive.Client.Events;
using NFive.Client.Rpc;
using NFive.SDK.Client;
using NFive.SDK.Client.Configuration;
using NFive.SDK.Client.Input;
using NFive.SDK.Client.Interface;
using NFive.SDK.Client.Services;
using NFive.SDK.Core.Communications;
using NFive.SDK.Core.Diagnostics;
using NFive.SDK.Core.Models.Player;
using NFive.SDK.Core.Plugins;

namespace NFive.Client
{
	[UsedImplicitly]
	public class Program : BaseScript
	{
		private readonly Logger logger = new Logger();
		private readonly List<Service> services = new List<Service>();

		/// <summary>
		/// Primary client entry point.
		/// Initializes a new instance of the <see cref="Program" /> class.
		/// </summary>
		public Program()
		{
			Startup();
		}

		private async void Startup()
		{
			this.logger.Info($"NFive {typeof(Program).Assembly.GetCustomAttributes<AssemblyInformationalVersionAttribute>().First().InformationalVersion}");

			// Setup RPC handlers
			RpcManager.Configure(this.EventHandlers);

			var ticks = new TickManager(c => this.Tick += c, c => this.Tick -= c);
			var events = new EventManager();
			var comms = new CommunicationManager(events);
			var commands = new CommandManager();
			var nui = new NuiManager(this.EventHandlers);

			this.logger.Warn("Request config...");

			// Initial connection
			var config = await comms.Event(NFiveCoreEvents.ClientInitialize).ToServer().Request<User, LogLevel, LogLevel>(typeof(Program).Assembly.GetName().Version.ToString());

			this.logger.Warn($"Got config: {config.Item1.Name}");

			//RpcManager.OnRaw("onClientResourceStart", new Action<Player, string>(OnClientResourceStartRaw));
			//RpcManager.OnRaw("onClientResourceStop", new Action<Player, string>(OnClientResourceStopRaw));
			//RpcManager.OnRaw("gameEventTriggered", new Action<Player, string, List<dynamic>>(OnGameEventTriggeredRaw));
			// RpcManager.OnRaw(FiveMClientEvents.PopulationPedCreating, new Action<float, float, float, uint, IPopulationPedCreatingSetter>(OnPopulationPedCreatingRaw));

			ClientConfiguration.ConsoleLogLevel = config.Item2;
			ClientConfiguration.MirrorLogLevel = config.Item3;

			// Load user key mappings
			Input.UserMappings.AddRange(Enum.GetValues(typeof(Control)).OfType<Control>().Select(c => new Hotkey(c)));

			var plugins = await comms.Event(NFiveCoreEvents.ClientPlugins).ToServer().Request<List<Plugin>>();

			foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
			{
				if (assembly.GetCustomAttribute<ClientPluginAttribute>() == null) continue;

				var plugin = plugins.FirstOrDefault(p => p.Client?.Main?.FirstOrDefault(m => m + ".net" == assembly.GetName().Name) != null);

				if (plugin == null)
				{
					this.logger.Debug("Skipping " + assembly.GetName().Name);
					continue;
				}

				this.logger.Info(plugin.FullName);
				this.logger.Info($"\t{assembly.GetName().Name}");

				foreach (var type in assembly.GetTypes().Where(t => !t.IsAbstract && t.IsSubclassOf(typeof(Service))))
				{
					this.logger.Info($"\t\t{type.FullName}");

					var service = (Service)Activator.CreateInstance(type, new Logger($"Plugin|{type.Name}"), ticks, comms, commands, new OverlayManager(plugin.Name, nui), config.Item1);
					await service.Loaded();

					this.services.Add(service);
				}
			}

			// Forward raw FiveM events
			//this.EventHandlers.Add("gameEventTriggered", new Action<string, List<object>>((s, a) => events.Emit("gameEventTriggered", s, a)));
			//this.EventHandlers.Add("populationPedCreating", new Action<float, float, float, uint, object>((x, y, z, model, setters) => events.Emit("populationPedCreating", new PedSpawnOptions(x, y, z, model, setters))));

			this.logger.Info("Plugins loaded");

			await Task.WhenAll(this.services.Select(s => s.Started()));

			this.logger.Info("Plugins started");

			comms.Event(NFiveCoreEvents.ClientInitialized).ToServer().Emit();

			foreach (var service in this.services) await service.HoldFocus();
		}
	}
}
