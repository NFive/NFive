using CitizenFX.Core;
using NFive.Client.Diagnostics;
using NFive.SDK.Core.Rpc;

namespace NFive.Client.Rpc
{
	public class RpcTrigger
	{
		private readonly Logger logger;
		private readonly Serializer serializer;
		private static int bandwidth;
		private static int bandwidthTime;

		public RpcTrigger(Logger logger, Serializer serializer)
		{
			this.logger = logger;
			this.serializer = serializer;
		}

		public void Fire(OutboundMessage message)
		{
			var serializedMessage = this.serializer.Serialize(message);
			var serializedMessageSize = serializedMessage.Length * 16;
			//this.logger.Debug($"Fire: \"{message.Event}\" with {message.Payloads.Count} payload(s) ({serializedMessage.Length * 16} bits): {string.Join(", ", message.Payloads)}");
			bandwidth += serializedMessageSize;
			this.logger.Debug($"Fire: \"{message.Event}\" with {message.Payloads.Count} payload(s) of total size '{serializedMessageSize}' bits");
			BaseScript.TriggerServerEvent(message.Event, serializedMessage);

			if (Game.GameTime <= bandwidthTime + 1000) return;
			bandwidthTime = Game.GameTime;
			this.logger.Debug($"RPC bits per second: {bandwidth}");
			bandwidth = 0;
		}
	}
}
