using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using CitizenFX.Core;
using NFive.SDK.Core.Diagnostics;
using NFive.SDK.Server.Communications;
using NFive.SDK.Server.Controllers;
using NFive.Server.Diagnostics;
using NFive.Server.Rpc;

namespace NFive.Server.Controllers
{
	public class EventController : Controller
	{
		private readonly ICommunicationManager comms;

		public EventController(ILogger logger, ICommunicationManager comms) : base(logger)
		{
			this.comms = comms;
		}

		public override Task Loaded()
		{
			// Client log mirroring
			this.comms.Event("nfive:log:mirror").FromClients().On<DateTime, LogLevel, string, string>(OnLogMirror);

			// Rebroadcast raw FiveM server events as wrapped server events
			RpcManager.OnRaw(FiveMServerEvents.PlayerConnecting, new Action<Player, string, CallbackDelegate, ExpandoObject>(OnPlayerConnectingRaw));
			RpcManager.OnRaw(FiveMServerEvents.PlayerDropped, new Action<Player, string>(OnPlayerDroppedRaw));

			RpcManager.OnRaw(FiveMServerEvents.ResourceStart, new Action<string>(OnResourceStartRaw));
			RpcManager.OnRaw(FiveMServerEvents.ResourceStop, new Action<string>(OnResourceStopRaw));

			// TODO: Move to client
			RpcManager.OnRaw("onClientResourceStart", new Action<Player, string>(OnClientResourceStartRaw));
			RpcManager.OnRaw("onClientResourceStop", new Action<Player, string>(OnClientResourceStopRaw));

			RpcManager.OnRaw("rconCommand", new Action<string, List<object>>(OnRconCommandRaw));
			RpcManager.OnRaw("gameEventTriggered", new Action<Player, string, List<dynamic>>(OnGameEventTriggeredRaw));
			// RpcManager.OnRaw(FiveMClientEvents.PopulationPedCreating, new Action<float, float, float, uint, IPopulationPedCreatingSetter>(OnPopulationPedCreatingRaw));

			return base.Loaded();
		}

		private static void OnLogMirror(ICommunicationMessage e, DateTime dt, LogLevel level, string prefix, string message)
		{
			new Logger(LogLevel.Trace, $"Client#{e.Client.Handle}|{prefix}").Log(message, level);
		}

		private void OnPlayerConnectingRaw([FromSource] Player player, string playerName, CallbackDelegate drop, ExpandoObject callbacks)
		{
			this.comms.Event("nfive:server:playerConnecting").ToServer().Emit(new Client(player.Handle), playerName, drop, callbacks);
		}

		private void OnPlayerDroppedRaw([FromSource] Player player, string reason)
		{
			this.comms.Event("nfive:server:playerDropped").ToServer().Emit(new Client(player.Handle), reason);
		}

		private void OnResourceStartRaw(string resourceName)
		{
			this.comms.Event("nfive:server:onResourceStart").ToServer().Emit(resourceName);
		}

		private void OnResourceStopRaw(string resourceName)
		{
			this.comms.Event("nfive:server:onResourceStop").ToServer().Emit(resourceName);
		}

		private void OnClientResourceStartRaw([FromSource] Player player, string resourceName)
		{
			this.comms.Event("nfive:server:onClientResourceStart").ToServer().Emit(new Client(player.Handle), resourceName);
		}

		private void OnClientResourceStopRaw([FromSource] Player player, string resourceName)
		{
			this.comms.Event("nfive:server:onClientResourceStop").ToServer().Emit(new Client(player.Handle), resourceName);
		}

		private void OnRconCommandRaw(string command, List<object> args)
		{
			this.comms.Event("nfive:server:rconCommand").ToServer().Emit(command, args.Select(a => a.ToString()).ToArray());
		}

		private void OnGameEventTriggeredRaw([FromSource] Player player, string @event, List<dynamic> args)
		{
			this.comms.Event("nfive:server:gameEventTriggered").ToServer().Emit(new Client(player.Handle), @event, args);
		}
	}
}
