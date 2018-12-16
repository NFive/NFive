using CitizenFX.Core.Native;
using NFive.SDK.Core.Diagnostics;
using NFive.SDK.Core.Plugins;
using NFive.SDK.Plugins.Configuration;
using NFive.SDK.Server.Controllers;
using NFive.SDK.Server.Events;
using NFive.SDK.Server.Rpc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NFive.Server.Controllers
{
	public class RconController : Controller
	{
		public RconController(Dictionary<Name, List<Controller>> controllers, ILogger logger, IEventManager events, IRpcHandler rpc) : base(logger, events, rpc)
		{
			this.Rpc.Event("rconCommand").OnRaw(new Action<string, List<object>>((c, a) => Handle(c, a, controllers)));
		}

		private void Handle(string command, IEnumerable<object> objArgs, Dictionary<Name, List<Controller>> controllers)
		{
			if (command.ToLowerInvariant() != "reload") return;
			try
			{
				this.Logger.Debug("Reload command called");

				var args = objArgs.Select(a => new Name(a.ToString())).ToList();
				if (args.Count == 0) args = controllers.Keys.ToList();

				foreach (var pluginName in args)
				{
					if (!controllers.ContainsKey(pluginName)) continue;

					foreach (var controller in controllers[pluginName])
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
