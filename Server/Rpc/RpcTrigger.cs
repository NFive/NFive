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

			this.logger.Trace($"Fire: \"{message.Event}\" {(message.Target != null ? $"to {message.Target.Handle} " : string.Empty)}with {message.Payloads.Count} payload(s): {string.Join(", ", message.Payloads)}");

			var json = this.serializer.Serialize(message);

			if (message.Target != null)
			{
				BaseScript.TriggerClientEvent(new PlayerList()[message.Target.Handle], message.Event, json);
			}
			else
			{
				BaseScript.TriggerClientEvent(message.Event, json);
			}
		}
	}
}
