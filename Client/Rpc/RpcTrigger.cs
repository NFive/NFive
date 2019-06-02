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
			this.logger.Trace($"Fire: \"{message.Event}\" with {message.Payloads.Count} payload(s): {string.Join(", ", message.Payloads)}");

			BaseScript.TriggerServerEvent(message.Event, this.serializer.Serialize(message));
		}
	}
}
