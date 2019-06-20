using CitizenFX.Core;
using NFive.Server.Diagnostics;
using System;

namespace NFive.Server.Rpc
{
	public class RpcTrigger
	{
		private readonly Logger logger;

		public RpcTrigger(Logger logger)
		{
			this.logger = logger;
		}

		public async void Fire(OutboundMessage message)
		{
			// Marshall back to the main thread in order to use a native call.
			await BaseScript.Delay(0);

			if (message.Payloads.Count > 0)
			{
				this.logger.Trace($"Fire: \"{message.Event}\" {(message.Target != null ? $"to {message.Target.Handle} " : string.Empty)}with {message.Payloads.Count} payload{(message.Payloads.Count > 1 ? "s" : string.Empty)}:{Environment.NewLine}\t{string.Join($"{Environment.NewLine}\t", message.Payloads)}");
			}
			else
			{
				this.logger.Trace($"Fire: \"{message.Event}\" {(message.Target != null ? $"to {message.Target.Handle} " : string.Empty)}with no payloads");
			}

			if (message.Target != null)
			{
				BaseScript.TriggerClientEvent(new PlayerList()[message.Target.Handle], message.Event, message.Pack());
			}
			else
			{
				BaseScript.TriggerClientEvent(message.Event, message.Pack());
			}
		}
	}
}
