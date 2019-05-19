using CitizenFX.Core;
using NFive.SDK.Core.Rpc;
using NFive.Server.Diagnostics;

namespace NFive.Server.Rpc
{
	public class RpcTrigger
	{
		private readonly Logger logger;
		private readonly Serializer serializer;

		public RpcTrigger(Logger logger, Serializer serializer)
		{
			this.logger = logger;
			this.serializer = serializer;
		}

		public async void Fire(OutboundMessage message)
		{
			// Marshall back to the main thread in order to use a native call.
			await BaseScript.Delay(0);

			this.logger.Trace($"Fire: \"{message.Event}\" with {message.Payloads.Count} payload(s): {string.Join(", ", message.Payloads)}");

			if (message.Target != null)
			{
				var player = new PlayerList()[message.Target.Handle];
				this.logger.Debug($"Rpc message target player: {player.Name}");
				player.TriggerEvent(message.Event, this.serializer.Serialize(message));
			}
			else
			{
				BaseScript.TriggerClientEvent(message.Event, this.serializer.Serialize(message));
			}
		}
	}
}
