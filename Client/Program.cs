using CitizenFX.Core;
using JetBrains.Annotations;
using NFive.Client.Diagnostics;
using NFive.Client.Events;
using NFive.Client.Rpc;
using NFive.SDK.Client;
using NFive.SDK.Client.Interface;
using NFive.SDK.Client.Services;
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
		/// Primary client entrypoint.
		/// Initializes a new instance of the <see cref="Program"/> class.
		/// </summary>
		public Program() => Startup();

		private async void Startup()
		{
			// Setup RPC handlers
			RpcManager.Configure(this.EventHandlers);

			var ticks = new TickManager(c => this.Tick += c, c => this.Tick -= c);
			var events = new EventManager();
			var rpc = new RpcHandler();
			var nui = new NuiManager(this.EventHandlers);

			var user = await rpc.Event(SDK.Core.Rpc.RpcEvents.ClientInitialize).Request<User>("1.0.0");
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

					var service = (Service)Activator.CreateInstance(type, new Logger($"Plugin|{type.Name}"), ticks, events, rpc, new OverlayManager(plugin.Name, nui), user);
					await service.Loaded();

					this.services.Add(service);
				}
			}

			this.logger.Info("Plugins loaded");

#pragma warning disable 4014
			foreach (var service in this.services) service.Started();
#pragma warning restore 4014

			this.logger.Info("Plugins started");

			rpc.Event(SDK.Core.Rpc.RpcEvents.ClientInitialized).Trigger();
		}
	}
}
