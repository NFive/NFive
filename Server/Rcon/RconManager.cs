using System;
using System.Collections.Generic;
using System.Linq;
using CitizenFX.Core.Native;
using NFive.SDK.Core.Arguments;
using NFive.SDK.Core.Plugins;
using NFive.SDK.Plugins.Configuration;
using NFive.SDK.Server.Controllers;
using NFive.SDK.Server.Rcon;
using NFive.SDK.Server.Rpc;

namespace NFive.Server.Rcon
{
	public class RconManager : IRconManager
	{
		public class Subscription
		{
			private readonly Delegate handler;

			public Subscription(Delegate handler)
			{
				this.handler = handler;
			}

			public void Handle(IEnumerable<string> args) => this.handler.DynamicInvoke(args);
		}

		public Dictionary<Name, List<Controller>> Controllers { get; set; } = new Dictionary<Name, List<Controller>>();
		public Dictionary<string, Subscription> Callbacks { get; set; } = new Dictionary<string, Subscription>();

		public RconManager(IRpcHandler rpc)
		{
			rpc.Event("rconCommand").OnRaw(new Action<string, List<object>>(Handle));
		}

		public void Register<T>(string command, Action<T> callback)
		{
			this.Callbacks.Add(command.ToLowerInvariant(), new Subscription(new Action<IEnumerable<string>>(a =>
			{
				callback(Argument.Parse<T>(a));
			})));
		}

		private void Handle(string command, IEnumerable<object> objArgs)
		{
			if (this.Callbacks.ContainsKey(command.ToLowerInvariant()))
			{
				this.Callbacks[command].Handle(objArgs.Select(a => a.ToString()));
				API.CancelEvent();
				return;
			}

			if (command.ToLowerInvariant() != "reload") return;
			try
			{
				var args = objArgs.Select(a => new Name(a.ToString())).ToList();
				if (args.Count == 0) args = this.Controllers.Keys.ToList();

				foreach (var pluginName in args)
				{
					if (!this.Controllers.ContainsKey(pluginName)) continue;

					foreach (var controller in this.Controllers[pluginName])
					{
						var controllerType = controller.GetType();

						if (controllerType.BaseType != null && controllerType.BaseType.IsGenericType && controllerType.BaseType.GetGenericTypeDefinition() == typeof(ConfigurableController<>))
						{
							controllerType.GetMethods().FirstOrDefault(m => m.DeclaringType == controllerType && m.Name == "Reload")?.Invoke(
								controller,
								new[] { ConfigurationManager.InitializeConfig(pluginName, controllerType.BaseType.GetGenericArguments()[0]) }
							);
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
