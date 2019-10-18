using System;
using System.Collections.Generic;
using System.Linq;
using CitizenFX.Core.Native;
using NFive.SDK.Core.Arguments;
using NFive.SDK.Core.Diagnostics;
using NFive.SDK.Core.Plugins;
using NFive.SDK.Plugins.Configuration;
using NFive.SDK.Server.Communications;
using NFive.SDK.Server.Controllers;
using NFive.SDK.Server.Events;
using NFive.SDK.Server.Rcon;
using NFive.Server.Diagnostics;

namespace NFive.Server.Rcon
{
	public class RconManager : IRconManager
	{
		private readonly Dictionary<string, Delegate> callbacks = new Dictionary<string, Delegate>();

		public Dictionary<Name, List<Controller>> Controllers { get; set; } = new Dictionary<Name, List<Controller>>();

		public RconManager(ICommunicationManager comms)
		{
			comms.Event(ServerEvents.RconCommand).FromServer().On<string, string[]>(OnCommand);
		}

		public void Register<T>(string command, Action<T> callback)
		{
			this.callbacks.Add(command.ToLowerInvariant(), new Action<IEnumerable<string>>(a =>
			{
				callback(Argument.Parse<T>(a));
			}));
		}

		public void Register(string command, Action callback)
		{
			this.callbacks.Add(command.ToLowerInvariant(), callback);
		}

		private void OnCommand(ICommunicationMessage e, string command, string[] objArgs)
		{
			new Logger(LogLevel.Trace, "Rcon").Debug($"{command} {string.Join(" ", objArgs)}");

			if (this.callbacks.ContainsKey(command.ToLowerInvariant()))
			{
				this.callbacks[command].DynamicInvoke(objArgs.Cast<object>().ToArray());
				API.CancelEvent();
				return;
			}

			if (command.ToLowerInvariant() != "reload") return;

			try
			{
				var args = objArgs.Select(a => new Name(a)).ToList();
				if (args.Count == 0) args = this.Controllers.Keys.ToList();

				foreach (var pluginName in args)
				{
					if (!this.Controllers.ContainsKey(pluginName)) continue;

					foreach (var controller in this.Controllers[pluginName])
					{
						var controllerType = controller.GetType();

						if (controllerType.BaseType != null && controllerType.BaseType.IsGenericType && controllerType.BaseType.GetGenericTypeDefinition() == typeof(ConfigurableController<>))
						{
							controllerType.GetMethods().FirstOrDefault(m => m.DeclaringType == controllerType && m.Name == "Reload")?.Invoke(controller, new[] { ConfigurationManager.InitializeConfig(pluginName, controllerType.BaseType.GetGenericArguments()[0]) });
						}
						else
						{
							controller.Reload();
						}
					}
				}
			}
			catch (Exception)
			{
				// TODO
			}
			finally
			{
				API.CancelEvent();
			}
		}
	}
}
