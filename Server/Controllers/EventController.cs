using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using NFive.SDK.Core.Diagnostics;
using NFive.SDK.Core.Models;
using NFive.SDK.Core.Rpc;
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
			this.comms.Event("nfive:log:mirror").FromClients().On<DateTime, LogLevel, string, string>(OnLogMirror); // TODO: Enum

			// Rebroadcast raw FiveM server events as wrapped server events
			RpcManager.OnRaw(FiveMServerEvents.PlayerConnecting, new Action<Player, string, CallbackDelegate, ExpandoObject>(OnPlayerConnectingRaw));
			RpcManager.OnRaw(FiveMServerEvents.PlayerDropped, new Action<Player, string>(OnPlayerDroppedRaw));

			RpcManager.OnRaw(FiveMServerEvents.ResourceStart, new Action<string>(OnResourceStartRaw));
			RpcManager.OnRaw(FiveMServerEvents.ResourceStop, new Action<string>(OnResourceStopRaw));

			RpcManager.OnRaw(FiveMServerEvents.RconCommand, new Action<string, List<object>>(OnRconCommandRaw));
			RpcManager.OnRaw(FiveMServerEvents.ExplosionEvent, new Action<string, dynamic>(OnExplosionEventRaw));

			return base.Loaded();
		}

		private static void OnLogMirror(ICommunicationMessage e, DateTime dt, LogLevel level, string prefix, string message)
		{
			new Logger(LogLevel.Trace, $"Client#{e.Client.Handle}|{prefix}").Log(message, level);
		}

		private void OnPlayerConnectingRaw([FromSource] Player player, string playerName, CallbackDelegate drop, ExpandoObject callbacks)
		{
			this.comms.Event(NFiveServerEvents.PlayerConnecting).ToServer().Emit(new Client(player.Handle), new ConnectionDeferrals(callbacks, drop));
		}

		private void OnPlayerDroppedRaw([FromSource] Player player, string reason)
		{
			this.comms.Event(NFiveServerEvents.PlayerDropped).ToServer().Emit(new Client(player.Handle), reason);
		}

		private void OnResourceStartRaw(string resourceName)
		{
			this.comms.Event(NFiveServerEvents.ResourceStart).ToServer().Emit(resourceName);
		}

		private void OnResourceStopRaw(string resourceName)
		{
			this.comms.Event(NFiveServerEvents.ResourceStop).ToServer().Emit(resourceName);
		}

		private void OnRconCommandRaw(string command, List<object> args)
		{
			this.comms.Event("nfive:server:rconCommand").ToServer().Emit(command, args.Select(a => a.ToString()).ToArray());
		}

		private void OnExplosionEventRaw(string source, dynamic obj)
		{
			this.comms.Event("nfive:server:explosionEvent").ToServer().Emit(source, new ExplosionEvent(obj));
		}

		public class ExplosionEvent // TODO: Interface
		{
			public int OwnerNetId { get; }

			public int ExplosionType { get; }

			public float DamageScale { get; }

			public float CameraShake { get; }

			public Position Position { get; }

			public bool IsAudible { get; }

			public bool IsInvisible { get; }

			public ExplosionEvent(dynamic @event)
			{
				this.OwnerNetId = @event.ownerNetId;
				this.ExplosionType = @event.explosionType;
				this.DamageScale = @event.damageScale;
				this.CameraShake = @event.cameraShake;
				this.Position = new Position(@event.posX, @event.posY, @event.posZ);
				this.IsAudible = @event.isAudible;
				this.IsInvisible = @event.isInvisible;
			}

			public void Cancel()
			{
				API.CancelEvent();
			}
		}
	}
}
