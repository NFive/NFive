using CitizenFX.Core;
using CitizenFX.Core.Native;
using JetBrains.Annotations;
using NFive.Client.Commands;
using NFive.Client.Diagnostics;
using NFive.Client.Events;
using NFive.Client.Rpc;
using NFive.SDK.Client;
using NFive.SDK.Client.Configuration;
using NFive.SDK.Client.Interface;
using NFive.SDK.Client.Services;
using NFive.SDK.Core.Diagnostics;
using NFive.SDK.Core.Models.Player;
using NFive.SDK.Core.Plugins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace NFive.Client
{
	[UsedImplicitly]
	public class Program : BaseScript
	{
		private readonly Logger logger = new Logger();
		private readonly List<Service> services = new List<Service>();

		/// <summary>
		/// Primary client entry point.
		/// Initializes a new instance of the <see cref="Program"/> class.
		/// </summary>
		public Program() => Startup();

		private async void Startup()
		{
			this.logger.Info($"NFive {typeof(Program).Assembly.GetCustomAttributes<AssemblyInformationalVersionAttribute>().First().InformationalVersion}");

			// Setup RPC handlers
			RpcManager.Configure(this.EventHandlers);
			var rpc = new RpcHandler();

			var ticks = new TickManager(c => this.Tick += c, c => this.Tick -= c);
			var events = new EventManager();
			var commands = new CommandManager(rpc);
			var nui = new NuiManager(this.EventHandlers);

			// Initial connection
			//var configuration = await rpc.Event(SDK.Core.Rpc.RpcEvents.ClientInitialize).Request<ClientConfiguration>(typeof(Program).Assembly.GetName().Version);
			var config = await rpc.Event(SDK.Core.Rpc.RpcEvents.ClientInitialize).Request<User, LogLevel, LogLevel>(typeof(Program).Assembly.GetName().Version.ToString());

			ClientConfiguration.ConsoleLogLevel = config.Item2;
			ClientConfiguration.MirrorLogLevel = config.Item3;



			foreach (Control control in Enum.GetValues(typeof(Control)))
			{
				var hotkey = new Hotkey(control);

				this.logger.Warn($"{{ Control.{ Enum.GetName(typeof(Control), control)}, JavaScriptCode.{hotkey} }},");
			}

			//// Load key mapping
			//this.logger.Warn($"Reading control values");
			//var controlType = typeof(Control);
			//var keyType = typeof(JavaScriptCode);
			//var clientControls = new Dictionary<Control, JavaScriptCode>(KeyMapping.ControlMappings);
			//foreach (var control in KeyMapping.ControlMappings.Keys)
			//{
			//	var controlMapping = API.GetControlInstructionalButton(0, (int)control, 0);
			//	var keyMapping = KeyMapping.KeyMappings[controlMapping];
			//	this.logger.Warn($"Control: {control} | Name: {Enum.GetName(controlType, control)} | ControlMapping: {controlMapping} | KeyMapping {Enum.GetName(keyType, keyMapping)}");
			//	clientControls[control] = keyMapping;
			//}
			//KeyMapping.ControlMappings = clientControls;



			var plugins = await rpc.Event(SDK.Core.Rpc.RpcEvents.ClientPlugins).Request<List<Plugin>>();

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

					var service = (Service)Activator.CreateInstance(type, new Logger($"Plugin|{type.Name}"), ticks, events, rpc, commands, new OverlayManager(plugin.Name, nui), config.Item1);
					await service.Loaded();

					this.services.Add(service);
				}
			}

			// Forward raw FiveM events
			this.EventHandlers.Add("gameEventTriggered", new Action<string, List<object>>((s, a) => events.Raise("gameEventTriggered", s, a)));
			this.EventHandlers.Add("populationPedCreating", new Action<float, float, float, uint, object>((x, y, z, model, setters) => events.Raise("populationPedCreating", new PedSpawnOptions(x, y, z, model, setters))));

			this.logger.Info("Plugins loaded");

#pragma warning disable 4014
			foreach (var service in this.services) service.Started();
#pragma warning restore 4014

			this.logger.Info("Plugins started");

			rpc.Event(SDK.Core.Rpc.RpcEvents.ClientInitialized).Trigger();

			foreach (var service in this.services)
				await service.HoldFocus();
		}
	}
}
