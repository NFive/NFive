using CitizenFX.Core;
using NFive.Client.Diagnostics;
using NFive.SDK.Core.Rpc;
using System;

namespace NFive.Client.Rpc
{
	public class RpcTrigger
	{
		private readonly Logger logger;

		public RpcTrigger(Logger logger)
		{
			this.logger = logger;
		}

		public void Fire(OutboundMessage message)
		{
			if (!message.Event.StartsWith("nfive:log:"))
			{
				if (message.Payloads.Count > 0)
				{
					this.logger.Trace($"Fire: \"{message.Event}\" with {message.Payloads.Count} payload{(message.Payloads.Count > 1 ? "s" : string.Empty)}:{Environment.NewLine}\t{string.Join($"{Environment.NewLine}\t", message.Payloads)}");
				}
				else
				{
					this.logger.Trace($"Fire: \"{message.Event}\" with no payloads");
				}
			}

			BaseScript.TriggerServerEvent(message.Event, message.Pack());
		}
	}
}
